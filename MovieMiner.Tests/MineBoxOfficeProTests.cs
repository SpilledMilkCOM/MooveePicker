using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBoxOfficeProTests
	{
		private const string PRIMARY_TEST_CATEGORY = "Mining";

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineBoxOfficePro_Mine()
		{
			var test = new MineBoxOfficePro();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
		}
	}
}