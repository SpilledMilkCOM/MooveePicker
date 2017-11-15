﻿using System.Diagnostics.CodeAnalysis;

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
		public void Movie_Equals_StartsWith_Matches()
		{
			var movie1 = new Movie { Name = "Star Wars" };
			var movie2 = new Movie { Name = "Star Wars A New Hope" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_StartsWithSecond_Matches()
		{
			var movie1 = new Movie { Name = "Star Wars A New Hope" };
			var movie2 = new Movie { Name = "Star Wars" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_EndsWith_Matches()
		{
			var movie1 = new Movie { Name = "Boo 2 A Madea Halloween" };
			var movie2 = new Movie { Name = "Tyler Perrys Boo 2 A Madea Halloween" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_EndsWithSecond_Matches()
		{
			var movie1 = new Movie { Name = "Tyler Perrys Boo 2 A Madea Halloween" };
			var movie2 = new Movie { Name = "Boo 2 A Madea Halloween" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}
	}
}