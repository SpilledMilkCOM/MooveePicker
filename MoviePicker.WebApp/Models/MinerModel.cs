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
		private const int NERD_INDEX = 0;

		public MinerModel()
		{
			Miners = CreateMinersWithData();
		}

		public List<IMiner> Miners { get; private set; }

		public List<IMiner> CreateMinersWithData()
		{
			var miners = CreateMiners();

			MineMiners(miners);

			FilterMinerMovies(miners);

			return miners;
		}

		public List<IMovie> CreateWeightedList()
		{
			var result = new List<IMovie>();

			// FML Nerd (Pete) should have all of the movies WITH the Bux
			// Use the nerd data to copy the bux to each list.

			var nerdMovies = Miners[NERD_INDEX].Movies;

			foreach (var movie in nerdMovies.OrderByDescending(item => item.Cost))
			{
				// My list is based on how well I trust these sources.

				result.Add(CreateWeightedMovie(movie));
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
				new MineNerd { Weight = 1 },
				new MineToddThatcher { Weight = 3 },
				new MineBoxOfficePro { Weight = 4 },
				new MineBoxOfficeMojo { Weight = 3 },
				new MineCulturedVultures { Weight = 2 },
				new MineBoxOfficeProphet { Weight = 2 },
				new MineBoxOfficeReport { Weight = 3 }
			};

			// Grab last weeks results for comparisons.

			result.Add(new MineBoxOfficeMojo(MovieDateUtil.LastSunday()) { Weight = 0 });

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
			decimal totalWeight = Miners[NERD_INDEX].Weight;

			var result = new Movie
			{
				Id = baseMovie.Id,
				MovieName = baseMovie.MovieName,
				Day = baseMovie.Day,
				Cost = baseMovie.Cost,
				Earnings = baseMovie.Earnings * totalWeight,
				WeekendEnding = baseMovie.WeekendEnding
			};

			for (int index = 0; index < Miners.Count; index++)
			{
				if (index != NERD_INDEX)
				{
					var foundMovie = Miners[index]?.Movies?.FirstOrDefault(item => item.Equals(baseMovie) && item.WeekendEnding == result.WeekendEnding);

					if (foundMovie != null)
					{
						result.Earnings += foundMovie.Earnings * Miners[index].Weight;
						totalWeight += Miners[index].Weight;
					}
				}
			}

			result.Earnings /= totalWeight;     // Weighted average.

			return result;
		}

		/// <summary>
		/// Filter out the data that does not match the base date.
		/// </summary>
		/// <param name="minerData"></param>
		void FilterMinerMovies(List<IMiner> minerData)
		{
			DateTime? weekendEnding = minerData[NERD_INDEX].Movies?.FirstOrDefault()?.WeekendEnding;

			for (int index = NERD_INDEX + 1; index < minerData.Count; index++)
			{
				if (minerData[index].Movies?.FirstOrDefault()?.WeekendEnding != weekendEnding)
				{
					minerData[index].Clear();
				}
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
			List<IMovie> nerdList = null;
			List<IMovie> compoundMovies = null;

			// Nerd list is first.

			nerdList = miners.First().Mine();
			result.Add(nerdList);

			compoundMovies = nerdList.Where(movie => movie.Day.HasValue).ToList();

			//TODO: Thread these calls out...

			Parallel.ForEach(miners, miner =>
			{
				try
				{
					if (miner != nerdList)
					{
						var movieList = (miner.Weight > 0) ? miner.Mine() : null;

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

							foreach (var movie in nerdList)
							{
								AssignCost(movie, movieList);
							}
						}
					}
				}
				catch (Exception ex)
				{
					result.Add(new List<IMovie>());     // Add a placeholder.

					//Logger.WriteLine($"EXCEPTION: Mining data for {miner.Name} -- {ex.Message}");
				}
			});

			return result;
		}
	}
}