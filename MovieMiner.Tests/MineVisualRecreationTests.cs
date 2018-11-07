using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineVisualRecreationTests : MineTestBase
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
		public void MineVisualRecreation_Mine_GenerateBoxOfficeValues()
		{
			// http://pro.boxoffice.com/

			var test = new MineFantasyMovieLeagueBoxOffice();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			var weekendEnding = actual[0].WeekendEnding;
			var tab = "\t";

			Logger.WriteLine($"{tab}{tab}{tab}var weekend = new DateTime({weekendEnding.Year}, {weekendEnding.Month}, {weekendEnding.Day});");
			Logger.WriteLine($"{tab}{tab}{tab}UrlSource = \"{test.UrlSource}\";");
			Logger.WriteLine($"{tab}{tab}{tab}return new List<IMovie>");
			Logger.WriteLine($"{tab}{tab}{tab}{tab}{tab}{tab}{{");

			foreach (var movie in actual.OrderByDescending(item => item.Cost))
			{
				Logger.WriteLine($"{tab}{tab}{tab}{tab}{tab}{tab}{tab}{tab}new Movie {{ MovieName = \"{movie.MovieName}\", Earnings = 0, WeekendEnding = weekend }},");
			}
			Logger.WriteLine($"{tab}{tab}{tab}{tab}{tab}{tab}}};");
		}
	}
}