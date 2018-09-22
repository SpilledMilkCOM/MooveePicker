using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviePicker.WebApp.Models
{
	/// <summary>
	///  This is the singleton that contains all of the miners.
	/// </summary>
	public class MinerModel : IMinerModel
	{
		//private const int NERD_INDEX = 1;
		public const int FML_INDEX = 0;
		private const int MY_INDEX = FML_INDEX + 1;
		private const int TODD_INDEX = FML_INDEX + 2;

		private bool _postersDownloaded;

		public MinerModel(bool createWithData)
		{
			if (createWithData)
			{
				Miners = CreateMinersWithData();
			}
		}

		public List<IMiner> Miners { get; private set; }

		public IMinerModel Clone()
		{
			var clone = new MinerModel(false) { Miners = new List<IMiner>() };
			int idx = 0;

			// Only download the posters once.

			clone._postersDownloaded = _postersDownloaded;

			foreach (var miner in Miners)
			{
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

			// If any of the miners reloaded, then the composite movies (in any) need to be refigured.

			if (clone.Miners.Any(miner => miner.CloneCausedReload))
			{
				var compoundMovies = CompoundMovies(clone.Miners[FML_INDEX].Movies);

				if (compoundMovies.Any())
				{
					compoundMovies = CompoundMovies(clone.Miners[TODD_INDEX].Movies);

					idx = TODD_INDEX + 1;

					foreach (var miner in clone.Miners.Skip(TODD_INDEX + 1))
					{
						if (miner.CloneCausedReload && miner.Movies.Any())
						{
							if (!miner.Movies.Any(movie => movie.Day.HasValue))
							{
								// The list has no compound movies so they need to be built

								// Need to readjust the movies. (of the clone)
								miner.SetMovies(SpreadCompoundMovies(compoundMovies, miner.Movies));

								// Need to readjust the movies. (of the singleton)
								Miners[idx].SetMovies(SpreadCompoundMovies(compoundMovies, Miners[idx].Movies));
							}
						}

						idx++;
					}
				}

				// Loop through all of the reloaded miners and adjust their movies.

				foreach (var miner in clone.Miners.Where(miner => miner.CloneCausedReload && miner != clone.Miners[FML_INDEX]))
				{
					foreach (var movie in clone.Miners[FML_INDEX].Movies)
					{
						AssignCostIdName(movie, miner.Movies);
					}

					FilterOutMovieIdZero(miner);

					var masterMiner = Miners.FirstOrDefault(item => item.Name == miner.Name);

					if (masterMiner != null)
					{
						// The MinerBase.Clone() method for explation of cloning this list.

						masterMiner.SetMovies(new List<IMovie>(miner.Movies));
					}
				}
			}

			FilterMinerMovies(clone.Miners);

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

			FilterMinerMovies(miners);

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

			foreach (var movie in miner.Movies)
			{
				//var localFileName = Path.GetFileName(FileUtility.LocalFile(movie.ImageUrlSource, localFilePrefix));
				var localFileName = FileUtility.LocalFile(movie.ImageUrlSource, localFilePrefix);

				if (File.Exists(localFileName))
				{
					// If the file was actually downloaded then use the local file.

					if (result)
					{
						//imageUtil.AdjustSize(localFileName, 300, 450);		// Closer to original size
						//imageUtil.AdjustAspectRatio(localFileName, 2 / 3m);
						imageUtil.AdjustSize(localFileName, 200, 300);			// Shrink image a bit.
					}

					if (movie.Day.HasValue)
					{
						var maskedFileName = $"{Path.GetFileNameWithoutExtension(localFileName)}-{movie.Day.Value}.jpg";

						if (!File.Exists(Path.Combine(Path.GetDirectoryName(localFileName), maskedFileName)))
						{
							imageUtil.MaskImage(localFileName, $"{movie.Day.Value}-mask.png", maskedFileName);
						}

						movie.ImageUrl = $"/Images/{maskedFileName}";
					}
					else
					{
						movie.ImageUrl = $"/Images/{Path.GetFileName(localFileName)}";
					}
				}
			}

			result |= !_postersDownloaded;

			// Only download ONCE per instance. (Meaning only update the ImageUrl once per instance since there is a lock() used to do this.)

			_postersDownloaded = true;

			return result;
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
		/// <param name="movie">The movie to find so the</param>
		/// <param name="movies">A list of base movies to search</param>
		private void AssignCostIdName(IMovie movie, IEnumerable<IMovie> movies)
		{
			var found = movies?.FirstOrDefault(item => item.Equals(movie));

			if (found != null)
			{
				found.Id = movie.Id;
				found.MovieName = movie.MovieName;        // So the names aren't fuzzy anymore.
				found.Cost = movie.Cost;
			}
		}

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
				new MineCoupe(),
				//new MineCulturedVultures(),
				new MineBoxOfficeProphet(),
				new MineBoxOfficeReport()
			};

			// Grab last weeks results for comparisons.  Always put this list last.

			result.Add(new MineBoxOfficeMojo(MovieDateUtil.LastSunday(MovieDateUtil.GameSunday().AddDays(-1))));

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

			// Initialize the result movie with base data (excluding the earnings)

			var result = new Movie
			{
				Id = baseMovie.Id,
				ImageUrl = baseMovie.ImageUrl,
				MovieName = baseMovie.MovieName,
				Day = baseMovie.Day,
				Cost = baseMovie.Cost,
				WeekendEnding = baseMovie.WeekendEnding
			};

			// The scan includes "My" data.

			for (int index = MY_INDEX; index < Miners.Count; index++)
			{
				var foundMovie = Miners[index]?.Movies?.FirstOrDefault(item => item.Equals(baseMovie) && item.WeekendEnding == result.WeekendEnding);

				if (foundMovie != null)
				{
					// Apply weights to all the miners, but only to "My" data if there are earnings. (don't factor in a zero)

					if (index != MY_INDEX || foundMovie.Earnings > 0)
					{
						result.Earnings += foundMovie.Earnings * Miners[index].Weight;
						totalWeight += Miners[index].Weight;
					}
				}
			}

			// Verify that this movie was even picked within the other miners.

			if (totalWeight > 0)
			{
				result.Earnings /= totalWeight;     // Weighted average.
			}

			return result;
		}

		/// <summary>
		/// Filter out the data that does not match the base date.
		/// </summary>
		/// <param name="minerData"></param>
		private void FilterMinerMovies(List<IMiner> minerData)
		{
			DateTime? weekendEnding = minerData[FML_INDEX].ContainsEstimates ? minerData[FML_INDEX].Movies?.FirstOrDefault()?.WeekendEnding : MovieDateUtil.GameSunday(isEstimate: minerData[FML_INDEX].ContainsEstimates);

			for (int index = FML_INDEX + 1; index < minerData.Count - 1; index++)
			{
				var hasData = minerData[index].Movies?.Count > 0;

				if (minerData[index].Movies?.FirstOrDefault()?.WeekendEnding != weekendEnding)
				{
					if (hasData && (string.IsNullOrEmpty(minerData[index].Error) || minerData[index].Error?.IndexOf("Error") < 0))
					{
						// Only set this if there was data and there's no error.
						minerData[index].Error = "Old Data";
						minerData[index].ErrorDetail = $"The box office data is from the weekend ending {minerData[index].Movies?.FirstOrDefault()?.WeekendEnding.ToShortDateString()}";
					}

					minerData[index].Clear();
				}
			}

			// BO Mojo has a HUGE list of movies that were mined

			FilterOutMovieIdZero(minerData.Last());

			//var lastWeekMovies = minerData.Last().Movies;       // Returns a copy of the the movies.
			//var moviesToRemove = minerData.Last().Movies.Where(movie => movie.Id == 0);

			//foreach (var movie in moviesToRemove)
			//{
			//	lastWeekMovies.Remove(movie);
			//}

			//minerData.Last().SetMovies(lastWeekMovies);     // Update the internal list with the reduced size.
		}

		private void FilterOutMovieIdZero(IMiner miner)
		{
			var movies = miner.Movies;                                              // Returns a copy of the the movies.
			var moviesToRemove = movies.Where(movie => movie.Id == 0).ToList();     // Need a new list of this

			foreach (var movie in moviesToRemove)
			{
				movies.Remove(movie);
			}

			miner.SetMovies(movies);     // Update the internal list with the reduced size.
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

			// FML Base list is first.

			((ICache)miners.First()).Load();
			baseList = miners.First().Movies;
			result.Add(baseList);

			// TODO: Fix this for the FML base list.  This will break when another compound movie (multi-day) comes into play.

			compoundMovies = CompoundMovies(baseList);

			if (compoundMovies.Any())
			{
				// Todd's list is preferred for compound movies.

				var minerTodd = miners.ToList()[TODD_INDEX];

				if (minerTodd != null)
				{
					((ICache)minerTodd).Load();

					compoundMovies = CompoundMovies(minerTodd.Movies);

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
						List<IMovie> movieList = null;

						if (miner.OkToMine)
						{
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

					//Logger.WriteLine($"EXCEPTION: Mining data for {miner.Name} -- {ex.Message}");
				}
				//}
			});

			// Mine my/custom movies LAST because they are based on all of the other miners.

			miners.ToList()[MY_INDEX].Mine();

			return result;
		}

		private List<IMovie> SpreadCompoundMovies(List<IMovie> compoundMovies, List<IMovie> movies)
		{
			var rootMovie = movies.FirstOrDefault(movie => movie.Equals(compoundMovies.FirstOrDefault()));
			var compoundTotal = compoundMovies.Sum(movie => movie.Earnings);

			if (rootMovie != null)
			{
				movies.Remove(rootMovie);

				foreach (var movieDay in compoundMovies)
				{
					movies.Add(new Movie
					{
						Name = movieDay.MovieName,
						Day = movieDay.Day,
						Earnings = movieDay.Earnings / compoundTotal * rootMovie.Earnings,
						WeekendEnding = movieDay.WeekendEnding
					});
				}
			}

			return movies;
		}
	}
}