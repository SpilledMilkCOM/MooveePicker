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
	public class MoviePickerTest_20170813 : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_Parker_20170813()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

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

            test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_BoxOfficePro_20170813()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Annabelle", 30m, 380));
            movies.Add(ConstructMovie(id++, "The Nut Job 2", 10m, 176));
            movies.Add(ConstructMovie(id++, "Dunkirk", 11.99m, 143));
            movies.Add(ConstructMovie(id++, "The Dark Tower", 7.66m, 106));
            movies.Add(ConstructMovie(id++, "Girls Trip", 6.84m, 89));
            movies.Add(ConstructMovie(id++, "The Emoji Movie", 6m, 77));
            movies.Add(ConstructMovie(id++, "Spider-Man", 5.75m, 75));
            movies.Add(ConstructMovie(id++, "Kidnap", 5.21m, 72));
            movies.Add(ConstructMovie(id++, "Detroit", 4.28m, 56));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 4.49m, 54));
            movies.Add(ConstructMovie(id++, "The Glass Castle", 3.5m, 52));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 3.8m, 44));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 3.82m, 43));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }

        [TestMethod]
        public void MoviePicker_ChooseBest_NerdGuru_20170813()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Annabelle", 27m, 380));
            movies.Add(ConstructMovie(id++, "The Nut Job 2", 12m, 176));
            movies.Add(ConstructMovie(id++, "Dunkirk", 9.995m, 143));
            movies.Add(ConstructMovie(id++, "The Dark Tower", 7.409m, 106));
            movies.Add(ConstructMovie(id++, "Girls Trip", 6.221m, 89));
            movies.Add(ConstructMovie(id++, "The Emoji Movie", 5.382m, 77));
            movies.Add(ConstructMovie(id++, "Spider-Man", 5.242m, 75));
            movies.Add(ConstructMovie(id++, "Kidnap", 5.032m, 72));
            movies.Add(ConstructMovie(id++, "Detroit", 3.914m, 56));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 3.774m, 54));
            movies.Add(ConstructMovie(id++, "The Glass Castle", 3.5m, 52));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 3.075m, 44));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 3.005m, 43));
            movies.Add(ConstructMovie(id++, "Baby Driver", 1.467m, 21));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 1.398m, 20));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }

        [TestMethod]
        public void MoviePicker_ChooseBest_Todd_20170813()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Annabelle", 31.4m, 380));
            movies.Add(ConstructMovie(id++, "The Nut Job 2", 13.2m, 176));
            movies.Add(ConstructMovie(id++, "Dunkirk", 11.7m, 143));
            movies.Add(ConstructMovie(id++, "The Dark Tower", 8.1m, 106));
            movies.Add(ConstructMovie(id++, "Girls Trip", 7.2m, 89));
            movies.Add(ConstructMovie(id++, "The Emoji Movie", 5.9m, 77));
            movies.Add(ConstructMovie(id++, "Spider-Man", 5.85m, 75));
            movies.Add(ConstructMovie(id++, "Kidnap", 5m, 72));
            movies.Add(ConstructMovie(id++, "Detroit", 4.1m, 56));
            movies.Add(ConstructMovie(id++, "Atomic Blonde", 3.9m, 54));
            movies.Add(ConstructMovie(id++, "The Glass Castle", 3.6m, 52));
            movies.Add(ConstructMovie(id++, "Despicable Me 3", 3.15m, 44));
            movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 3.1m, 43));
            movies.Add(ConstructMovie(id++, "Baby Driver", 1.55m, 21));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 1.5m, 20));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }
    }
}