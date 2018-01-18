using Microsoft.VisualStudio.TestTools.UnitTesting;
using MooveePicker;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineAllSimulationTests : MineTestBase
	{
		private const int NERD_INDEX = 0;

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<IMovie, Movie>();
			_unity.RegisterType<IMovieList, MovieList>();
			_unity.RegisterType<IMoviePicker, MoviePickerVariants>();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAllSimulation_BuildAllLists_WithPicks()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			FilterMiners(minedData);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			Logger.WriteLine($"================== Picking Movies for {nextSunday} ==================\n");

			for (int index = 0; index < miners.Count; index++)
			{
				if (minedData[index] != null
				&& minedData[index].Any()
				&& minedData[index][0].WeekendEnding == nextSunday
				&& miners[index].Weight > 0)
				{
					// Only show data that will be used.

					WriteMoviesAndPicks($"==== {miners[index].Name} ====", minedData[index]);
				}
			}

			Logger.WriteLine(string.Empty);

			WriteMoviesAndPicks("==== Spilled Milk Cinema ====", myList);

			WriteFMLNerdLink(myList);
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AggregateNames(Dictionary<string, int> counts, List<IMovie> movies, string name)
		{
			foreach (var movie in movies)
			{
				var foundKey = counts.Keys.FirstOrDefault(item => item == movie.Name);

				if (foundKey != null)
				{
					counts[foundKey] = counts[foundKey] + 1;
				}
				else
				{
					counts.Add(movie.Name, 1);
					Logger.WriteLine($"{name} movie {movie.Name} not found.");
				}
			}
		}

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

		private void ChangeMovieEarnings(List<IMovie> list, string name, decimal earnings)
		{
			var found = list.FirstOrDefault(movie => movie.Name == name);

			if (found != null)
			{
				found.Earnings = earnings;
			}
		}

		private void ChangeMovieName(List<IMovie> list, string fromName, string toName)
		{
			var found = list.FirstOrDefault(movie => movie.Name == fromName);

			if (found != null)
			{
				found.Name = toName;
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
				new MineBoxOfficeMojo { Weight = 3},
				new MineCulturedVultures { Weight = 2 },
				new MineBoxOfficeProphet { Weight = 2 }
			};
		}

		private List<IMovie> CreateMyList(List<List<IMovie>> movieData, List<IMiner> miners)
		{
			var myList = new List<IMovie>();

			// FML Nerd (Pete) should have all of the movies WITH the Bux
			// Use the nerd data to copy the bux to each list.

			var nerds = movieData[NERD_INDEX];

			int id = 1;

			foreach (var movie in nerds.OrderByDescending(item => item.Cost))
			{
				// My list is based on how well I trust these sources.

				myList.Add(CreateMyMovie(movie, movieData, miners));

				id++;
			}

			return myList;
		}

		/// <summary>
		/// Create a single movie with the Earnings calculated from the mined data.
		/// </summary>
		/// <param name="baseMovie">The movie being matched (from FML Nerd)</param>
		/// <param name="movieData">All of the movie lists.</param>
		/// <param name="miners">All of the movie miners.</param>
		/// <returns></returns>
		private IMovie CreateMyMovie(IMovie baseMovie, List<List<IMovie>> movieData, List<IMiner> miners)
		{
			var totalWeight = miners[NERD_INDEX].Weight;

			var result = new Movie
			{
				Id = baseMovie.Id,
				MovieName = baseMovie.MovieName,
				Day = baseMovie.Day,
				Cost = baseMovie.Cost,
				Earnings = baseMovie.Earnings * miners[NERD_INDEX].Weight,
				WeekendEnding = baseMovie.WeekendEnding
			};

			for (int index = 0; index < movieData.Count; index++)
			{
				if (index != NERD_INDEX)
				{
					var foundMovie = movieData[index]?.FirstOrDefault(item => item.Equals(baseMovie) && item.WeekendEnding == result.WeekendEnding);

					if (foundMovie != null)
					{
						result.Earnings += foundMovie.Earnings * miners[index].Weight;
						totalWeight += miners[index].Weight;
					}
				}
			}

			result.Earnings /= totalWeight;     // Weighted average.

			return result;
		}

		void FilterMiners(List<List<IMovie>> minerData)
		{
			DateTime? weekendEnding = minerData[NERD_INDEX].FirstOrDefault()?.WeekendEnding;

			for (int index = NERD_INDEX + 1; index < minerData.Count; index++)
			{
				if (minerData[index].FirstOrDefault()?.WeekendEnding != weekendEnding)
				{
					minerData[index] = null;
				}
			}
		}

		private IMiner FindMiner<TMiner>(List<IMiner> miners)
		{
			return miners.FirstOrDefault(miner => miner.GetType() == typeof(TMiner));
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

					Logger.WriteLine($"EXCEPTION: Mining data for {miner.Name} -- {ex.Message}");
				}
			}

			return result;
		}

		private void WriteFMLNerdLink(IEnumerable<IMovie> list)
		{
			string url = "http://analyzer.fmlnerd.com/lineups/?ests=";
			string movieList = null;

			Logger.WriteLine("\nUpload for FML Analyzer Site");

			foreach (var movie in list.OrderByDescending(movie => movie.Cost))
			{
				if (movieList != null)
				{
					movieList += ",";
				}

				movieList += ((int)movie.Earnings).ToString();
			}

			Logger.WriteLine(url + movieList);
		}

		private void WriteMoviesAndPicks(string header, List<IMovie> movies)
		{
			Logger.WriteLine($"\n{header}\n");
			WriteMovies(movies);

			var test = ConstructTestObject();

			test.AddMovies(movies);

			var best = test.ChooseBest();

			Logger.WriteLine(string.Empty);
			WritePicker(test);
			WriteMovies(best);

			Logger.WriteLine("\n==== Best Performer Disabled ====\n");

			test.EnableBestPerformer = false;

			best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		private void UpdateWithWeekendValues(List<IMovie> myList)
		{
			ChangeMovieEarnings(myList, "Dunkirk", 1900000);
			ChangeMovieEarnings(myList, "It [Friday]", 51000000);
		}
	}
}