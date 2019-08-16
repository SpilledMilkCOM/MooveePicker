using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using SM.Common.Interfaces;
using SM.Common.Tests;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineFandangoTicketSalesByDayTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
			_unity.RegisterType<IMoviePicker, MsfMovieSolver>();
			_unity.RegisterType<IMiner, MineFandangoTicketSalesByDay>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_Mine()
		{
			var test = ConstructTest();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			var gameEnd = MovieDateUtil.GameSunday();

			WriteMovies(actual.Where(movie => movie.WeekendEnding == gameEnd.AddDays(-2)));
			WriteMovies(actual.Where(movie => movie.WeekendEnding == gameEnd.AddDays(-1)));
			WriteMovies(actual.Where(movie => movie.WeekendEnding == gameEnd));
		}

		private IMiner ConstructTest()
		{
			return _unity.Resolve<IMiner>();
		}
	}
}