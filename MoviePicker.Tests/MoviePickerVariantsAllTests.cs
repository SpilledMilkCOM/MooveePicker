﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MooveePicker;
using MooveePicker.Simulations;
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
	public class MoviePickerVariantsAllTests : MoviePickerTestBase
	{
		private const string TEST_CATEGORY = "Simulation";

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<IMovie, Movie>();
			_unity.RegisterType<IMovieList, MovieList>();
			_unity.RegisterType<IMoviePicker, MoviePickerVariantsAll>();
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf01()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(1).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf02()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(2).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(1, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf03()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(3).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(2, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf04()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(4).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf05()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(5).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(6, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf06()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(6).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf07()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(7).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf08()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(8).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(7, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf09()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(9).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_OutOf10()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks().Take(10).ToList());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.AreEqual(8, best.Movies.Count());
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_WeekEnding_20170604()
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

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_2017061()
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

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170618()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// The baseline movie list.

			movies.Add(ConstructMovie(id++, "Cars 3", 60m, 719));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 36m, 478));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 24.4m, 327));
			movies.Add(ConstructMovie(id++, "Rough Night", 15.1m, 243));
			movies.Add(ConstructMovie(id++, "The Mummy", 13m, 167));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 7m, 105));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 5.5m, 78));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 5m, 71));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 3.5m, 60));
			movies.Add(ConstructMovie(id++, "It Comes at night", 3m, 34));
			movies.Add(ConstructMovie(id++, "The Book of Henry", 2m, 31));
			movies.Add(ConstructMovie(id++, "Baywatch", 2.324m, 29));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 1.9m, 25));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 0.913m, 11));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 0.8m, 10));

            IgnoreMovies(movies, 7m);

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170618_ByPercent()
		{
			var test = ConstructTestObject(true);
			var movies = new List<IMovie>();
			int id = 1;

			// The baseline movie list.

			movies.Add(ConstructMovie(id++, "Cars 3", 60m, 719));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 36m, 478));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 24.4m, 327));
			movies.Add(ConstructMovie(id++, "Rough Night", 15.1m, 243));
			movies.Add(ConstructMovie(id++, "The Mummy", 13m, 167));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 7m, 105));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 5.5m, 78));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 5m, 71));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 3.5m, 60));
			movies.Add(ConstructMovie(id++, "It Comes at night", 3m, 34));
			movies.Add(ConstructMovie(id++, "The Book of Henry", 2m, 31));
			movies.Add(ConstructMovie(id++, "Baywatch", 2.324m, 29));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 1.9m, 25));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 0.913m, 11));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 0.8m, 10));

			IgnoreMovies(movies, 7m);

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariants)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariants)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170625()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            // The baseline movie list.

            movies.Add(ConstructMovie(id++, "Transformers", 70m, 560));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 30m, 286));
            movies.Add(ConstructMovie(id++, "Cars 3", 30m, 278));
            movies.Add(ConstructMovie(id++, "All Eyez on Me", 10m, 104));
            movies.Add(ConstructMovie(id++, "The Mummy", 8m, 73));
            movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 7.25m, 58));
            movies.Add(ConstructMovie(id++, "47 Meters Down", 6.875m, 55));
            movies.Add(ConstructMovie(id++, "Captain Underpants", 5.625m, 45));
            movies.Add(ConstructMovie(id++, "Rough Night", 4.875m, 39));
            movies.Add(ConstructMovie(id++, "Tubelight", 4.25m, 34));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 4m, 32));
            movies.Add(ConstructMovie(id++, "Beatriz at Dinner", 2.125m, 17));
            movies.Add(ConstructMovie(id++, "Megan Leavey", 2m, 16));
            movies.Add(ConstructMovie(id++, "It Comes at night", 1.625m, 13));
            movies.Add(ConstructMovie(id++, "The Book of Henry", 1.375m, 11));

            IgnoreMovies(movies, 5m);

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
			Logger.WriteLine(string.Empty);

            foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
            {
                WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
            }
        }

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170730()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            // The baseline movie list.

            movies.Add(ConstructMovie(id++, "The Emoji Movie", 31.8666666666667m, 400));
            movies.Add(ConstructMovie(id++, "Dunkirk", 29.0386666666667m, 373));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 21.0666666666667m, 289));
            movies.Add(ConstructMovie(id++, "Girls Trip", 17.4663333333333m, 219));
            movies.Add(ConstructMovie(id++, "Spider-Man", 12.1573333333333m, 151));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 10.1466666666667m, 126));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 7.909m, 100));
            movies.Add(ConstructMovie(id++, "Valerian and the City of a Thousand Planets", 7.01533333333333m, 99));
            movies.Add(ConstructMovie(id++, "Baby Driver", 4.142m, 52));
            movies.Add(ConstructMovie(id++, "The Big Sick", 3.52533333333333m, 44));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 3.123m, 39));
            movies.Add(ConstructMovie(id++, "Wish Upon", 1.13966666666667m, 15));
            movies.Add(ConstructMovie(id++, "Cars 3", 1.04566666666667m, 14));
            movies.Add(ConstructMovie(id++, "Transformers", 0.471666666666667m, 6));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy Vol. 2", 0.287666666666667m, 4));

            IgnoreMovies(movies, 5m);

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
			Logger.WriteLine(string.Empty);

            foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
            {
                WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
            }
        }

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170813()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            // The baseline movie list.

            movies.Add(ConstructMovie(id++, "Annabelle", 30.15m, 380));
            movies.Add(ConstructMovie(id++, "The Nut Job 2", 11.45m, 176));
            movies.Add(ConstructMovie(id++, "Dunkirk", 11.631875m, 143));
            movies.Add(ConstructMovie(id++, "The Dark Tower", 7.793625m, 106));
            movies.Add(ConstructMovie(id++, "Girls Trip", 6.897625m, 89));
            movies.Add(ConstructMovie(id++, "The Emoji Movie", 5.88525m, 77));
            movies.Add(ConstructMovie(id++, "Spider-Man", 5.724m, 75));
            movies.Add(ConstructMovie(id++, "Kidnap", 5.109m, 72));
            movies.Add(ConstructMovie(id++, "Detroit", 4.16675m, 56));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 4.17925m, 54));
            movies.Add(ConstructMovie(id++, "The Glass Castle", 3.5375m, 52));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 3.465625m, 44));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 3.448125m, 43));
            movies.Add(ConstructMovie(id++, "Baby Driver", 1.52925m, 21));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 1.4745m, 20));

            IgnoreMovies(movies, 5m);

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
			Logger.WriteLine(string.Empty);

            foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
            {
                WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
            }
        }

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20170820()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			// The baseline movie list.

			movies.Add(ConstructMovie(id++, "The Hitman's Bodyguard", 16.9416666666667m, 254));
			movies.Add(ConstructMovie(id++, "Annabelle", 14.8578333333333m, 216));
			movies.Add(ConstructMovie(id++, "Logan Lucky", 10.925m, 184));
			movies.Add(ConstructMovie(id++, "Dunkirk", 7.10666666666667m, 98));
			movies.Add(ConstructMovie(id++, "The Nut Job 2", 4.56583333333333m, 62));
			movies.Add(ConstructMovie(id++, "Spider-Man", 3.97016666666667m, 58));
			movies.Add(ConstructMovie(id++, "The Emoji Movie", 3.52533333333333m, 52));
			movies.Add(ConstructMovie(id++, "Girls Trip", 3.6845m, 50));
			movies.Add(ConstructMovie(id++, "The Dark Tower", 3.337m, 48));
			movies.Add(ConstructMovie(id++, "The Glass Castle", 3.0145m, 48));
			movies.Add(ConstructMovie(id++, "Wind River", 3.0184m, 43));
			movies.Add(ConstructMovie(id++, "Kidnap", 2.5836m, 38));
			movies.Add(ConstructMovie(id++, "Atomic Blonde", 2.2956m, 34));
			movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 1.9512m, 29));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 1.834m, 28));

			IgnoreMovies(movies, 5m);

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20180729()
		{
			var test = ConstructTestObject();

			// By default two adjustments are made (+/- 100,000)

			test.AddMovies(ConstructMovieList_20180729());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20180729_Percentage()
		{
			var test = ConstructTestObject();

			// By default two adjustments are made (+/- 3%)

			test.AddMovies(ConstructMovieList_20180729());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20180729_PercentageCustom()
		{
			var test = ConstructTestObject(true);

			// By default two adjustments are made (+/- 3%) because EarningsAdjustmentMax is 3%

			// The setting below will allow for 6 adjustments and one baseline.

			((MoviePickerVariantsAll)test).EarningsAdjustment = 0.01m;
			((MoviePickerVariantsAll)test).EarningsAdjustmentMax = 0.04m;

			test.AddMovies(ConstructMovieList_20180729());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
			Logger.WriteLine(string.Empty);

			foreach (var movieList in ((MoviePickerVariantsAll)test).GetRankedMovieLists())
			{
				WriteMovies(movieList);
				Logger.WriteLine($"Total List Count: {((MoviePickerVariantsAll)test).GetRankedMovieListCount(movieList)}");
				Logger.WriteLine(string.Empty);
			}
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private List<IMovie> ConstructMovieList_20180729()
		{
			var movies = new List<IMovie>();
			int id = 1;

			// Movie list generated from FML.xlsx CodeGen tab

			movies.Add(ConstructMovie(id++, "Mission Impossible  Fallout", 62.675m, 756));
			movies.Add(ConstructMovie(id++, "Mamma Mia Here We Go Again", 18.99375m, 227));
			movies.Add(ConstructMovie(id++, "The Equalizer 2", 16.5m, 201));
			movies.Add(ConstructMovie(id++, "Hotel Transylvania 3 Summer Vacation", 13.475m, 157));
			movies.Add(ConstructMovie(id++, "Teen Titans GO to the Movies", 15.10625m, 149));
			movies.Add(ConstructMovie(id++, "AntMan  the Wasp", 9.161538461538461538461538462m, 116));
			movies.Add(ConstructMovie(id++, "Incredibles 2", 8.023076923076923076923076923m, 89));
			movies.Add(ConstructMovie(id++, "Jurassic World Fallen Kingdom", 6.9307692307692307692307692308m, 76));
			movies.Add(ConstructMovie(id++, "Skyscraper", 5.6076923076923076923076923077m, 61));
			movies.Add(ConstructMovie(id++, "Blindspotting", 2.3333333333333333333333333333m, 41));
			movies.Add(ConstructMovie(id++, "The First Purge", 2.3384615384615384615384615385m, 28));
			movies.Add(ConstructMovie(id++, "Eighth Grade", 0.946m, 26));
			movies.Add(ConstructMovie(id++, "Unfriended Dark Web", 1.6m, 18));
			movies.Add(ConstructMovie(id++, "Sorry to Bother You", 1.8m, 18));
			movies.Add(ConstructMovie(id++, "Three Identical Strangers", 1.1m, 16));

			IgnoreMovies(movies);
			IncludeMoviesByEfficiency(movies, 6);
			IncludeMoviesByBoxOffice(movies, 1);

			return movies;
		}

		protected IMoviePicker ConstructTestObject(bool earningsAdjustmentByPercent = false)
        {
            var result = base.ConstructTestObject() as MoviePickerVariantsAll;

			result.EarningsAdjustmentByPercent = earningsAdjustmentByPercent;

			if (!earningsAdjustmentByPercent)
			{
				result.EarningsAdjustment = 100000m;
				result.EarningsAdjustmentMax = 100000m;

			}
			result.LogMessagesMax = 1000;

            return result;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="movies"></param>
		/// <param name="topCount"></param>
		private void IncludeMoviesByEfficiency(IEnumerable<IMovie> movies, int topCount)
		{
			foreach (var movie in movies.OrderByDescending(item => item.Efficiency).Take(topCount))
			{
				movie.AdjustEarnings = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="movies"></param>
		/// <param name="topCount"></param>
		private void IncludeMoviesByBoxOffice(IEnumerable<IMovie> movies, int topCount)
		{
			foreach (var movie in movies.OrderByDescending(item => item.EarningsBase).Take(topCount))
			{
				movie.AdjustEarnings = true;
			}
		}

		private void IgnoreMovies(IEnumerable<IMovie> movies)
		{
			foreach (var movie in movies)
			{
				movie.AdjustEarnings = false;
			}
		}

		/// <summary>
		/// Ignore the movies BELOW (and equal) the lower bound.
		/// </summary>
		/// <param name="movies"></param>
		/// <param name="lowerBound"></param>
		private void IgnoreMovies(IEnumerable<IMovie> movies, decimal lowerBound)
        {
            lowerBound *= 1000000;

            foreach (var movie in movies)
            {
                movie.AdjustEarnings = movie.Earnings > lowerBound;
            }
        }

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

			foreach (var movie in movies)
			{
				if (movie.Id > 7)
				{
					movie.AdjustEarnings = false;
				}
			}

			return movies;
		}
	}
}