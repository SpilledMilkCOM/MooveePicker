using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MooveePicker;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Tests;

namespace MooviePicker.Tests
{
	[TestClass]
    [ExcludeFromCodeCoverage]
    public class MoviePickerTest_20170611 : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_Parker_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 38m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 11m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 12m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 5m, 60));
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
		public void MoviePicker_ChooseBest_Debug_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55m, 613));
			movies.Add(ConstructMovie(id++, "Baywatch", 5m, 69));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 12m, 143));
			//movies.Add(ConstructMovie(id++, "Baywatch", 5m, 69));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.6m, 8));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(86900000m, best.TotalEarnings);
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_Doug_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 35m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 12m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 11m, 143));
			movies.Add(ConstructMovie(id++, "Baywatch", 4m, 60));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3m, 59));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 2m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.5m, 8));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12m, 198));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.5m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 5m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.5m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0.5m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0.5m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_Raj_20170611()
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
		public void MoviePicker_ChooseBest_BoxOfficePro_20170611()
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
		public void MoviePicker_ChooseBest_Cinefiles_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// https://cinefilesreviews.com/2017/06/07/weekend-box-office-predictions-june-9-june-11/

			movies.Add(ConstructMovie(id++, "Wonder Woman", 83m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 65m, 526));
			movies.Add(ConstructMovie(id++, "It Comes at night", 20m, 150));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 16m, 198));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10m, 143));

			// End of picks, using Parker's data for the rest.
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3.3m, 59));
			movies.Add(ConstructMovie(id++, "Baywatch", 5m, 69));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.6m, 8));
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
		public void MoviePicker_ChooseBest_TheNumbers_20170611()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// http://www.the-numbers.com/news/222490830-Weekend-Predictions-Can-Mummy-Bury-the-Competition

			movies.Add(ConstructMovie(id++, "Wonder Woman", 47m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 34m, 526));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12m, 198));
			movies.Add(ConstructMovie(id++, "It Comes at night", 12m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10m, 143));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3m, 59));

			// End of picks, using Parker's data for the rest.
			movies.Add(ConstructMovie(id++, "Baywatch", 5m, 69));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.6m, 8));
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
		public void MoviePicker_ChooseBest_ToddMThatcher_20170611()
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
	}
}