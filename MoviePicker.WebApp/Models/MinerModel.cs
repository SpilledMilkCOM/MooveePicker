﻿using System.Collections.Generic;
using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.Common.Interfaces;
using System.Linq;
using MoviePicker.Common;
using System;

namespace MoviePicker.WebApp.Models
{
	public class MinerModel : IMinerModel
	{
		public MinerModel()
		{
			Miners = CreateMinersWithData();
		}

		public List<IMiner> Miners { get; set; }

		public List<IMiner> CreateMinersWithData()
		{
			var miners = CreateMiners();

			return null;
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
			return new List<IMiner> {
				new MineNerd { Weight = 1 },
				new MineToddThatcher { Weight = 3 },
				new MineBoxOfficePro { Weight = 4 },
				new MineBoxOfficeMojo { Weight = 1 },
				new MineCulturedVultures { Weight = 2 },
				new MineBoxOfficeProphet { Weight = 2 }
			};
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

			foreach (var miner in miners)
			{
				try
				{
					if (nerdList == null)
					{
						// Nerd list is first.

						nerdList = miner.Mine();
						result.Add(nerdList);

						compoundMovies = nerdList.Where(movie => movie.Day.HasValue).ToList();
					}
					else
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
			}

			return result;
		}
	}
}