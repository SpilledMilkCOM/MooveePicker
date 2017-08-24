using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MoviePicker.Common;

namespace MoviePicker.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MovieTests
	{
		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_ExactMatch_Matches()
		{
			var movie1 = new Movie { Name = "Star Wars" };
			var movie2 = new Movie { Name = "Star Wars" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_Contains_Matches()
		{
			var movie1 = new Movie { Name = "Star Wars" };
			var movie2 = new Movie { Name = "Star Wars A New Hope" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_ContainsSecond_Matches()
		{
			var movie1 = new Movie { Name = "Star Wars A New Hope" };
			var movie2 = new Movie { Name = "Star Wars" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}
	}
}