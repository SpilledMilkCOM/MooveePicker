using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineFandangoTicketSalesTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
			_unity.RegisterType<IMoviePicker, MsfMovieSolver>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_Mine()
		{
			var test = new MineFandangoTicketSales();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_Totals()
		{
			var test = new MineFandangoTicketSales();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			actual = actual.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.Where(movie => movie.Earnings > 1000)
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			WriteMovies(actual);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_TotalsCompressed()
		{
			var test = new MineFandangoTicketSales();
			var fmlMiner = new MineFantasyMovieLeagueBoxOffice();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			var gameMovies = fmlMiner.Mine();

			Assert.IsNotNull(gameMovies);
			Assert.IsTrue(gameMovies.Any(), "The game list was empty.");

			actual = actual.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			actual = actual.Where(movie => gameMovies.Contains(movie)).ToList();

			var copy = new List<IMovie>(actual);
			var toRemove = new List<IMovie>();

			foreach (var movie in actual)
			{
				// Find a simlarly named movie

				var likeMovie = copy.FirstOrDefault(item => item.Equals(movie) && item.MovieName != movie.MovieName);

				if (likeMovie != null && !toRemove.Contains(likeMovie))
				{
					// Add the totals.

					movie.Earnings += likeMovie.Earnings;

					// Remove the movie so it can't be found again.

					RemoveSameName(copy, likeMovie);
					toRemove.Add(likeMovie);
				}
			}

			foreach (var movie in toRemove)
			{
				var found = actual.FirstOrDefault(item => item.MovieName == movie.MovieName);

				if (found != null)
				{
					RemoveSameName(actual, found);
				}
			}

			// Assign the cost for WriteMovies to compute Efficiency.

			var now = DateTime.Now;

			foreach (var movie in actual)
			{
				var found = gameMovies.FirstOrDefault(item => item.Equals(movie));

				if (found != null)
				{
					movie.Cost = found.Cost;
					movie.WeekendEnding = now;
				}
			}

			WriteMovies(actual, true);
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_TotalsCompressed_MakePicks()
		{
			var test = new MineFandangoTicketSales();
			var fmlMiner = new MineFantasyMovieLeagueBoxOffice();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			var gameMovies = fmlMiner.Mine();

			Assert.IsNotNull(gameMovies);
			Assert.IsTrue(gameMovies.Any(), "The game list was empty.");

			actual = actual.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			actual = actual.Where(movie => gameMovies.Contains(movie)).ToList();

			var copy = new List<IMovie>(actual);
			var toRemove = new List<IMovie>();

			foreach (var movie in actual)
			{
				// Find a simlarly named movie

				var likeMovie = copy.FirstOrDefault(item => item.Equals(movie) && item.MovieName != movie.MovieName);

				if (likeMovie != null && !toRemove.Contains(likeMovie))
				{
					// Add the totals.

					movie.Earnings += likeMovie.Earnings;

					// Remove the movie so it can't be found again.

					RemoveSameName(copy, likeMovie);
					toRemove.Add(likeMovie);
				}
			}

			foreach (var movie in toRemove)
			{
				var found = actual.FirstOrDefault(item => item.MovieName == movie.MovieName);

				if (found != null)
				{
					RemoveSameName(actual, found);
				}
			}

			// Assign the cost for WriteMovies to compute Efficiency.

			var now = DateTime.Now;

			foreach (var movie in actual)
			{
				var found = gameMovies.FirstOrDefault(item => item.Equals(movie));

				if (found != null)
				{
					movie.Cost = found.Cost;
					movie.WeekendEnding = now;
				}
			}

			WriteMovies(actual, true);

			var moviePicker = _unity.Resolve<IMoviePicker>();

			moviePicker.AddMovies(actual);
			//moviePicker.EnableBestPerformer = false;

			var bonusPicks = moviePicker.ChooseBest();

			WritePicker(moviePicker);
			WriteMovies(bonusPicks);
		}

		private void RemoveSameName(List<IMovie> list, IMovie toRemove)
		{
			var indexToRemove = 0;

			foreach (var movie in list)
			{
				if (movie.MovieName == toRemove.MovieName)
				{
					break;
				}

				indexToRemove++;
			}

			if (indexToRemove < list.Count)
			{
				list.RemoveAt(indexToRemove);
			}
		}
	}
}