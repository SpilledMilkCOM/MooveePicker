using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using Newtonsoft.Json;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineNerdTests : MineTestBase
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
		public void MineNerd_Mine()
		{
			var test = new MineNerd();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== FML Nerd (Pete) ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineNerd_Serialize()
		{
			// This test helps visualize the serialization to see how to massage Pete's data.
			// His data is a hybrid of Javascript and JSON.

			var test = new MineNerdData
			{
				Year = 2017,
				Season = "Summer",
				Week = 12,
				Movies = new MineNerdMovie[]
				{
					new MineNerdMovie {Index = 0, Title = "Test Title", Bux = 123, CurrentEstimatedBoxOffice = 1230000 }
				}
			};

			var json = JsonConvert.SerializeObject(test);

			Assert.IsNotNull(json);
		}
	}
}