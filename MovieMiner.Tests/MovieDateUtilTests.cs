using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MovieDateUtilTests : MineTestBase
	{
		private static List<string> _seasons;

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();

			_seasons = new List<string> { "Spring", "Summer", "Fall", "Winter" };
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DateToSeason_StartOfSpring()
		{
			Assert.AreEqual("Spring", MovieDateUtil.DateToSeason(new DateTime(2017, 3, 5)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DateToSeason_StartOfSummer()
		{
			Assert.AreEqual("Summer", MovieDateUtil.DateToSeason(new DateTime(2017, 6, 4)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DisplayCurrentSeason()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			Logger.WriteLine($"Year: {nextSunday.Year}");
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DisplayFirstWeekendOfYear()
		{
			var nextSunday = MovieDateUtil.NextSunday(new DateTime(DateTime.Now.Year, 1, 1));

			Logger.WriteLine($"First weekend: {nextSunday}");
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DisplaySeasons()
		{
			// TODO: Wrap this date logic into class.

			// Assuming each "season" is 13 weeks.  There is some sort of offset.
			var firstWeekend = MovieDateUtil.StartOfSeason;

			for (int index = 0; index < 4; index++)
			{
				Logger.WriteLine($"Start of {_seasons[index]} Season: {firstWeekend.AddDays(7 * 13 * index)}");
			}
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_LastSunday_IsASunday()
		{
			var actual = MovieDateUtil.LastSunday();

			Assert.AreEqual(DayOfWeek.Sunday, actual.DayOfWeek);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_LastSunday_GivenDate()
		{
			var actual = MovieDateUtil.LastSunday(new DateTime(2017, 8, 23));

			Assert.AreEqual(new DateTime(2017, 8, 20), actual);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_NextSunday_IsASunday()
		{
			var actual = MovieDateUtil.NextSunday();

			Assert.AreEqual(DayOfWeek.Sunday, actual.DayOfWeek);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_NextSunday_GivenDate()
		{
			var actual = MovieDateUtil.NextSunday(new DateTime(2017, 8, 23));

			Assert.AreEqual(new DateTime(2017, 8, 27), actual);
		}
	}
}