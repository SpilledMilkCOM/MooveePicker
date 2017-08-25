using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

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
		public void MineAll_BuildMyList()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			Logger.WriteLine($"================== Picking Movies for {nextSunday} ==================\n");

			for (int index = 0; index < miners.Count; index++)
			{
				WriteMoviesAndPicks($"==== {miners[index].Name} ====", minedData[index]);
			}

			Logger.WriteLine(string.Empty);

			WriteMoviesAndPicks("==== Spilled Milk Cinema ====", myList);

			Logger.WriteLine("Upload for FML Analyzer Site");

			foreach (var movie in myList.OrderByDescending(movie => movie.Cost))
			{
				Logger.WriteLine(movie.Earnings.ToString());
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildMyList_NumbersOnly()
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
				rowData.Append($" | {miner.Name}");
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
						rowData.Append($"{movie.Name,-30} | {0m,5} | {movie.Earnings,12:N0}");
					}
					else
					{
						var foundMovie = movieList.FirstOrDefault(item => item.Equals(movie));

						if (foundMovie != null)
						{
							rowData.Append($" | {foundMovie.Earnings,12:N0}");
						}
						else
						{
							rowData.Append($" | {0m,12:N0}");
						}
					}
				}

				var myMovie = myList.FirstOrDefault(item => item.Equals(movie));

				if (myMovie != null)
				{
					rowData.Append($" | {myMovie.Earnings,12:N0}");
				}
				else
				{
					rowData.Append($" | {0m,12:N0}");
				}

				Logger.WriteLine(rowData.ToString());
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareEffeciencies()
		{
			var miners = CreateMiners();
			var minedData = MineMiners(miners);

			// TODO: Should probably connect the mined data to the miner.

			var myList = CreateMyList(minedData, miners);

			var mostEfficient = myList.OrderByDescending(item => item.Efficiency).First();
			int index = 1;

			Logger.WriteLine("\nEffeciency Differences\n");

			foreach (var movie in myList.OrderBy(item => mostEfficient.Efficiency * item.Cost - item.Earnings))
			{
				Logger.WriteLine($"{index}. {movie.Name,-30} -- {movie.Efficiency / 1000:F3} [${movie.Earnings:N2}] "
								 + $"==> **${mostEfficient.Efficiency * movie.Cost:N2} "
								 + $"++${mostEfficient.Efficiency * movie.Cost - movie.Earnings:N2}  {(mostEfficient.Efficiency * movie.Cost - movie.Earnings) / movie.Earnings * 100:F2}%");
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

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AggregateNames(Dictionary<string, int> counts, List<IMovie> movies, string name)
		{
			foreach (var movie in movies)
			{
				// Don't use contains key because we're testing the Equals() method.
				var foundKey = counts.Keys.FirstOrDefault(item => (new Movie { Name = item }).Equals(movie));

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
			var found = movies.FirstOrDefault(item => item.Name.Equals(movie.Name));

			if (found != null)
			{
				found.Id = movie.Id;
				found.Name = movie.Name;        // So the names aren't fuzzy anymore.
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
				new MineNerd(),
				new MineToddThatcher(),
				new MineBoxOfficePro(),
				new MineCulturedVultures()
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
				movie.Id = id;

				for (int index = 0; index < movieData.Count; index++)
				{
					if (index != NERD_INDEX)
					{
						AssignCost(movie, movieData[index]);
					}
				}

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
			int nerdWeight = 4;
			int toddWeight = 6;
			int boProWeight = 8;
			int totalWeight = nerdWeight;

			var result = new Movie
			{
				Id = baseMovie.Id,
				Name = baseMovie.Name,
				Cost = baseMovie.Cost,
				Earnings = baseMovie.Earnings * nerdWeight,
				WeekendEnding = baseMovie.WeekendEnding
			};

			for (int index = 0; index < movieData.Count; index++)
			{
				if (index != NERD_INDEX)
				{
					var foundMovie = movieData[index].FirstOrDefault(item => item.Name.Equals(baseMovie.Name) && item.WeekendEnding == result.WeekendEnding);

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

		/// <summary>
		/// Mine all of the miner movie data.
		/// </summary>
		/// <param name="miners"></param>
		/// <returns></returns>
		private List<List<IMovie>> MineMiners(IEnumerable<IMiner> miners)
		{
			var result = new List<List<IMovie>>();

			foreach (var miner in miners)
			{
				result.Add(miner.Mine());
			}

			return result;
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
	}
}