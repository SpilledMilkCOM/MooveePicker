using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineAllTests : MineTestBase
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
			_unity.RegisterType<IMoviePicker, MoviePicker.Msf.MsfMovieSolver>();
			//_unity.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_Actuals_WithBestPick()
		{
			var lastSunday = MovieDateUtil.LastSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			Logger.WriteLine($"================== Picking Movies for {lastSunday} ==================\n");

			for (int index = 0; index < miners.Count; index++)
			{
				if (minedData[index] != null
				&& minedData[index].Any()
				&& minedData[index][0].WeekendEnding == lastSunday
				&& miners[index].Weight > 0)
				{
					// Only show data that will be used.

					WriteMoviesAndPicks($"==== {miners[index].Name} ====", minedData[index]);
				}
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_EstimatesOnly_ForHistory()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			Logger.WriteLine($"================== All Estimates for {nextSunday} ==================\n");

			// Build rows of movies, each column is a miner's mined value.

			var nerdsMovies = minedData[NERD_INDEX];
			var rowData = new StringBuilder();

			rowData.Append("Title                          |  Actuals");

			foreach (var miner in miners)
			{
				rowData.Append($" | {miner.Abbreviation}");
			}

			rowData.Append($" | SM Cinema");

			Logger.WriteLine(rowData.ToString());

			foreach (var movie in nerdsMovies)
			{
				rowData.Clear();

				foreach (var movieList in minedData)
				{
					if (movieList == nerdsMovies)
					{
						rowData.Append($"{movie.Name,-30} | {0m,5} | {movie.Earnings / 1000000,5:F3}");
					}
					else
					{
						var foundMovie = movieList.FirstOrDefault(item => item.Equals(movie));

						if (foundMovie != null)
						{
							rowData.Append($" | {foundMovie.Earnings / 1000000,5:F3}");
						}
						else
						{
							rowData.Append(" | ");
						}
					}
				}

				var myMovie = myList.FirstOrDefault(item => item.Equals(movie));

				if (myMovie != null)
				{
					rowData.Append($" | {myMovie.Earnings / 1000000,5:F5}");
				}
				else
				{
					rowData.Append(" | ");
				}

				Logger.WriteLine(rowData.ToString());
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_WithPicks()
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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_WithPicks_TrackDuringWeekend()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			// Adjust based on known values...

			UpdateWithWeekendValues(myList);

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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_WithPicks_NoCultVult_NoBOProphet()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			FindMiner<MineBoxOfficeProphet>(miners).Weight = 0;
			FindMiner<MineCulturedVultures>(miners).Weight = 0;

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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_CompareAllNumbers()
		{
			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			FilterMiners(minedData);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			var header = "\n\n__Name____________________________SM Cine";

			for (int index = 0; index < miners.Count; index++)
			{
				header += $"______{miners[index].Abbreviation}*{miners[index].Weight}";
			}

			Logger.WriteLine(header);

			foreach (var movie in myList)
			{
				string isBestPerformer = movie.IsBestPerformer ? "*" : string.Empty;
				string row = $"{movie.Name + isBestPerformer,-30}  {movie.Earnings,11:N0}";

				for (int index = 0; index < miners.Count; index++)
				{
					var foundMovie = minedData[index]?.FirstOrDefault(item => item.Equals(movie));

					if (foundMovie != null)
					{
						row += $" | {foundMovie.Earnings,11:N0}";
					}
					else
					{
						row += " |  ----------";
					}
				}

				Logger.WriteLine(row);
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_ComparePickNumbers()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);
			var picker = ConstructTestObject();

			picker.AddMovies(myList);

			var best = picker.ChooseBest();

			var header = "\n__Name____________________________SM Cine";

			for (int index = 0; index < miners.Count; index++)
			{
				header += $"______{miners[index].Abbreviation}*{miners[index].Weight}";
			}

			Logger.WriteLine(header);

			foreach (var movie in best.Movies)
			{
				string isBestPerformer = movie.IsBestPerformer ? "*" : string.Empty;
				string row = $"{movie.Name + isBestPerformer,-30}  {movie.Earnings,11:N0}";

				for (int index = 0; index < miners.Count; index++)
				{
					var foundMovie = minedData[index].FirstOrDefault(item => item.Equals(movie));

					if (foundMovie != null)
					{
						row += $" | {foundMovie.Earnings,11:N0}";
					}
					else
					{
						row += " |  --------";
					}
				}

				Logger.WriteLine(row);
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildAllLists_WithPicks_SolomonsTest()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// Adjust your weights HERE!!

			FindMiner<MineBoxOfficeProphet>(miners).Weight = 0;
			FindMiner<MineCulturedVultures>(miners).Weight = 0;

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

			Logger.WriteLine("\nUpload for FML Analyzer Site");

			foreach (var movie in myList.OrderByDescending(movie => movie.Cost))
			{
				Logger.WriteLine(movie.Earnings.ToString());
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareEfficiencies()
		{
			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			var mostEfficient = myList.OrderByDescending(item => item.Efficiency).First();
			int index = 1;

			Logger.WriteLine("\nEfficiency Differences\n");

			Logger.WriteLine("____Title______________________________Box_Office________Efficiency________New_Box_Office____Difference_____Pct__");

			foreach (var movie in myList.OrderBy(item => mostEfficient.Efficiency * item.Cost - item.Earnings))
			{
				Logger.WriteLine($"{index,2}. {movie.Name,-30} --  ${movie.Earnings,13:N2}  [${movie.Efficiency:N2}]"
								 + $"==> **${mostEfficient.Efficiency * movie.Cost,13:N2} "
								 + $"++ ${mostEfficient.Efficiency * movie.Cost - movie.Earnings,13:N2}  {(mostEfficient.Efficiency * movie.Cost - movie.Earnings) / movie.Earnings * 100,6:F2}%");
				index++;
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareEfficiencies_TrackingDuringWeekend()
		{
			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			UpdateWithWeekendValues(myList);

			var mostEfficient = myList.OrderByDescending(item => item.Efficiency).First();
			int index = 1;

			Logger.WriteLine("\nEfficiency Differences\n");

			Logger.WriteLine("____Title______________________________Box_Office________Efficiency________New_Box_Office____Difference_____Pct__");

			foreach (var movie in myList.OrderBy(item => mostEfficient.Efficiency * item.Cost - item.Earnings))
			{
				Logger.WriteLine($"{index,2}. {movie.Name,-30} --  ${movie.Earnings,13:N2}  [${movie.Efficiency:N2}]"
								 + $"==> **${mostEfficient.Efficiency * movie.Cost,13:N2} "
								 + $"++ ${mostEfficient.Efficiency * movie.Cost - movie.Earnings,13:N2}  {(mostEfficient.Efficiency * movie.Cost - movie.Earnings) / movie.Earnings * 100,6:F2}%");
				index++;
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareEfficiencies_NoCultVult_NoBOProphet()
		{
			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			FindMiner<MineBoxOfficeProphet>(miners).Weight = 0;
			FindMiner<MineCulturedVultures>(miners).Weight = 0;

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			var mostEfficient = myList.OrderByDescending(item => item.Efficiency).First();
			int index = 1;

			Logger.WriteLine("\nEfficiency Differences\n");

			Logger.WriteLine($"____Title______________________________Box_Office_____Efficiency___________New_Box_Office____Difference_____Pct__");

			foreach (var movie in myList.OrderBy(item => mostEfficient.Efficiency * item.Cost - item.Earnings))
			{
				Logger.WriteLine($"{index,2}. {movie.Name,-30} --  ${movie.Earnings:N2}  [${movie.Efficiency:N2}]"
								 + $"==> **${mostEfficient.Efficiency * movie.Cost,14:N2} "
								 + $"++ ${mostEfficient.Efficiency * movie.Cost - movie.Earnings,12:N2}  {(mostEfficient.Efficiency * movie.Cost - movie.Earnings) / movie.Earnings * 100,6:F2}%");
				index++;
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareMovieNames()
		{
			// This test is to verify that the data is synchronized with the analyzer.

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			var nerds = minedData[NERD_INDEX];

			// TODO: Could try to use Linq to JOIN these lists to find common movie names.

			// Verify movie counts.

			var counts = new Dictionary<string, int>();

			// Add nerds movies.

			nerds.ForEach(movie => counts.Add(movie.Name, 1));

			for (int index = 0; index < miners.Count; index++)
			{
				if (index != NERD_INDEX)
				{
					AggregateNames(counts, minedData[index], miners[index].Name);
				}
			}

			Logger.WriteLine(string.Empty);

			var orderedCounts = counts.OrderByDescending(movie => movie.Value).ThenBy(movie => movie.Key);

			foreach (var pair in orderedCounts)
			{
				Logger.WriteLine($"{pair.Key} -- {pair.Value}");
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareMovieNames_SameDate()
		{
			// This test is to verify that the data is synchronized with the analyzer.

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			var nerds = minedData[NERD_INDEX];
			var restrictDate = nerds[0].WeekendEnding;

			// TODO: Could try to use Linq to JOIN these lists to find common movie names.

			// Verify movie counts.

			var counts = new Dictionary<string, int>();

			// Add nerds movies.

			nerds.ForEach(movie => counts.Add(movie.Name, 1));

			for (int index = 0; index < miners.Count; index++)
			{
				if (index != NERD_INDEX)
				{
					if (minedData[index] != null && minedData[index].Any())
					{
						if (minedData[index][0].WeekendEnding.Date == restrictDate.Date)
						{
							AggregateNames(counts, minedData[index], miners[index].Name);
						}
						else
						{
							Logger.WriteLine($"\nOmitted \"{miners[index].Name}\" due to mismatched date. [{minedData[index][0].WeekendEnding.Date.ToString("d")}]");
						}
					}
					else
					{
						Logger.WriteLine($"\nOmitted \"{miners[index].Name}\" due to empty list.");
					}
				}
			}

			Logger.WriteLine(string.Empty);
			Logger.WriteLine(miners[NERD_INDEX].Movies.First().WeekendEnding.ToShortDateString());

			var orderedCounts = counts.OrderByDescending(movie => movie.Value).ThenBy(movie => movie.Key);

			foreach (var pair in orderedCounts)
			{
				Logger.WriteLine($"{pair.Key} -- {pair.Value}");
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineAll_FML_PerfectPick()
		{
			var test = new MineFantasyMovieLeagueBoxOffice();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMoviesAndPicks($"==== {test.Name} ====", actual);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_MojoHistory()
		{
			var nerdMiner = new MineNerd();
			var nerdMovies = nerdMiner.Mine();
			var referenceDate = nerdMovies[0].WeekendEnding.AddDays(-7);
			var movieHistory = new Dictionary<int, List<IMovie>>();
			bool foundHistory = false;
			bool firstPass = true;

			do
			{
				var mojoMiner = new MineBoxOfficeMojo(referenceDate);
				var mojoMovies = mojoMiner.Mine();

				foundHistory = false;

				foreach (var movie in nerdMovies)
				{
					IMovie foundMovie = (firstPass) ? mojoMovies.FirstOrDefault(item => item.Equals(movie))     // Use fuzzy search on first pass.
													: mojoMovies.FirstOrDefault(item => item.Name == movie.Name);

					if (foundMovie != null)
					{
						if (movieHistory.ContainsKey(movie.Id))
						{
							movieHistory[movie.Id].Add(foundMovie);
						}
						else
						{
							movieHistory.Add(movie.Id, new List<IMovie> { foundMovie });
						}

						foundHistory = true;

						if (firstPass)
						{
							// Use Mojo's name (hopefully) they don't change mid-stream.
							movie.Name = foundMovie.Name;
						}
					}
				}

				firstPass = false;

				referenceDate = referenceDate.AddDays(-7);

			} while (foundHistory);

			foreach (var movie in nerdMovies)
			{
				List<IMovie> movies = movieHistory.ContainsKey(movie.Id) ? movieHistory[movie.Id] : null;
				string history = null;

				if (movies != null)
				{
					foreach (var hist in movies)
					{
						if (history != null)
						{
							history += ", ";
						}

						history += hist.Earnings.ToString("N0");
					}
				}

				Logger.WriteLine($"{movie.Name,-30} {history}");
			}

			foreach (var movie in nerdMovies)
			{
				List<IMovie> movies = movieHistory.ContainsKey(movie.Id) ? movieHistory[movie.Id] : null;
				string history = null;

				if (movies != null)
				{
					foreach (var hist in movies)
					{
						if (history != null)
						{
							history += ", ";
						}

						history += hist.Earnings.ToString("N0");
					}
				}

				Logger.WriteLine($"{movie.Name,-30} {history}");
			}
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