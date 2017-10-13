using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;

namespace MoviePicker.Tests
{
	[TestClass]
    [ExcludeFromCodeCoverage]
    public class TitleMatchTests
	{
		[TestMethod, TestCategory("Mock")]
		public void TitleMatch_Match_Exact()
		{
			var test = ConstructTestObject();
			var actual = test.Match("Test Title", "Test Title");

			Assert.AreEqual(1.0m, actual);
		}

		[TestMethod, TestCategory("Mock")]
		public void TitleMatch_Match_FirstContainsSecond()
		{
			var test = ConstructTestObject();
			var actual = test.Match("Test Title", "Test");

			// 4 characters match out of 10

			Assert.AreEqual(0.4m, actual);
		}

		[TestMethod, TestCategory("Mock")]
		public void TitleMatch_Match_SecondContainsFirst()
		{
			var test = ConstructTestObject();
			var actual = test.Match("Test", "Test Title");

			// 4 characters match out of 10

			Assert.AreEqual(0.4m, actual);
		}

		[TestMethod, TestCategory("Mock")]
		public void TitleMatch_Match_MatchFirst3Characters()
		{
			var test = ConstructTestObject();
			var actual = test.Match("Tess", "Test Title");

			// 3 characters match out of 4 (use smallest for the denominator)

			Assert.AreEqual(0.75m, actual);
		}

		[TestMethod, TestCategory("Mock")]
		public void TitleMatch_Match_MatchFirst3Characters_Title2()
		{
			var test = ConstructTestObject();
			var actual = test.Match("Test Title", "Tess");

			// 3 characters match out of 4 (use smallest for the denominator)

			Assert.AreEqual(0.75m, actual);
		}

		private TitleMatch ConstructTestObject()
		{
			return new TitleMatch();
		}
	}
}