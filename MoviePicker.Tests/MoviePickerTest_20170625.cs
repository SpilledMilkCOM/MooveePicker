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
    public class MoviePickerTest_20170625 : MoviePickerTestBase
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
		public void MoviePicker_ChooseBest_Parker_20170618()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

            movies.Add(ConstructMovie(id++, "Transformers", 68.4m, 560));
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

            test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}

        [TestMethod]
        public void MoviePicker_ChooseBest_Average_NerdGuru_Todd_20170618()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Transformers", 68.4m, 560));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 31.675m, 286));
            movies.Add(ConstructMovie(id++, "Cars 3", 32.375m, 278));
            movies.Add(ConstructMovie(id++, "All Eyez on Me", 10.7m, 104));
            movies.Add(ConstructMovie(id++, "The Mummy", 8.2625m, 73));
            movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 6.025m, 58));
            movies.Add(ConstructMovie(id++, "47 Meters Down", 6.1375m, 55));
            movies.Add(ConstructMovie(id++, "Captain Underpants", 4.8625m, 45));
            movies.Add(ConstructMovie(id++, "Rough Night", 4.3875m, 39));
            movies.Add(ConstructMovie(id++, "Tubelight", 3.125m, 34));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 3.55m, 32));
            movies.Add(ConstructMovie(id++, "Beatriz at Dinner", 1.7125m, 17));
            movies.Add(ConstructMovie(id++, "Megan Leavey", 1.65m, 16));
            movies.Add(ConstructMovie(id++, "It Comes at night", 1.3625m, 13));
            movies.Add(ConstructMovie(id++, "The Book of Henry", 1.091m, 11));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }

        [TestMethod]
        public void MoviePicker_ChooseBest_NerdGuru_20170618()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Transformers", 70m, 560));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 35.75m, 286));
            movies.Add(ConstructMovie(id++, "Cars 3", 34.75m, 278));
            movies.Add(ConstructMovie(id++, "All Eyez on Me", 13m, 104));
            movies.Add(ConstructMovie(id++, "The Mummy", 9.125m, 73));
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

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }

        [TestMethod]
        public void MoviePicker_ChooseBest_Todd_20170618()
        {
            var test = ConstructTestObject();
            var movies = new List<IMovie>();
            int id = 1;

            movies.Add(ConstructMovie(id++, "Transformers", 66.8m, 560));
            movies.Add(ConstructMovie(id++, "Wonder Woman", 27.6m, 286));
            movies.Add(ConstructMovie(id++, "Cars 3", 30m, 278));
            movies.Add(ConstructMovie(id++, "All Eyez on Me", 8.4m, 104));
            movies.Add(ConstructMovie(id++, "The Mummy", 7.4m, 73));
            movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 4.8m, 58));
            movies.Add(ConstructMovie(id++, "47 Meters Down", 5.4m, 55));
            movies.Add(ConstructMovie(id++, "Captain Underpants", 4.1m, 45));
            movies.Add(ConstructMovie(id++, "Rough Night", 3.9m, 39));
            movies.Add(ConstructMovie(id++, "Tubelight", 2m, 34));
            movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 3.1m, 32));
            movies.Add(ConstructMovie(id++, "Beatriz at Dinner", 1.3m, 17));
            movies.Add(ConstructMovie(id++, "Megan Leavey", 1.3m, 16));
            movies.Add(ConstructMovie(id++, "It Comes at night", 1.1m, 13));
            movies.Add(ConstructMovie(id++, "The Book of Henry", 0.807m, 11));

            test.AddMovies(movies);

            var best = test.ChooseBest();

            WritePicker(test);
            WriteMovies(best);
        }
    }
}