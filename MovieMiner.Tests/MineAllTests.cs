using System;
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
		private const string PRIMARY_TEST_CATEGORY = "Mining";

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_CompareMovieNames()
		{
			IMiner test = new MineToddThatcher();

			var todds = test.Mine();

			test = new MineNerd();

			var nerds = test.Mine();

			test = new MineBoxOfficePro();

			var boPros = test.Mine();

			// TODO: Could try to use Linq to JOIN these lists to find common movie names.

			// Verify movie counts.

			var counts = new Dictionary<string, int>();

			// Add nerds movies.

			nerds.ForEach(movie => counts.Add(movie.Name, 1));

			foreach (var movie in todds)
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
				}
			}

			foreach (var movie in boPros)
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
				}
			}

			var orderedCounts = from pair in counts orderby pair.Key select pair;

			foreach (var pair in orderedCounts)
			{
				Logger.WriteLine($"{pair.Key} -- {pair.Value}");
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineAll_BuildMyList()
		{
			IMiner test = new MineToddThatcher();

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

			Logger.WriteLine("\n==== FML Nerd (Pete) ====\n");
			WriteMovies(nerds);

			Logger.WriteLine("\n==== Todd M Thatcher ====\n");
			WriteMovies(todds);

			Logger.WriteLine("\n==== Box Office Pros ====\n");
			WriteMovies(boPros);

			Logger.WriteLine("\n==== Spilled Milk Cinema ====\n");
			WriteMovies(myList);
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
				Earnings = nerdMovie.Earnings * nerdWeight
			};

			if (toddMovie != null)
			{
				result.Earnings += toddMovie.Earnings * toddWeight;
				totalWeight += toddWeight;
			}

			if (boProMovie != null)
			{
				result.Earnings += boProMovie.Earnings * boProWeight;
				totalWeight += boProWeight;
			}

			result.Earnings /= totalWeight;		// Weighted average.

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

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
	}
}