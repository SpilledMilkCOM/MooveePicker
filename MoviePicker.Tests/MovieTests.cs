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
		public void Movie_Equals_ContainsMatch_Matches()
		{
			var movie1 = new Movie { Name = "The Grinch" };
			var movie2 = new Movie { Name = "Dr Seuss The Grinch 2018" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

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
		public void Movie_Equals_StartsWith_Matches2()
		{
			var movie1 = new Movie { Name = "Anna" };
			var movie2 = new Movie { Name = "Annabelle Comes Home" };

			Assert.IsFalse(movie1.Equals(movie2), "The movie names equal");
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

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_Case_Insensitive_Match()
		{
			var movie1 = new Movie { Name = "The House with a Clock in its Walls" };
			var movie2 = new Movie { Name = "The House with a Clock in Its Walls" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names do NOT equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_Case_Insensitive_Match_With_Noise()
		{
			var movie1 = new Movie { Name = "Hey Im Coupe blah blah blah blah blah The House with a Clock in its Walls" };
			var movie2 = new Movie { Name = "The House with a Clock in Its Walls" };

			Assert.IsFalse(movie1.Equals(movie2), "The movie names equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_Case_Insensitive_Match_With_Noise_At_End()
		{
			var movie1 = new Movie { Name = "Birds of Prey and the Fantabulous Emancipation of One Harley Quinn" };
			var movie2 = new Movie { Name = "Birds of Prey" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names aren't equal");
		}

		[TestMethod, TestCategory("Mock")]
		public void Movie_Equals_Case_Insensitive_Match_With_Noise_At_End2()
		{
			var movie1 = new Movie { Name = "Birds of Prey" };
			var movie2 = new Movie { Name = "Birds of Prey and the Fantabulous Emancipation of One Harley Quinn" };

			Assert.IsTrue(movie1.Equals(movie2), "The movie names aren't equal");
		}
	}
}