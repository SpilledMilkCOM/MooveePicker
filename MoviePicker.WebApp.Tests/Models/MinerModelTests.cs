using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Tests;
using MoviePicker.WebApp.Models;
using System.Diagnostics;
using Unity;

using TopMoviePicker = MooveePicker.MoviePicker;

namespace MoviePicker.WebApp.Tests.Models
{
	[TestClass]
	public class MinerModelTests : MoviePickerTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			//_unity.RegisterType<IMovie, Movie>();
			//_unity.RegisterType<IMovieList, MovieList>();
			//_unity.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_TopMoviePicker()
		{
			var test = new MinerModel(true);
			var moviePicker = new TopMoviePicker(new MovieList());

			var sw = new Stopwatch();

			sw.Start();

			moviePicker.AddMovies(test.Miners[2].Movies);

			var movieLists = moviePicker.ChooseBest(10);

			sw.Stop();

			Logger.WriteLine($"Total milliseconds: {sw.ElapsedMilliseconds:N}");

			Logger.WriteLine("\n==== BONUS ON PICKS ====\n");

			WritePicker(moviePicker);

			foreach (var movieList in movieLists)
			{
				Logger.WriteLine(string.Empty);
				WriteMovies(movieList);
			}
		}


		[TestMethod, TestCategory("Integration")]
		public void MinerModel_WriteCodedOutput()
		{
			var test = new MinerModel(true);

			foreach (var movie in test.Miners[2].Movies)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}
	}
}