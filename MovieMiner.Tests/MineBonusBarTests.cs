using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBonusBarTests : MineTestBase
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
		public void MineBonusBar_Mine()
		{
			var test = new MineBonusBar();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Bonus Bar ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBonusBar_ImportValues()
		{
			var test = new MineBonusBar();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Bonus Bar ====\n");
			foreach (var movie in actual.OrderByDescending(item => item.Cost))
			{
				Logger.WriteLine($"{movie.Earnings:N0}");
			}
		}
	}
}