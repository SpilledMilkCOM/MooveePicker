using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MovieDateUtilTests : MineTestBase
	{
		protected new const string PRIMARY_TEST_CATEGORY = "Mock";

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
		public void MovieDateUtil_DateToWeek_StartOfSummer()
		{
			Assert.AreEqual(1, MovieDateUtil.DateToWeek(new DateTime(2017, 6, 4)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DateToWeek_StartOfSummerPlusTwoWeeks()
		{
			Assert.AreEqual(3, MovieDateUtil.DateToWeek(new DateTime(2017, 6, 4).AddDays(14)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DateToWeek_StartOfFall()
		{
			Assert.AreEqual(1, MovieDateUtil.DateToWeek(new DateTime(2017, 9, 3)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_DisplayCurrentSeason()
		{
			var nextSunday = MovieDateUtil.NextSunday();

			Logger.WriteLine($"Year: {nextSunday.Year}  Season:  {MovieDateUtil.DateToSeason(nextSunday)}  Week:  {MovieDateUtil.DateToWeek(nextSunday)}");
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
		public void MovieDateUtil_GameSunday_IsASunday()
		{
			var actual = MovieDateUtil.GameSunday();

			Assert.AreEqual(DayOfWeek.Sunday, actual.DayOfWeek);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MovieDateUtil_GameSunday_OnMonday_IsASunday()
		{
			var actual = MovieDateUtil.GameSunday(new DateTime(2018, 9, 10), true);

			Assert.AreEqual(DayOfWeek.Sunday, actual.DayOfWeek);
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