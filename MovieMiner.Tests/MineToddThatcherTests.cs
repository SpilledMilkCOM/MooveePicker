using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

using TopMoviePicker = MooveePicker.MoviePicker;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineToddThatcherTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineToddThatcher_Mine()
		{
			var test = new MineToddThatcher();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Todd M Thatcher ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineToddThatcher_MineAndSolve()
		{
			var fmlMiner = new MineFantasyMovieLeagueBoxOffice();
			var test = new MineToddThatcher();
			var moviePicker = new TopMoviePicker(new MovieList());

			var fmlMovies = fmlMiner.Mine();
			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			AssignMovies(fmlMovies, actual);

			Logger.WriteLine("\n==== Todd M Thatcher ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));

			moviePicker.AddMovies(actual);

			var movieLists = moviePicker.ChooseBest(10);

			Logger.WriteLine("\n==== BONUS ON PICKS ====\n");

			foreach (var movieList in movieLists)
			{
				WriteMovies(movieList.Movies);
				Logger.WriteLine(string.Empty);
			}
		}

		private void AssignMovies(List<IMovie> fmlMovies, List<IMovie> actual)
		{
			foreach (var movie in fmlMovies)
			{
				var found = actual.First(item => item.Equals(movie));

				if (found != null)
				{
					found.Name = movie.Name;
					found.Id = movie.Id;
					found.Cost = movie.Cost;
				}
			}
		}
	}
}