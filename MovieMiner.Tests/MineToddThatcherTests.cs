using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineToddThatcherTests
	{
		[TestMethod]
		public void MineToddThatcher_Mine()
		{
			var test = new MineToddThatcher();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
		}
	}
}