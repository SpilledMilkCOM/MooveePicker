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
	public class MoviePickerTest_20170730 : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_Parker_20170730()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

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

            test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_NerdGuru_20170730()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "The Emoji Movie", 30m, 400));
            movies.Add(ConstructMovie(id++, "Dunkirk", 30.316m, 373));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 26m, 289));
            movies.Add(ConstructMovie(id++, "Girls Trip", 17.799m, 219));
            movies.Add(ConstructMovie(id++, "Spider-Man", 12.272m, 151));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 10.24m, 126));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 8.127m, 100));
            movies.Add(ConstructMovie(id++, "Valerian and the City of a Thousand Planets", 8.046m, 99));
            movies.Add(ConstructMovie(id++, "Baby Driver", 4.226m, 52));
            movies.Add(ConstructMovie(id++, "The Big Sick", 3.576m, 44));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 3.169m, 39));
            movies.Add(ConstructMovie(id++, "Wish Upon", 1.219m, 15));
            movies.Add(ConstructMovie(id++, "Cars 3", 1.137m, 14));
            movies.Add(ConstructMovie(id++, "Transformers", 0.487m, 6));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy Vol. 2", 0.325m, 4));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }

        [TestMethod]
        public void MoviePicker_ChooseBest_Todd_20170730()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "The Emoji Movie", 32.8m, 400));
            movies.Add(ConstructMovie(id++, "Dunkirk", 28.4m, 373));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 18.6m, 289));
            movies.Add(ConstructMovie(id++, "Girls Trip", 17.3m, 219));
            movies.Add(ConstructMovie(id++, "Spider-Man", 12.1m, 151));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 10.1m, 126));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 7.8m, 100));
            movies.Add(ConstructMovie(id++, "Valerian and the City of a Thousand Planets", 6.5m, 99));
            movies.Add(ConstructMovie(id++, "Baby Driver", 4.1m, 52));
            movies.Add(ConstructMovie(id++, "The Big Sick", 3.5m, 44));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 3.1m, 39));
            movies.Add(ConstructMovie(id++, "Wish Upon", 1.1m, 15));
            movies.Add(ConstructMovie(id++, "Cars 3", 1m, 14));
            movies.Add(ConstructMovie(id++, "Transformers", 0.464m, 6));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy Vol. 2", 0.269m, 4));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }
    }
}