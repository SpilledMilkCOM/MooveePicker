﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
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
			var test = new MineToddThatcher();
			var moviePicker = new TopMoviePicker(new MovieList());

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Todd M Thatcher ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));

			moviePicker.AddMovies(actual);

			var movieLists = moviePicker.ChooseBest(10);

			Logger.WriteLine("\n==== BONUS ON PICKS ====\n");

			foreach (var movieList in movieLists)
			{
				WriteMovies(movieList.Movies);
			}
		}
	}
}