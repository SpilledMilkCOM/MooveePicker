using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineNerdTests
	{
		private const string PRIMARY_TEST_CATEGORY = "Mining";

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineNerd_Mine()
		{
			var test = new MineNerd();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
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