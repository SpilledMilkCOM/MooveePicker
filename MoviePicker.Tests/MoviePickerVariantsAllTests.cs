using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		public void MoviePickerVariantsAll_ChooseBest_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMoviesPicks());

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);
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

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void MoviePickerVariantsAll_ChooseBest_Parker_20180805_Percentage()
		{
			var test = ConstructTestObject();

			// By default two adjustments are made (+/- 3%)
			// The setting below will allow for 6 adjustments and one baseline.

			((MoviePickerVariantsAll)test).EarningsAdjustment = 0.01m;
			((MoviePickerVariantsAll)test).EarningsAdjustmentMax = 0.03m;

			test.AddMovies(ConstructMovieList_20180805());

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

			movies.Add(ConstructMovie(id++, "Mission Impossible  Fallout", 62.529411764705882352941176471m, 756));
			movies.Add(ConstructMovie(id++, "Mamma Mia Here We Go Again", 19.194117647058823529411764706m, 227));
			movies.Add(ConstructMovie(id++, "The Equalizer 2", 16.582352941176470588235294118m, 201));
			movies.Add(ConstructMovie(id++, "Hotel Transylvania 3 Summer Vacation", 13.494117647058823529411764706m, 157));
			movies.Add(ConstructMovie(id++, "Teen Titans GO to the Movies", 15.064705882352941176470588235m, 149));
			movies.Add(ConstructMovie(id++, "AntMan  the Wasp", 9.178571428571428571428571429m, 116));
			movies.Add(ConstructMovie(id++, "Incredibles 2", 8.057142857142857142857142857m, 89));
			movies.Add(ConstructMovie(id++, "Jurassic World Fallen Kingdom", 6.95m, 76));
			movies.Add(ConstructMovie(id++, "Skyscraper", 5.5857142857142857142857142857m, 61));
			movies.Add(ConstructMovie(id++, "Blindspotting", 2.3333333333333333333333333333m, 41));
			movies.Add(ConstructMovie(id++, "The First Purge", 2.3857142857142857142857142857m, 28));
			movies.Add(ConstructMovie(id++, "Eighth Grade", 0.946m, 26));
			movies.Add(ConstructMovie(id++, "Unfriended Dark Web", 1.6m, 18));
			movies.Add(ConstructMovie(id++, "Sorry to Bother You", 1.8m, 18));
			movies.Add(ConstructMovie(id++, "Three Identical Strangers", 1.1m, 16));

			IgnoreMovies(movies);
			IncludeMoviesByEfficiency(movies, 6);
			IncludeMoviesByBoxOffice(movies, 1);

			return movies;
		}

		private List<IMovie> ConstructMovieList_20180805()
		{
			var movies = new List<IMovie>();
			int id = 1;

			// Movie list generated from MinerModelTests.MinerModel_WriteCodedOutput

			movies.Add(ConstructMovie(id++, "Christopher Robin", 30.458823529411764705882352941m, 513));
			movies.Add(ConstructMovie(id++, "Mission Impossible  Fallout", 31.7m, 512));
			movies.Add(ConstructMovie(id++, "The Spy Who Dumped Me", 13.741176470588235294117647059m, 268));
			movies.Add(ConstructMovie(id++, "The Darkest Minds", 7.4882352941176470588235294118m, 154));
			movies.Add(ConstructMovie(id++, "Mamma Mia Here We Go Again", 8.541176470588235294117647059m, 136));
			movies.Add(ConstructMovie(id++, "Hotel Transylvania 3 Summer Vacation", 6.8142857142857142857142857143m, 131));
			movies.Add(ConstructMovie(id++, "The Equalizer 2", 7.0928571428571428571428571429m, 126));
			movies.Add(ConstructMovie(id++, "Teen Titans GO to the Movies", 5.3357142857142857142857142857m, 93));
			movies.Add(ConstructMovie(id++, "AntMan  the Wasp", 5.3769230769230769230769230769m, 88));
			movies.Add(ConstructMovie(id++, "Incredibles 2", 4.7714285714285714285714285714m, 76));
			movies.Add(ConstructMovie(id++, "Jurassic World Fallen Kingdom", 4.2m, 70));
			movies.Add(ConstructMovie(id++, "Death of a Nation", 2.7333333333333333333333333333m, 55));
			movies.Add(ConstructMovie(id++, "Skyscraper", 2.3m, 45));
			movies.Add(ConstructMovie(id++, "Eighth Grade", 2.37m, 32));
			movies.Add(ConstructMovie(id++, "The First Purge", 0.954m, 16));

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