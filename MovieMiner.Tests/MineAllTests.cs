using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
			IMiner test = new MineToddThatcher();
			var nextSunday = MovieDateUtil.NextSunday();

			var todds = test.Mine();

			test = new MineNerd();

			var nerds = test.Mine();

			test = new MineBoxOfficePro();

			var boPros = test.Mine();

			var myList = new List<IMovie>();

			// FML Nerd (Pete) should have all of the movies WITH the Bux
			// Use the nerd data to copy the bux to each list.

			int id = 1;

			foreach (var movie in nerds.OrderByDescending(item => item.Cost))
			{
				movie.Id = id;

				AssignCost(movie, todds);
				AssignCost(movie, boPros);

				// My list is based on how well I trust these sources.

				myList.Add(CreateMyMovie(movie, todds, boPros));

				id++;
			}

			Logger.WriteLine($"================== Picking Movies for {nextSunday} ==================\n");

			WriteMoviesAndPicks("==== FML Nerd (Pete) ====", nerds);
			WriteMoviesAndPicks("==== Todd M Thatcher ====", todds);
			WriteMoviesAndPicks("==== Box Office Pros ====", boPros);
			WriteMoviesAndPicks("==== Spilled Milk Cinema ====", myList);

			Logger.WriteLine("Upload for FML Analyzer Site");

			foreach (var movie in myList.OrderByDescending(movie => movie.Cost))
			{
				Logger.WriteLine(movie.Earnings.ToString());
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareMovieNames()
		{
			// This test is to verify that the data is synchronized with the analyzer.

			IMiner test = new MineNerd();

			var nerds = test.Mine();

			// TODO: Could try to use Linq to JOIN these lists to find common movie names.

			// Verify movie counts.

			var counts = new Dictionary<string, int>();

			// Add nerds movies.

			nerds.ForEach(movie => counts.Add(movie.Name, 1));

			AggregateNames(counts, new MineToddThatcher().Mine(), "Todd M Thatcher");
			AggregateNames(counts, new MineBoxOfficePro().Mine(), "Box Office Pro");
			//AggregateNames(counts, new MineCulturedVultures().Mine(), "Cultured Vultures");

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

		private IMovie CreateMyMovie(IMovie nerdMovie, List<IMovie> todds, List<IMovie> boPros)
		{
			int nerdWeight = 4;
			int toddWeight = 6;
			int boProWeight = 8;
			int totalWeight = nerdWeight;
			var toddMovie = todds.FirstOrDefault(item => item.Name.Equals(nerdMovie.Name));
			var boProMovie = boPros.FirstOrDefault(item => item.Name.Equals(nerdMovie.Name));

			var result = new Movie
			{
				Id = nerdMovie.Id,
				Name = nerdMovie.Name,
				Cost = nerdMovie.Cost,
				Earnings = nerdMovie.Earnings * nerdWeight,
				WeekendEnding = nerdMovie.WeekendEnding
			};

			if (toddMovie != null && toddMovie.WeekendEnding == result.WeekendEnding)
			{
				result.Earnings += toddMovie.Earnings * toddWeight;
				totalWeight += toddWeight;
			}

			if (boProMovie != null && boProMovie.WeekendEnding == result.WeekendEnding)
			{
				result.Earnings += boProMovie.Earnings * boProWeight;
				totalWeight += boProWeight;
			}

			result.Earnings /= totalWeight;     // Weighted average.

			return result;
		}

		private void WriteMoviesAndPicks(string header, List<IMovie> movies)
		{
			Logger.WriteLine($"\n{header}\n");
			WriteMovies(movies);

			var test = ConstructTestObject();

			test.AddMovies(movies);

			var best = test.ChooseBest();

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