using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviePicker.WebApp.Models
{
	public class MinerModel : IMinerModel
	{
		//private const int NERD_INDEX = 1;
		private const int FML_INDEX = 0;
		private const int MY_INDEX = FML_INDEX + 1;

		public MinerModel()
		{
			Miners = CreateMinersWithData();
		}

		public List<IMiner> Miners { get; private set; }

		public List<IMiner> CreateMinersWithData()
		{
			var miners = CreateMiners();

			// Need this set BEFORE mining.

			Miners = miners;

			MineMiners(miners);

			FilterMinerMovies(miners);

			return miners;
		}

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

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AssignCost(IMovie movie, IEnumerable<IMovie> movies)
		{
			var found = movies?.FirstOrDefault(item => item.Equals(movie));

			if (found != null)
			{
				found.Id = movie.Id;
				found.MovieName = movie.MovieName;        // So the names aren't fuzzy anymore.
				found.Cost = movie.Cost;
			}
		}

		/// <summary>
		/// Create all of the movie miners.
		/// </summary>
		/// <returns></returns>
		private List<IMiner> CreateMiners()
		{
			var result = new List<IMiner> {
				new MineFantasyMovieLeagueBoxOffice { IsHidden = true, Weight = 0 },
//				new MineNerd { Weight = 1 },
				new MineMine(this) {Weight = 0},
				new MineToddThatcher { Weight = 3 },
				new MineBoxOfficePro { Weight = 4 },
				new MineBoxOfficeMojo { Weight = 3 },
				new MineCulturedVultures { Weight = 2 },
				new MineBoxOfficeProphet { Weight = 2 },
				new MineBoxOfficeReport { Weight = 3 }
			};

			// Grab last weeks results for comparisons.  Always put this list last.

			result.Add(new MineBoxOfficeMojo(MovieDateUtil.LastSunday(MovieDateUtil.GameSunday().AddDays(-1))) { Weight = 0 });

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
		void FilterMinerMovies(List<IMiner> minerData)
		{
			DateTime? weekendEnding = minerData[FML_INDEX].Movies?.FirstOrDefault()?.WeekendEnding;

			for (int index = FML_INDEX + 1; index < minerData.Count - 1; index++)
			{
				if (minerData[index].Movies?.FirstOrDefault()?.WeekendEnding != weekendEnding)
				{
					minerData[index].Clear();
				}
			}

			// BO Mojo has a HUGE list of movies that were mined

			var moviesToRemove = new List<IMovie>();
			var lastWeekMovies = minerData.Last().Movies;
			var baseMovies = minerData.First().Movies;

			foreach (var movie in lastWeekMovies)
			{
				var found = baseMovies.FirstOrDefault(item => movie.Equals(item));      // Use the fuzzy logic to match the movie name.

				if (found == null)
				{
					moviesToRemove.Add(movie);
				}
				else
				{
					movie.MovieName = found.MovieName;
				}
			}

			foreach (var movie in moviesToRemove)
			{
				lastWeekMovies.Remove(movie);
			}
		}

		/// <summary>
		/// Mine all of the miner movie data.
		/// </summary>
		/// <param name="miners"></param>
		/// <returns></returns>
		private List<List<IMovie>> MineMiners(IEnumerable<IMiner> miners)
		{
			var result = new List<List<IMovie>>();
			List<IMovie> baseList = null;
			List<IMovie> compoundMovies = null;

			// FML Base list is first.

			baseList = miners.First().Mine();
			result.Add(baseList);

			// TODO: Fix this for the FML base list.  This will break when another compound movie (multi-day) comes into play.

			compoundMovies = baseList.Where(movie => movie.Day.HasValue).ToList();

			Parallel.ForEach(miners, miner =>
			{
				try
				{
					if (miner != baseList)
					{
						var movieList = (miner.OkToMine) ? miner.Mine() : null;

						result.Add(movieList);

						if (movieList != null)
						{
							if (compoundMovies.Any())
							{
								if (miner.Abbreviation == "Todd")
								{
									// Prefer Todd's breakdown if it's available.

									compoundMovies = movieList.Where(movie => movie.Day.HasValue).ToList();
								}

								// TODO: Move this creation outside of the thread so the compound movie list is set.

								if (!movieList.Any(movie => movie.Day.HasValue))
								{
									// The list has no compound movies so they need to be built

									var rootMovie = movieList.FirstOrDefault(movie => movie.Equals(compoundMovies.First()));
									var compoundTotal = compoundMovies.Sum(movie => movie.Earnings);

									if (rootMovie != null)
									{
										movieList.Remove(rootMovie);

										foreach (var movieDay in compoundMovies)
										{
											movieList.Add(new Movie
											{
												Name = movieDay.MovieName,
												Day = movieDay.Day,
												Earnings = movieDay.Earnings / compoundTotal * rootMovie.Earnings,
												WeekendEnding = movieDay.WeekendEnding
											});
										}
									}
								}
							}

							// Assign the id, name, and cost to each movie.

							foreach (var movie in baseList)
							{
								AssignCost(movie, movieList);
							}
						}
					}
				}
				catch (Exception ex)
				{
					result.Add(new List<IMovie>());     // Add a placeholder.

					miner.Error = "Error";

					//Logger.WriteLine($"EXCEPTION: Mining data for {miner.Name} -- {ex.Message}");
				}
			});

			// Mine my movies LAST because they are based on all of the other miners.

			miners.ToList()[1].Mine();

			return result;
		}
	}
}