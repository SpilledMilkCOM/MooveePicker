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
		[TestMethod]
		public void MineNerd_Mine()
		{
			var test = new MineNerd();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
		}

		[TestMethod]
		public void MineNerd_MineAsync()
		{
			var test = new MineNerd();

			var actual = test.MineAsync();

			Assert.IsNotNull(actual.Result);
			Assert.IsTrue(actual.Result.Any(), "The list was empty.");
		}

		[TestMethod]
		public void MineNerd_Serialize()
		{
			var test = new MinerNerdData
			{
				Year = 2017,
				Season = "Summer",
				Week = 12,
				Movies = new MinerNerdMovie[]
				{
					new MinerNerdMovie {Title = "Test Title", Bux = 123, Index = 0}
				}
			};

			var json = JsonConvert.SerializeObject(test);

			Assert.IsNotNull(json);
		}
	}
}