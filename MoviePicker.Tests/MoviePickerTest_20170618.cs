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
    public class MoviePickerTest_20170618 : MoviePickerTestBase
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

			movies.Add(ConstructMovie(id++, "Cars 3", 60m, 719));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 35m, 478));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 23m, 327));
			movies.Add(ConstructMovie(id++, "Rough Night", 16m, 243));
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

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
		}
	}
}