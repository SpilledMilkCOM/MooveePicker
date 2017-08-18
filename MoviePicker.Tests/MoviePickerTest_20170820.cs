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
	public class MoviePickerTest_20170820 : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_Parker_20170820()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

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

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Logger.WriteLine("\n==== Best Performer Disabled ====\n");

			test.EnableBestPerformer = false;

			best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_BoxOfficePro_20170820()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

			movies.Add(ConstructMovie(id++, "The Hitman's Bodyguard", 16.8m, 254));
			movies.Add(ConstructMovie(id++, "Annabelle", 14.5m, 216));
			movies.Add(ConstructMovie(id++, "Logan Lucky", 10.5m, 184));
			movies.Add(ConstructMovie(id++, "Dunkirk", 7.07m, 98));
			movies.Add(ConstructMovie(id++, "The Nut Job 2", 5.01m, 62));
			movies.Add(ConstructMovie(id++, "Spider-Man", 4.22m, 58));
			movies.Add(ConstructMovie(id++, "The Emoji Movie", 3.8m, 52));
			movies.Add(ConstructMovie(id++, "Girls Trip", 3.82m, 50));
			movies.Add(ConstructMovie(id++, "The Dark Tower", 3.52m, 48));
			movies.Add(ConstructMovie(id++, "The Glass Castle", 3.04m, 48));
			// From Todd's picks...
			movies.Add(ConstructMovie(id++, "Wind River", 3.1m, 43));
			movies.Add(ConstructMovie(id++, "Kidnap", 2.6m, 38));
			movies.Add(ConstructMovie(id++, "Atomic Blonde", 2.3m, 34));
			movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 1.95m, 29));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 1.8m, 28));

			test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);

			Logger.WriteLine("\n==== Best Performer Disabled ====\n");

			test.EnableBestPerformer = false;

			best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_NerdGuru_20170820()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

			movies.Add(ConstructMovie(id++, "The Hitman's Bodyguard", 17.5m, 254));
			movies.Add(ConstructMovie(id++, "Annabelle", 14.547m, 216));
			movies.Add(ConstructMovie(id++, "Logan Lucky", 12m, 184));
			movies.Add(ConstructMovie(id++, "Dunkirk", 6.6m, 98));
			movies.Add(ConstructMovie(id++, "The Nut Job 2", 4.175m, 62));
			movies.Add(ConstructMovie(id++, "Spider-Man", 3.906m, 58));
			movies.Add(ConstructMovie(id++, "The Emoji Movie", 3.502m, 52));
			movies.Add(ConstructMovie(id++, "Girls Trip", 3.367m, 50));
			movies.Add(ConstructMovie(id++, "The Dark Tower", 3.232m, 48));
			movies.Add(ConstructMovie(id++, "The Glass Castle", 3.232m, 48));
			movies.Add(ConstructMovie(id++, "Wind River", 2.896m, 43));
			movies.Add(ConstructMovie(id++, "Kidnap", 2.559m, 38));
			movies.Add(ConstructMovie(id++, "Atomic Blonde", 2.289m, 34));
			movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 1.953m, 29));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 1.885m, 28));

			test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);

			Logger.WriteLine("\n==== Best Performer Disabled ====\n");

			test.EnableBestPerformer = false;

			best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_Todd_20170820()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

			movies.Add(ConstructMovie(id++, "The Hitman's Bodyguard", 16.7m, 254));
			movies.Add(ConstructMovie(id++, "Annabelle", 14.6m, 216));
			movies.Add(ConstructMovie(id++, "Logan Lucky", 13.2m, 184));
			movies.Add(ConstructMovie(id++, "Dunkirk", 7m, 98));
			movies.Add(ConstructMovie(id++, "The Nut Job 2", 4.4m, 62));
			movies.Add(ConstructMovie(id++, "Spider-Man", 3.95m, 58));
			movies.Add(ConstructMovie(id++, "The Emoji Movie", 3.5m, 52));
			movies.Add(ConstructMovie(id++, "Girls Trip", 3.5m, 50));
			movies.Add(ConstructMovie(id++, "The Dark Tower", 3.2m, 48));
			movies.Add(ConstructMovie(id++, "The Glass Castle", 3.15m, 48));
			movies.Add(ConstructMovie(id++, "Wind River", 3.1m, 43));
			movies.Add(ConstructMovie(id++, "Kidnap", 2.6m, 38));
			movies.Add(ConstructMovie(id++, "Atomic Blonde", 2.3m, 34));
			movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 1.95m, 29));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 1.8m, 28));

			test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);

			Logger.WriteLine("\n==== Best Performer Disabled ====\n");

			test.EnableBestPerformer = false;

			best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}
    }
}