﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Common.Interfaces;
using SM.Common.Tests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBoxOfficeMojoTheaterCountTests : MineTestBase
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
		public void MineBoxOfficeMojoTheaterCount_Mine()
		{
			var test = new MineBoxOfficeMojoTheaterCount();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMovies(actual.OrderByDescending(item => item.TheaterCount));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojoTheaterCount_Mine_PreviousWeek()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now);
			var test = new MineBoxOfficeMojoTheaterCount(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojoTheaterCount_Mine_Previous2Weeks()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now).AddDays(-7);
			var test = new MineBoxOfficeMojoTheaterCount(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojoTheaterCount_Mine_Previous3Weeks()
		{
			var weekendEnding = MovieDateUtil.LastSunday(DateTime.Now).AddDays(-14);
			var test = new MineBoxOfficeMojoTheaterCount(weekendEnding);

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine($"Weekend Ending: {weekendEnding}");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}
	}
}