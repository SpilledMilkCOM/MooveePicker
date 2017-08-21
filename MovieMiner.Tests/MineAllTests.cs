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

			// FML Nerd (Pete) should have all of the movies WITH the Bux

			Logger.WriteLine("==== FML Nerd (Pete) ====");
			WriteMovies(nerds);

			Logger.WriteLine("==== Todd M Thatcher ====");
			WriteMovies(todds);

			Logger.WriteLine("==== Box Office Pros ====");
			WriteMovies(boPros);
		}
	}
}