using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MoviePicker.Tests
{
	[TestClass]
    [ExcludeFromCodeCoverage]
    public class MovieListTests : MoviePickerTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<IMovie, Movie>();
			_unity.RegisterType<IMovieList, MovieList>();
			_unity.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();
		}

		[TestMethod]
		public void MovieList_Add_ReconcileCount()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[0]);

			Assert.AreEqual(1, test.Movies.Count(), "The counts are different");
		}

		[TestMethod]
		public void MovieList_Add_ReconcileCount4()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[0]);
			test.Add(movies[1]);
			test.Add(movies[2]);

			Assert.IsTrue(test.CanAdd(movies[3]), "Should be able to add the movie.");

			test.Add(movies[3]);

			Assert.AreEqual(4, test.Movies.Count(), "The counts are different");
		}

		[TestMethod]
		public void MovieList_Add_ReconcileCount8()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[0]);
			test.Add(movies[0]);
			test.Add(movies[1]);
			test.Add(movies[1]);
			test.Add(movies[2]);
			test.Add(movies[2]);
			test.Add(movies[3]);
			test.Add(movies[3]);

			Assert.AreEqual(8, test.Movies.Count(), "The hash codes are different");
		}

		[TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MovieList_Add_9Items_ThrowsException()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
		}

		[TestMethod]
		public void MovieList_CanAdd_ReconcileCount()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			Assert.IsTrue(test.CanAdd(movies[0]));

			test.Add(movies[0]);

			Assert.IsFalse(test.CanAdd(movies[0]));
		}

		[TestMethod]
		public void MovieList_CanAdd_OnlyTestsCostNotQuantity()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);

			Assert.IsTrue(test.CanAdd(movies[5]));
		}

		[TestMethod]
		public void MovieList_GetHashCode_DifferentListProducesDifferentHashCode()
		{
			var movies = ThisWeeksMoviesPicks();
			var test1 = UnityContainer.Resolve<IMovieList>();
			var test2 = UnityContainer.Resolve<IMovieList>();

			test1.Add(movies[0]);
			test1.Add(movies[2]);

			test2.Add(movies[0]);
			test2.Add(movies[1]);

			Assert.AreNotEqual(test1.GetHashCode(), test2.GetHashCode(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_GetHashCode_SameListProducesSameHashCode()
		{
			var movies = ThisWeeksMoviesPicks();
			var test1 = UnityContainer.Resolve<IMovieList>();
			var test2 = UnityContainer.Resolve<IMovieList>();

			test1.Add(movies[0]);
			test1.Add(movies[1]);

			test2.Add(movies[0]);
			test2.Add(movies[1]);

			Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_GetHashCode_ReverseListProducesSameHashCode()
		{
			var movies = ThisWeeksMoviesPicks();
			var test1 = UnityContainer.Resolve<IMovieList>();
			var test2 = UnityContainer.Resolve<IMovieList>();

			test1.Add(movies[0]);
			test1.Add(movies[1]);

			test2.Add(movies[1]);
			test2.Add(movies[0]);

			Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_GetHashCode_OutOfOrderListProducesSameHashCode()
		{
			var movies = ThisWeeksMoviesPicks();
			var test1 = UnityContainer.Resolve<IMovieList>();
			var test2 = UnityContainer.Resolve<IMovieList>();

			test1.Add(movies[0]);
			test1.Add(movies[1]);
			test1.Add(movies[2]);
			test1.Add(movies[3]);
			test1.Add(movies[4]);

			test2.Add(movies[1]);
			test2.Add(movies[0]);
			test2.Add(movies[4]);
			test2.Add(movies[3]);
			test2.Add(movies[2]);

			Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_IsFull_TestsQuantityAndNotCost()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);

			Assert.IsFalse(test.IsFull);

			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);
			test.Add(movies[5]);

			Assert.IsTrue(test.IsFull);
		}

		[TestMethod]
		public void MovieList_ToString()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[0]);
			test.Add(movies[2]);
			test.Add(movies[5]);

			Assert.AreEqual("WW,CU,Grdns", test.ToString(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_ToString_DayOfWeek()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			movies[0].Day = DayOfWeek.Friday;
			movies[2].Day = DayOfWeek.Saturday;
			movies[5].Day = DayOfWeek.Sunday;

			test.Add(movies[0]);
			test.Add(movies[2]);
			test.Add(movies[5]);

			Assert.AreEqual("WW-Fri,CU-Sat,Grdns-Sun", test.ToString(), "The hash codes are different");
		}

		[TestMethod]
		public void MovieList_ToStringx2()
		{
			var movies = ThisWeeksMoviesPicks();
			var test = UnityContainer.Resolve<IMovieList>();

			test.Add(movies[0]);
			test.Add(movies[0]);
			test.Add(movies[2]);
			test.Add(movies[2]);
			test.Add(movies[5]);
			test.Add(movies[5]);

			Assert.AreEqual("WWx2,CUx2,Grdnsx2", test.ToString(), "The hash codes are different");
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private List<IMovie> ThisWeeksMoviesPicks()
		{
			var movies = new List<IMovie>();

			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 38, 526));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12, 198));
			movies.Add(ConstructMovie(id++, "It Comes at Night", 20, 150));
			movies.Add(ConstructMovie(id++, "Pirates", 12, 143));
			movies.Add(ConstructMovie(id++, "Guardians", 5, 70));
			movies.Add(ConstructMovie(id++, "Baywatch", 5, 60));
			movies.Add(ConstructMovie(id++, "Meagan Leavey", 3.3m, 59));
			movies.Add(ConstructMovie(id++, "Everything", 1.5m, 28));
			movies.Add(ConstructMovie(id++, "Alien", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1, 15));
			movies.Add(ConstructMovie(id++, "Snatched", 0.6m, 9));
			movies.Add(ConstructMovie(id++, "Best of the Rest", 1.1m, 9));
			movies.Add(ConstructMovie(id++, "Diary of a Wimpy Kid", 0.6m, 8));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.5m, 7));

			return movies;
		}
	}
}