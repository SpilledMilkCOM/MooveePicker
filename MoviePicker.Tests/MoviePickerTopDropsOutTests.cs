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
	public class MoviePickerTopDropsOutTests : MoviePickerTestBase
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
			_unity.RegisterType<IMoviePicker, MoviePickerTopDropsOut>();
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf01()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(1).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf02()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(2).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf03()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(3).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(2, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf04()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(4).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf05()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(5).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf06()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(6).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf07()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(7).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf08()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(8).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf09()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(9).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_OutOf10()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(10).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_WeekEnding_20170604()
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

			// From Raj
			//movies.Add(ConstructMovie(id++, "Wonder Woman", 103.25m, 845));
			//movies.Add(ConstructMovie(id++, "Captain Underpants", 23.9m, 239));
			//movies.Add(ConstructMovie(id++, "Pirates of the Caribbean: Dead Men Tell No Tales", 22.1m, 193));
			//movies.Add(ConstructMovie(id++, "Guardians of the Galaxy Vol. 2", 9.8m, 74));
			//movies.Add(ConstructMovie(id++, "Baywatch", 8.7m, 62));
			//movies.Add(ConstructMovie(id++, "Alien: Covenant", 4.1m, 31));
			//movies.Add(ConstructMovie(id++, "Everything Everything", 3.3m, 22));
			//movies.Add(ConstructMovie(id++, "Diary of a Wimpy Kid: The Long Haul", 1.3m, 17));
			//movies.Add(ConstructMovie(id++, "Snatched", 1.3m, 14));
			//movies.Add(ConstructMovie(id++, "King Arthur: Legend of the Sword", 1.2m, 12));

			// These movies seem inconsequential.
			//movies.Add(ConstructMovie(id++, "The Mummy", 38, 526));
			//movies.Add(ConstructMovie(id++, "It Comes at Night", 20, 150));
			//movies.Add(ConstructMovie(id++, "Meagan Leavey", 3.3m, 59));
			//movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1, 15));
			//movies.Add(ConstructMovie(id++, "Best of the Rest", 1.1m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.AreEqual(7, best.Movies.Count(movie => movie.Name == "Everything Everything"));
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_Parker_2017061()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 38m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 11m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 12m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 5m, 69));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3.3m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.6m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.5m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 5m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.5m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0.6m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0.5m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_Raj_2017061()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 50m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 32m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 10m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10.5m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 4m, 69));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 2.4m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 0.5m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.5m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 13m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.5m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 4.5m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.5m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0.5m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0.5m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_BoxOfficePro_2017061()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 50m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 32m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 11m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10.4m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 4.3m, 69));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 2.4m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 1.86m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 0m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 13.35m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.98m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 4.7m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePickerTopDropsOut_ChooseBest_ToddMThatcher_2017061()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 49.5m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 38.7m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 9.5m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 12m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 4m, 69));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3.3m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 0m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 1m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 7.18m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.39m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 4.5m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.72m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0.87m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
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