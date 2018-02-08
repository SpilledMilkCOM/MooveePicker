using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MoviePicker.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MoviePickerTests : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_OutOf01()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(1).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf02()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(2).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf03()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(3).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(2, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf04()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(4).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf05()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(5).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf06()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(6).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf07()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(7).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf08()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(8).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf09()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(9).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_OutOf10()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(10).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_WeekEnding_20170604()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// FML 
			movies.Add(ConstructMovie(id++, "Wonder Woman", 103, 845));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 23.9m, 239));
			movies.Add(ConstructMovie(id++, "Pirates", 22.1m, 193));
			movies.Add(ConstructMovie(id++, "Guardians", 9.8m, 74));
			movies.Add(ConstructMovie(id++, "Baywatch", 8.7m, 62));
			movies.Add(ConstructMovie(id++, "Alien", 4.1m, 31));
			movies.Add(ConstructMovie(id++, "Everything Everything", 3.3m, 22));
			movies.Add(ConstructMovie(id++, "Diary of a Wimpy Kid", 1.3m, 17));
			movies.Add(ConstructMovie(id++, "Snatched", 1.3m, 14));
			movies.Add(ConstructMovie(id++, "King Arthur", 1.2m, 12));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.AreEqual(7, best.Movies.Count(movie => movie.Name == "Everything Everything"));
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_WeekEnding_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// Weekend gross from box office mojo (with FML bux)
 
			movies.Add(ConstructMovie(id++, "Wonder Woman", 58520672m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 31688375m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 5988370m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10704103m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 4648207m, 60));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3810867m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 1826579m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 968506m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 656843m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12180704m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1627295m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 6312367m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 455744m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 491644m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 512445m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.AreEqual(5, best.Movies.Count(movie => movie.Name == "Guardians of the Galaxy"));
			Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Alien: Covenant"));
			Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Diary of a wimpy Kid"));
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_WeekEnding_20170618()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            // Weekend gross from box office mojo (with FML bux)

            movies.Add(ConstructMovie(id++, "Cars 3", 53.68868m, 719));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 41.26885m, 478));
            movies.Add(ConstructMovie(id++, "All Eyez on Me", 26.435354m, 327));
            movies.Add(ConstructMovie(id++, "Rough Night", 8.004283m, 243));
            movies.Add(ConstructMovie(id++, "The Mummy", 14.51096m, 167));
            movies.Add(ConstructMovie(id++, "47 Meters Down", 11.205561m, 105));
            movies.Add(ConstructMovie(id++, "Captain Underpants", 7.181541m, 78));
            movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 8.964039m, 71));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 5.071732m, 60));
            movies.Add(ConstructMovie(id++, "It Comes at night", 2.601468m, 34));
            movies.Add(ConstructMovie(id++, "The Book of Henry", 1.42454m, 31));
            movies.Add(ConstructMovie(id++, "Baywatch", 1.610987m, 29));
            movies.Add(ConstructMovie(id++, "Megan Leavey", 2.541644m, 25));
            movies.Add(ConstructMovie(id++, "Alien: Covenant", 0.568149m, 11));
            movies.Add(ConstructMovie(id++, "Everything, Everything", 0.356241m, 10));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);

            Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
            Assert.AreEqual(7, best.Movies.Count(movie => movie.Name == "Pirates of the caribbean"));
       }

        [TestMethod]
		public void MoviePicker_ChooseBest_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_Top10_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks());

			var best = test.ChooseBest(10);

			WritePicker(test);

			foreach (var movieList in best)
			{
				Logger.WriteLine(string.Empty);
				WriteMovies(movieList);
			}
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private List<IMovie> ThisWeeksMoviesPicks()
		{
			var movies = new List<IMovie>();

			int id = 1;

			movies.Add(ConstructMovie(id++, "Fifty Shades Freed", 38.4m, 556));
			movies.Add(ConstructMovie(id++, "Peter Rabbit", 18.7m, 276));
			movies.Add(ConstructMovie(id++, "The 1517 to Paris", 15.1m, 221));
			movies.Add(ConstructMovie(id++, "Jumanji Welcome to the Jungle", 7.2m, 126));
			movies.Add(ConstructMovie(id++, "The Greatest Showman", 6m, 101));
			movies.Add(ConstructMovie(id++, "Maze Runner The Death Cure", 5.1m, 83));
			movies.Add(ConstructMovie(id++, "Winchester", 4m, 66));
			movies.Add(ConstructMovie(id++, "The Shape of Water", 3.3m, 51));
			movies.Add(ConstructMovie(id++, "The Post", 3.2m, 50));
			movies.Add(ConstructMovie(id++, "Hostiles", 2.7m, 46));
			movies.Add(ConstructMovie(id++, "12 Strong", 2.6m, 43));
			movies.Add(ConstructMovie(id++, "Den of Thieves", 2.3m, 40));
			movies.Add(ConstructMovie(id++, "Three Billboards Outside Ebbing Missouri", 2.2m, 35));
			movies.Add(ConstructMovie(id++, "I Tonya", 1.8m, 30));
			movies.Add(ConstructMovie(id++, "Darkest Hour", 1.7m, 29));

			return movies;
		}
	}
}