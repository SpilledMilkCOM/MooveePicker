using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using SM.Common.Interfaces;
using SM.Common.Tests;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBoxOfficeMojoTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_Mine()
		{
			var test = new MineBoxOfficeMojo();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine(test.UrlSource);
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_Mine_PreviousWeek()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now);
			var test = new MineBoxOfficeMojo(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_Mine_Previous2Weeks()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now).AddDays(-7);
			var test = new MineBoxOfficeMojo(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_Mine_Previous3Weeks()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now).AddDays(-14);
			var test = new MineBoxOfficeMojo(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_MineWeekend_ThisWeek()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now);
			var test = new MineBoxOfficeMojo(weekendEnding);

			var actual = test.MineWeekend(weekendEnding);

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			Logger.WriteLine(test.UrlSource);
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojo_MineWeekend_ThisWeek_Filtered()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now);
			var test = new MineBoxOfficeMojo(weekendEnding);

			var actual = test.MineWeekend(weekendEnding);

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			actual = FilterMovies(actual);

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));

			foreach (var item in actual.OrderByDescending(item => item.Cost))
			{
				//Logger.WriteLine($"{item.Name} {item.Cost}  {item.EarningsBase}");
				Logger.WriteLine($"{item.EarningsBase}");
			}
		}

		//----==== PRIVATE ====--------------------------------------------------------------------------

		private List<IMovie> FilterMovies(List<IMovie> toFilter)
		{
			var fmlMiner = new MineFantasyMovieLeagueBoxOffice();
			var fmlMovies = fmlMiner.Mine();
			var result = new List<IMovie>();

			foreach (var movie in toFilter)
			{
				var fmlMovie = fmlMovies.FirstOrDefault(item => item.Equals(movie));

				if (fmlMovie != null)
				{
					movie.Cost = fmlMovie.Cost;
					result.Add(movie);
				}
			}

			return result;
		}
	}
}