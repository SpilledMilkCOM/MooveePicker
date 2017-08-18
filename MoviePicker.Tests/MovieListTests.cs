using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MooveePicker;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

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