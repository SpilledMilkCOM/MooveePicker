using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineToddThatcherTests : MineTestBase
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
		public void MineToddThatcher_Mine()
		{
			var test = new MineToddThatcher();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Todd M Thatcher ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}
	}
}