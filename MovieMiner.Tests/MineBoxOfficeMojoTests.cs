using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MoviePicker.Common.Interfaces;

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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineBoxOfficeMojo_Mine()
		{
			var test = new MineBoxOfficeMojo();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
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

	}
}