using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBoxOfficeMojoHistoryTests : MineTestBase
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
		public void MineBoxOfficeMojoHistory_Mine()
		{
			var test = new MineBoxOfficeMojoHistory("alita");

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		//----==== PRIVATE ====--------------------------------------------------------------------------

		private List<IMovie> FilterMovies(List<IMovie> toFilter)
		{
			var fmlMiner = new MineFantasyMovieLeagueBoxOffice();
			var fmlMovies = fmlMiner.Mine();
			var result = new List<IMovie>();

			foreach (var movie in toFilter)
			{
				var fmlMovie = fmlMovies.FirstOrDefault(item => item.Equals(movie));

				if (fmlMovie != null)
				{
					movie.Cost = fmlMovie.Cost;
					result.Add(movie);
				}
			}

			return result;
		}
	}
}