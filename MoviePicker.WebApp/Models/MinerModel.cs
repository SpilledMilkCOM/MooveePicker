﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Utilities;
using SendGrid;
using SM.COMS.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePicker.WebApp.Models
{
	/// <summary>
	///  This is the singleton that contains all of the miners.
	/// </summary>
	public class MinerModel : IMinerModel
	{
		public const int FML_INDEX = 0;
		public const int MY_INDEX = FML_INDEX + 1;
		public const int TODD_INDEX = FML_INDEX + 2;
		public const int BOPRO_INDEX = FML_INDEX + 3;
		public const int MOJO_INDEX = FML_INDEX + 4;
		public const int COUPE_INDEX = FML_INDEX + 5;
		public const int VIS_REC_INDEX = FML_INDEX + 5;             // Same as above
		public const int PROPHET_INDEX = FML_INDEX + 6;
		public const int BORPT_INDEX = FML_INDEX + 7;
		public const int NUMBERS_THEATER_INDEX = FML_INDEX + 8;
		public const int MOJO_LAST_INDEX = FML_INDEX + 9;           // Miner contains the last week values (also contains all the history).

		private readonly IMailUtility _mailUtility;
		private bool _postersDownloaded;
		private TelemetryClient _telemetryClient;

		public MinerModel(bool createWithData, IMailUtility mailUtility, TelemetryClient telemetryClient)
		{
			_mailUtility = mailUtility;
			_telemetryClient = telemetryClient;

			if (createWithData)
			{
				Miners = CreateMinersWithData();
			}
		}

		public List<IMiner> Miners { get; private set; }

		public DateTime? WeekendEnding
		{
			get
			{
				return Miners.First().Movies?.FirstOrDefault()?.WeekendEnding;
			}
		}

		/// <summary>
		/// Clone ALL of the miners in the list.  NEW data may be loaded so adjsut for that.
		/// </summary>
		/// <returns>The MinerModel clone.</returns>
		public IMinerModel Clone()
		{
			var clone = new MinerModel(false, _mailUtility, _telemetryClient) { Miners = new List<IMiner>() };
			var idx = 0;
			var containsEstimates = Miners.Any() ? Miners[FML_INDEX].ContainsEstimates : false;

			// FML could change below which means ContainsEstimates might be out of date for these clones, it will be correct the NEXT time around.

			// Only download the posters once.

			clone._postersDownloaded = _postersDownloaded;

			// TODO: Why isn't this Parallel? (probably reasons)

			foreach (var miner in Miners)
			{
				miner.ContainsEstimates = containsEstimates;

				// Clone could possibly load in fresh data.
				clone.Miners.Add(miner.Clone());

				if (idx == MY_INDEX)
				{
					// The custom/mine/my (numbers) miner needs a reference to THIS model

					var mineMine = clone.Miners[idx] as MineMine;

					if (mineMine != null)
					{
						mineMine.Model = clone;
					}
				}

				idx++;
			}

			AssignTheaterCounts(clone);

			// If any of the miners are reloaded, then the composite movies (if any) need to be reassigned based on Todd's spread.
			// The miner's that have loaded their own composite movies will not be overwritten.

			if (clone.Miners.Any(miner => miner.CloneCausedReload))
			{
				var compoundMovies = CompoundMovies(clone.Miners[FML_INDEX].Movies);

				if (compoundMovies.Any())
				{
					compoundMovies = CompoundMovies(clone.Miners[TODD_INDEX].Movies);

					idx = TODD_INDEX + 1;

					foreach (var miner in clone.Miners.Skip(TODD_INDEX + 1))
					{
						if (miner.CloneCausedReload && !miner.CompoundLoaded && miner.Movies.Any())
						{
							if (!miner.Movies.Any(movie => movie.Day.HasValue))
							{
								// The list has no compound movies so they need to be built

								// Need to readjust the movies. (of the clone)
								miner.SetMovies(SpreadCompoundMovies(compoundMovies, miner.Movies));

								// Need to readjust the movies. (of the singleton)
								Miners[idx].SetMovies(SpreadCompoundMovies(compoundMovies, Miners[idx].Movies));

								if (miner == clone.Miners.Last())
								{
									LoadLastBoxOfficeMojoCompound(miner as MineBoxOfficeMojo, compoundMovies.First());
								}
							}
						}

						idx++;
					}
				}

				// Loop through all of the reloaded miners and adjust their movies.

				foreach (var miner in clone.Miners.Where(miner => miner.CloneCausedReload && miner != clone.Miners[FML_INDEX]))
				{
					if (miner != clone.Miners[MY_INDEX])
					{
						_telemetryClient.TrackTrace($"Clone caused miner to reload '{miner.Name}'", SeverityLevel.Information
									, new Dictionary<string, string> {
											{ "minerName", miner.Name }
											, { "gameDays", miner.GameDays.ToString() }
											, { "lastUpdated", miner.LastUpdated?.ToString() }
											, { "isNewData", miner.IsNewData.ToString() }
											, { "twitterId" , miner.TwitterID } });
					}

					foreach (var movie in clone.Miners[FML_INDEX].Movies)
					{
						AssignCostIdName(movie, miner.Movies);
					}

					FilterOutMovieIdZero(miner);

					var masterMiner = Miners.FirstOrDefault(item => item.Name == miner.Name);

					if (masterMiner != null)
					{
						// There is still nothing to prevent this from happening twice (or multiple times).

						lock (masterMiner)
						{
							// The MinerBase.Clone() method for explanation of cloning this list.

							masterMiner.SetMovies(new List<IMovie>(miner.Movies));

							// Only need to make the picks ONCE after it's loaded.
							// (no need to make the picks in the controller for every request).

							if (masterMiner != Miners[MY_INDEX])
							{
								// Do not make picks for "my picks" because the BO values have not been set yet (from request string)

								MakePicks(masterMiner);
							}
						}
					}
				}

				if (containsEstimates && clone.Miners[FML_INDEX].CloneCausedReload)
				{
					LoadBoxOfficeMojoEstimates(clone.Miners[FML_INDEX]);

					var masterMiner = Miners[FML_INDEX];

					if (masterMiner != null)
					{
						// There is still nothing to prevent this from happening twice (or multiple times).

						lock (masterMiner)
						{
							LoadBoxOfficeMojoEstimates(masterMiner);
						}
					}
				}
			}

			FilterMinersMovies(clone.Miners);

			return clone;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public List<IMiner> CreateMinersWithData()
		{
			var miners = CreateMiners();

			// Need this set BEFORE mining.

			Miners = miners;

			MineMiners(miners);

			FilterMinersMovies(miners);

			AssignTheaterCounts(this);

			MakePicks(miners);

			return miners;
		}

		/// <summary>
		/// Create box office earning numbers based on all of the miners' weights.
		/// </summary>
		/// <returns></returns>
		public List<IMovie> CreateWeightedList()
		{
			var result = new List<IMovie>();

			if (Miners != null)
			{
				// FML should have all of the movies WITH the Bux
				// Use the FML data to copy the bux to each list.

				var baseMovies = Miners[FML_INDEX].Movies;

				foreach (var movie in baseMovies.OrderByDescending(item => item.Cost))
				{
					// My list is based on how well I trust these sources.

					result.Add(CreateWeightedMovie(movie));
				}
			}

			return result;
		}

		/// <summary>
		/// Makes sure that the movie posters are downloaded locally.  (This is so the URL referenced isn't used a TON.)
		/// Whether the file was downloaded or not, the ImageUrl is updated to the local file (provided it exists).
		/// 
		/// </summary>
		/// <returns>A flag to update the image URL</returns>
		public bool DownloadMoviePosters(string localFilePrefix)
		{
			var imageUtil = new ImageUtility();
			var miner = Miners[FML_INDEX];
			var result = FileUtility.DownloadFiles(miner.Movies.Select(movie => movie.ImageUrlSource), localFilePrefix);
			var multiDayCount = miner.Movies.Where(item => item.Day.HasValue).Count();

			foreach (var movie in miner.Movies)
			{
				var localFileName = FileUtility.LocalFile(movie.ImageUrlSource, localFilePrefix);

				if (File.Exists(localFileName))
				{
					// If the file was actually downloaded then use the local file.

					if (result)
					{
						imageUtil.AdjustSize(localFileName, 200, 300);          // Shrink image a bit.
					}

					if (movie.Day.HasValue)
					{
						var multiDay = (multiDayCount == 2) ? (movie.Day.Value == DayOfWeek.Friday ? "-Saturday" : "-Monday") : string.Empty;
						var maskedFileName = $"{Path.GetFileNameWithoutExtension(localFileName)}-{movie.Day.Value}{multiDay}.jpg";

						if (!File.Exists(Path.Combine(Path.GetDirectoryName(localFileName), maskedFileName)))
						{
							imageUtil.MaskImage(localFileName, $"{movie.Day.Value}{multiDay}-mask.png", maskedFileName);
						}

						movie.ImageUrl = $"/Images/{maskedFileName}";
					}
					else
					{
						movie.ImageUrl = $"/Images/{Path.GetFileName(localFileName)}";
					}
				}
				else
				{
					movie.ImageUrl = movie.ImageUrlSource;
				}
			}

			result |= !_postersDownloaded;

			// Only download ONCE per instance. (Meaning only update the ImageUrl once per instance since there is a lock() used to do this.)

			_postersDownloaded = true;

			return result;
		}

		/// <summary>
		/// Send an email if any of the miners were loaded.
		/// </summary>
		/// <param name="miners"></param>
		public async Task<Response> EmailLoadedMinersAsync()
		{
			var minersLoaded = string.Empty;
			var myMiner = Miners[MY_INDEX];
			// Create a text list of miners that were loaded.

			var loadedMiners = Miners.Where(miner => miner.CloneCausedReload && miner.IsNewData && miner.Name != myMiner.Name).ToList();

			if (loadedMiners.Any())
			{
				loadedMiners.ForEach(miner => minersLoaded += $"{miner.Name}\r\n");

				return await _mailUtility.SendAsync(null, null
					, $"MVP: {loadedMiners.Count} Miner(s) Loaded", $"Hey,\r\n\r\nThe following miners were loaded:\r\n\r\n{minersLoaded}\r\nTHANKS!,\r\n{Constants.APPLICATION_NAME}.");
			}

			return null;
		}

		/// <summary>
		/// Expire all of the miners so they reload the next time.
		/// </summary>
		public void Expire()
		{
			foreach (ICache miner in Miners)
			{
				miner.Expire();
			}
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		/// <summary>
		/// Assign base movie attributes to the movie passed in.
		/// </summary>
		/// <param name="sourceMovie">The movie containing the base FML data.</param>
		/// <param name="movies">A list of base movies to search</param>
		private void AssignCostIdName(IMovie sourceMovie, IEnumerable<IMovie> movies)
		{
			// Need to do the "fuzzy" compare because the MovieName and Id have not been assigned yet.
			var found = movies?.FirstOrDefault(item => item.Equals(sourceMovie));

			if (found != null)
			{
				found.Id = sourceMovie.Id;                      // Will be able to do quicker Finds.
				found.MovieName = sourceMovie.MovieName;        // So the names aren't fuzzy anymore.
				found.Cost = sourceMovie.Cost;
				found.ControlId = sourceMovie.ControlId;
				//found.TheaterCount = sourceMovie.TheaterCount;
			}
		}

		/// <summary>
		/// Assign the theater counts from MojoTheaterCount.
		/// NOTE: Reloading the FML movies will overwrite the theater count so then need to be corrected.
		/// </summary>
		/// <param name="clone"></param>
		private void AssignTheaterCounts(MinerModel clone)
		{
			var theaterCountMiner = clone.Miners[NUMBERS_THEATER_INDEX];
			var fmlMiner = clone.Miners[FML_INDEX];

			if (theaterCountMiner.CloneCausedReload || fmlMiner.CloneCausedReload)
			{
				foreach (var movie in fmlMiner.Movies)
				{
					var found = theaterCountMiner?.Movies?.FirstOrDefault(item => item.Equals(movie));

					if (found != null)
					{
						movie.TheaterCount = found.TheaterCount;
					}
				}

				var masterMiner = Miners[FML_INDEX];

				if (masterMiner != null)
				{
					// There is still nothing to prevent this from happening twice (or multiple times).

					lock (masterMiner)
					{
						// The MinerBase.Clone() method for explanation of cloning this list.

						masterMiner.SetMovies(new List<IMovie>(fmlMiner.Movies));
					}
				}
			}
		}

		/// <summary>
		/// Return a list of the compound movies (Day has a value)
		/// </summary>
		/// <param name="movies"></param>
		/// <returns>List of compound/breakout movies.</returns>
		private List<IMovie> CompoundMovies(IList<IMovie> movies)
		{
			return movies.Where(movie => movie.Day.HasValue).ToList();
		}

		/// <summary>
		/// Create all of the movie miners.
		/// </summary>
		/// <returns></returns>
		private List<IMiner> CreateMiners()
		{
			var result = new List<IMiner> {
				new MineFantasyMovieLeagueBoxOffice { IsHidden = true },
				new MineMine(this) { Weight = 1 },
				new MineToddThatcher(),
				new MineBoxOfficePro(),
				new MineBoxOfficeMojo(),
				//new MineVisualRecreation(),
				new MineCoupe { IsHidden = true, OkToMine = false },
				//new MineCulturedVultures(),
				new MineBoxOfficeProphet(),
				new MineBoxOfficeReport(),
				new MineTheNumbers { IsHidden = true }
			};

			// Grab last weeks results for comparisons.  Always put this list last.

			result.Add(new MineBoxOfficeMojo(MovieDateUtil.LastSunday(MovieDateUtil.GameSunday().AddDays(-1))));

			var properties = new Dictionary<string, string>();
			var index = 0;

			foreach (var miner in result)
			{
				properties.Add($"miner{index:00}Name", miner.Name);
				index++;
			}

			_telemetryClient.TrackTrace($"Constructed Miners", SeverityLevel.Information, properties);

			return result;
		}

		/// <summary>
		/// Create a single movie with the Earnings calculated from the mined data.
		/// </summary>
		/// <param name="baseMovie">The movie being matched (from FML Nerd)</param>
		/// <param name="movieData">All of the movie lists.</param>
		/// <param name="miners">All of the movie miners.</param>
		/// <returns></returns>
		private IMovie CreateWeightedMovie(IMovie baseMovie)
		{
			decimal totalWeight = 0;
			decimal earnings = 0;

			// Initialize the result movie with base data (excluding the earnings)

			var result = new Movie
			{
				ControlId = baseMovie.ControlId,
				Cost = baseMovie.Cost,
				Day = baseMovie.Day,
				Id = baseMovie.Id,
				ImageUrl = baseMovie.ImageUrl,
				MovieName = baseMovie.MovieName,
				TheaterCount = baseMovie.TheaterCount,
				WeekendEnding = baseMovie.WeekendEnding
			};

			// The scan includes "My" data.

			for (int index = MY_INDEX; index < Miners.Count; index++)
			{
				// Compare the Id first so this comparison can short circuit and be quicker.
				var foundMovie = Miners[index]?.Movies?.FirstOrDefault(item => ((item.Id != 0 && item.Id == baseMovie.Id) || item.Equals(baseMovie)) && WeekendEndingMatch(result.WeekendEnding, item.WeekendEnding));

				if (foundMovie != null)
				{
					// Apply weights to all the miners, but only to "My" data if there are earnings. (don't factor in a zero)

					if (index != MY_INDEX || foundMovie.EarningsBase > 0)
					{
						earnings += foundMovie.EarningsBase * Miners[index].Weight;
						totalWeight += Miners[index].Weight;
					}
				}
			}

			// Verify that this movie was even picked within the other miners.

			if (totalWeight > 0)
			{
				// NOTE: Since Earnings may change based on OTHER movie's earnings, the EarningsBase needs to be used ABOVE and BELOW.

				earnings /= totalWeight;     // Weighted average.
			}

			result.Earnings = earnings;

			return result;
		}

		private bool WeekendEndingMatch(DateTime? baseWeekendEnding, DateTime? minerWeekendEnding)
		{
			return (minerWeekendEnding == baseWeekendEnding
				|| (minerWeekendEnding.HasValue && baseWeekendEnding.HasValue && minerWeekendEnding.Value.AddDays(1) == baseWeekendEnding));
		}

		/// <summary>
		/// Filter out the data that does not match the base date.
		/// </summary>
		/// <param name="minerData"></param>
		private void FilterMinersMovies(List<IMiner> minerData)
		{
			DateTime? weekendEnding = minerData[FML_INDEX].Movies?.FirstOrDefault()?.WeekendEnding;

			for (int index = FML_INDEX + 1; index < minerData.Count - 1; index++)
			{
				FilterMinerMovies(minerData[index], weekendEnding);
			}

			// BO Mojo has a HUGE list of movies that were mined

			FilterOutMovieIdZero(minerData.Last());
		}

		private void FilterMinerMovies(IMiner miner, DateTime? weekendEnding)
		{
			var hasData = miner.Movies?.Count > 0;
			var minerWeekendEnding = miner.Movies?.FirstOrDefault()?.WeekendEnding;

			if (WeekendEndingMatch(weekendEnding, minerWeekendEnding))
			{
				// Remove movies that did not get mapped.  Most likely duplicates or bad data.

				FilterOutMovieIdZero(miner);
			}
			else
			{
				if (hasData && (string.IsNullOrEmpty(miner.Error) || miner.Error?.IndexOf("Error") < 0))
				{
					// Only set this if there was data and there's no error.
					miner.Error = "Old Data";
					miner.ErrorDetail = $"The box office data is from the weekend ending {miner.Movies?.FirstOrDefault()?.WeekendEnding.ToShortDateString()}";

					if (miner.CloneCausedReload)
					{
						// Only log this if the miner loaded (not EVERY time)

						_telemetryClient.TrackTrace($"Old Data", SeverityLevel.Warning
								, new Dictionary<string, string> { { "name", miner.Name }, { "errorDetail", miner.ErrorDetail } });
					}
				}

				miner.Clear();
			}
		}

		private void FilterOutMovieIdZero(IMiner miner)
		{
			var movies = miner.Movies;                                              // Returns a copy of the the movies.
			var moviesToRemove = movies.Where(movie => movie.Id == 0).ToList();     // Need a new list of this

			if (moviesToRemove.Any())
			{
				foreach (var movie in moviesToRemove)
				{
					movies.Remove(movie);
				}

				miner.SetMovies(movies);     // Update the internal list with the reduced size.
			}
		}

		/// <summary>
		/// Replace the estimates in the FML Miner with the Box Office Mojo estimates.
		/// </summary>
		/// <param name="fmlMiner">The FML Miner with estimate data.</param>
		private void LoadBoxOfficeMojoEstimates(IMiner fmlMiner)
		{
			//var gameSunday = MovieDateUtil.LastSunday();
			var gameSunday = MovieDateUtil.GameSunday();
			var mojoEstimates = new MineBoxOfficeMojo(gameSunday);
			var mojoMovies = mojoEstimates.Mine();

			FilterMinerMovies(mojoEstimates, gameSunday);

			if (fmlMiner.Movies != null && fmlMiner.Movies.Any()
				&& mojoMovies != null && mojoMovies.Any())
			{
				foreach (var fmlMovie in fmlMiner.Movies)
				{
					var found = mojoMovies.FirstOrDefault(item => item.Equals(fmlMovie));

					if (found != null && found.WeekendEnding == fmlMovie.WeekendEnding)
					{
						fmlMovie.Earnings = found.EarningsBase;     // DON'T assign the bonus (if there is one).
					}
				}
			}
		}

		/// <summary>
		/// Load the compound movies for the previous week.
		/// </summary>
		/// <param name="lastMojoMiner"></param>
		/// <param name="firstCompoundMovie"></param>
		private void LoadLastBoxOfficeMojoCompound(MineBoxOfficeMojo lastMojoMiner, IMovie firstCompoundMovie)
		{
			var mojoMovies = lastMojoMiner.Movies;              // A copy of its movie list

			// Last week's BO Mojo movies need to be loaded on their own and NOT rely on the current compound movie spread.
			// (Only this miner mines the Identifier so you have to pull the movie from this miner's list of movies.)

			var firstMojoMovie = mojoMovies.FirstOrDefault(item => item.Equals(firstCompoundMovie));
			var dailyMiner = new MineBoxOfficeMojoDaily(firstMojoMovie?.Identifier, lastMojoMiner.WeekendEnding);

			var movies = dailyMiner.Mine();         // Each movie will have their corresponding DayOfWeek set.

			foreach (var movie in movies)
			{
				movie.Name = firstMojoMovie.MovieName;
				var found = mojoMovies.FirstOrDefault(item => item.Equals(movie));

				if (found != null)
				{
					found.Earnings = movie.Earnings;
					found.WeekendEnding = movie.WeekendEnding;
				}
			}

			lastMojoMiner.SetCompoundLoaded(true);
			lastMojoMiner.SetMovies(mojoMovies);
		}

		/// <summary>
		/// Make the FML picks for a miner (both with bonus and without)
		/// </summary>
		/// <param name="miner">The miner to adjust the picks.</param>
		private void MakePicks(IMiner miner)
		{
			if (miner.Movies != null && miner.Movies.Any())
			{
				// Need to clone the list otherwise the above MovieList will lose its BestPerformer.

				var clonedList = new List<IMovie>();

				foreach (var movie in miner.Movies.Where(item => item.Earnings > 0))
				{
					clonedList.Add(movie.Clone());
				}

				// TODO: This needs to be an injected prototype.

				IMoviePicker moviePicker = new MsfMovieSolver();

				moviePicker.AddMovies(clonedList);

				miner.Picks = moviePicker.ChooseBest();

				clonedList.Clear();

				foreach (var movie in miner.Movies.Where(item => item.Earnings > 0))
				{
					clonedList.Add(movie.Clone());
				}

				moviePicker.AddMovies(clonedList);
				moviePicker.EnableBestPerformer = false;

				miner.PicksBonusOff = moviePicker.ChooseBest();
			}
		}

		private void MakePicks(List<IMiner> miners)
		{
			//Parallel.ForEach(miners.Skip(TODD_INDEX).Take(miners.Count - TODD_INDEX - 1), miner =>
			foreach (var miner in miners.Skip(TODD_INDEX).Take(miners.Count - TODD_INDEX - 1))
			{
				MakePicks(miner);
			}//);
		}

		/// <summary>
		/// Mine all of the miner movie data.
		/// </summary>
		/// <param name="miners"></param>
		/// <returns></returns>
		private List<List<IMovie>> MineMiners(IEnumerable<IMiner> miners)
		{
			var result = new List<List<IMovie>>();
			int toSkip = 2;         // Skip FML and "My" (custom) miner
			List<IMovie> baseList = null;
			List<IMovie> compoundMovies = null;

			// !!!!! FML Base list is first !!!!!

			_telemetryClient.TrackTrace($"Loading Miner", SeverityLevel.Information, new Dictionary<string, string> { { "name", miners.First().Name } });

			((ICache)miners.First()).Load();
			baseList = miners.First().Movies;
			result.Add(baseList);

			// Get the contains estimates value from the FML Miner.
			var containsEstimates = miners.First().ContainsEstimates;
			var weekendEnding = WeekendEnding;      // The actual end date of the game.
			var gameDays = WeekendEnding.HasValue ? WeekendEnding.Value.Subtract(MovieDateUtil.GameStartTime(WeekendEnding.Value)).Days + 1 : 3;

			if (containsEstimates)
			{
				_telemetryClient.TrackTrace($"Loading Miner", SeverityLevel.Information, new Dictionary<string, string> { { "name", "Box Office Mojo Estimates" } });

				LoadBoxOfficeMojoEstimates(miners.First());
			}

			// TODO: Fix this for the FML base list.  This will break when another compound movie (multi-day) comes into play.

			compoundMovies = CompoundMovies(baseList);

			if (compoundMovies.Any())
			{
				// Todd's list is preferred for compound movies.

				var minerTodd = miners.ToList()[TODD_INDEX];

				if (minerTodd != null)
				{
					_telemetryClient.TrackTrace($"Loading Miner", SeverityLevel.Information, new Dictionary<string, string> { { "name", minerTodd.Name } });

					((ICache)minerTodd).Load();

					compoundMovies = CompoundMovies(minerTodd.Movies);

					// Assign the id, name, and cost to each movie.

					foreach (var movie in baseList)
					{
						AssignCostIdName(movie, minerTodd.Movies);
					}

					toSkip++;       // Now skip Todd's too.
				}
			}

			Parallel.ForEach(miners.Skip(toSkip), miner =>
			//foreach (var miner in miners.Skip(toSkip))
			{
				try
				{
					if (miner != baseList)
					{
						miner.ContainsEstimates = containsEstimates;
						miner.GameDays = gameDays;

						List<IMovie> movieList = null;

						if (miner.OkToMine)
						{
							_telemetryClient.TrackTrace($"Loading Miner", SeverityLevel.Information, new Dictionary<string, string> { { "name", miner.Name } });

							((ICache)miner).Load();

							movieList = miner.Movies;
						}

						lock (result)
						{
							result.Add(movieList);
						}

						if (movieList != null && movieList.Any())
						{
							if (compoundMovies.Any())
							{
								if (!movieList.Any(movie => movie.Day.HasValue))
								{
									// The list has no compound movies so they need to be built
									// Need to readjust the movies.
									miner.SetMovies(SpreadCompoundMovies(compoundMovies, movieList));
								}

								if (miner == miners.Last())
								{
									LoadLastBoxOfficeMojoCompound(miner as MineBoxOfficeMojo, compoundMovies.First());
								}
							}

							// Assign the id, name, and cost to each movie.

							foreach (var movie in baseList)
							{
								AssignCostIdName(movie, movieList);
							}
						}
					}
				}
				catch (Exception ex)
				{
					lock (result)
					{
						result.Add(new List<IMovie>());     // Add a placeholder.
					}

					miner.Error = "Error";
					miner.ErrorDetail = ex.Message;

					_telemetryClient.TrackTrace($"Load Error", SeverityLevel.Error
							, new Dictionary<string, string> { { "name", miner.Name }, { "errorDetail", miner.ErrorDetail } });
				}
				//}
			});

			// Mine my/custom movies LAST because they are based on all of the other miners.

			miners.ToList()[MY_INDEX].Mine();

			_telemetryClient.TrackTrace($"Finished Loading Miners", SeverityLevel.Information);

			return result;
		}

		private List<IMovie> SpreadCompoundMovies(List<IMovie> compoundMovies, List<IMovie> movies)
		{
			var rootMovie = movies.FirstOrDefault(movie => movie.Equals(compoundMovies.FirstOrDefault()));
			var compoundTotal = compoundMovies.Sum(movie => movie.Earnings);

			if (rootMovie != null)
			{
				var theaterCount = rootMovie.TheaterCount;

				movies.Remove(rootMovie);

				foreach (var movieDay in compoundMovies)
				{
					movies.Add(new Movie
					{
						Name = movieDay.MovieName,
						Day = movieDay.Day,
						Earnings = movieDay.Earnings / compoundTotal * rootMovie.Earnings,
						Identifier = rootMovie.Identifier,
						TheaterCount = theaterCount,
						WeekendEnding = movieDay.WeekendEnding
					});
				}
			}

			return movies;
		}
	}
}