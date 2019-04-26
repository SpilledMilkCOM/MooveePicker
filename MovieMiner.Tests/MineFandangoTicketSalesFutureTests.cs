using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;
using Unity.Injection;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	[DeploymentItem("appSettings.secret.config")]
	public class MineFandangoTicketSalesFutureTests : MineTestBase
	{
		private const string FUTURE_URL_KEY = "fandango:future";

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			var fandangoFutureUrl = ConfigurationManager.AppSettings[FUTURE_URL_KEY];

			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
			_unity.RegisterType<IMoviePicker, MsfMovieSolver>();
			_unity.RegisterType<IMiner, MineFandangoTicketSalesFuture>(new InjectionConstructor(fandangoFutureUrl));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSalesFuture_Mine()
		{
			var test = ConstructTest();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			var gameEnd = MovieDateUtil.GameSunday();

			WriteMovies(actual);
		}

		private IMiner ConstructTest()
		{
			return _unity.Resolve<IMiner>();
		}
	}
}