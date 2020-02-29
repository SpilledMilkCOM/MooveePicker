using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Tests;
using MoviePicker.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity;

using TopMoviePicker = MooveePicker.MoviePicker;

namespace MoviePicker.WebApp.Tests.Models
{
	[TestClass]
	public class MinerModelTests : MoviePickerTestBase
	{
		private const int TODD_INDEX = 2;
		private const int MAX_MINERS = 6;

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
		public void MinerModel_ConstructWithData()
		{
			var test = ConstructTest();

			foreach (var miner in test.Miners)
			{
				Logger.WriteLine(string.Empty);
				Logger.WriteLine($"======== {miner.Name} ========");
				WriteMovies(miner.Movies);
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_BoxOfficeReportNumbers()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners[0].Movies.Count > 0);

			Logger.WriteLine("=========================== BO REPORT'S NUMBERS ===========================");

			foreach (var movie in test.Miners[7].Movies)
			{
				Logger.WriteLine($"{movie.Id}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_CoupesNumbers()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners[0].Movies.Count > 0);

			Logger.WriteLine("=========================== COUPE'S NUMBERS ===========================");

			foreach (var movie in test.Miners[5].Movies)
			{
				Logger.WriteLine($"{movie.Id}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_DownloadMoviePosters()
		{
			var test = ConstructTest();
			var cwd = Directory.GetCurrentDirectory() + "\\..\\..";

			Assert.IsNotNull(test.Miners);

			test.DownloadMoviePosters($"{cwd}\\Images\\");
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_CoupesNumbers_ExpireAndReload()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners[0].Movies.Count > 0);

			Logger.WriteLine("=========================== COUPE'S NUMBERS ===========================");

			foreach (var movie in test.Miners[5].Movies)
			{
				Logger.WriteLine($"{movie.Id}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}

			((ICache)test.Miners[5]).Expire();

			//((ICache)test.Miners[5]).Load();

			test = test.Clone() as MinerModel;

			Logger.WriteLine("=========================== COUPE'S NUMBERS ===========================");

			foreach (var movie in test.Miners[5].Movies)
			{
				Logger.WriteLine($"{movie.Id}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_FMLNumbers()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners.First().Movies.Count > 0);

			Logger.WriteLine("=========================== FML'S NUMBERS ===========================");

			foreach (var movie in test.Miners.First().Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_FMLNumbers_ExpireAndReload()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners.First().Movies.Count > 0);

			Logger.WriteLine("=========================== FML'S NUMBERS ===========================");

			foreach (var movie in test.Miners.First().Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}

			((ICache)test.Miners.First()).Expire();

			test = test.Clone() as MinerModel;

			Logger.WriteLine("=========================== FML'S NUMBERS ===========================");

			foreach (var movie in test.Miners.First().Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_MojosNumbers()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);

			var miner = test.Miners[MinerModel.MOJO_INDEX];

			Assert.IsTrue(miner.Movies.Count > 0);

			Logger.WriteLine("=========================== MOJO'S NUMBERS ===========================");

			Logger.WriteLine(test.WeekendEnding.ToString());

			foreach (var movie in miner.Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_MojosNumbers_ExpireAndReload()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);
			Assert.IsTrue(test.Miners[0].Movies.Count > 0);

			Logger.WriteLine("=========================== MOJO'S NUMBERS ===========================");

			foreach (var movie in test.Miners.Last().Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}

			((ICache)test.Miners.Last()).Expire();

			test = test.Clone() as MinerModel;

			Logger.WriteLine("=========================== MOJO'S NUMBERS ===========================");

			foreach (var movie in test.Miners.Last().Movies)
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase}, {movie.Cost} BUX");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_To10pMoviePicker()
		{
			var test = ConstructTest();
			var moviePicker = new TopMoviePicker(new MovieList());

			var sw = new Stopwatch();

			sw.Start();

			moviePicker.AddMovies(test.Miners[TODD_INDEX].Movies);

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
		public void MinerModel_Simulation()
		{
			var test = ConstructTest();

			SetWeights(test, CreateDefaultWeights());

			var myPicks = test.CreateWeightedList();

			foreach (var movie in myPicks)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_TheaterEfficiency()
		{
			var test = ConstructTest();

			Assert.IsNotNull(test.Miners);

			SetWeights(test, CreateDefaultWeights());

			var myPicks = test.CreateWeightedList();

			Assert.IsTrue(myPicks.Count > 0);

			Logger.WriteLine("=========================== MOJO'S NUMBERS ===========================");

			foreach (var movie in myPicks.OrderByDescending(item => item.EarningsBase / item.TheaterCount))
			{
				Logger.WriteLine($"{movie.Id}  {movie.WeekendEnding}  \"{movie.Name}\", ${movie.EarningsBase:N0}, {movie.Cost} BUX, IN {movie.TheaterCount} ${movie.EarningsBase / movie.TheaterCount:N2} / Theater");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_WriteCodedOutput()
		{
			var test = ConstructTest();

			SetWeights(test, CreateDefaultWeights());

			var myPicks = test.CreateWeightedList();

			foreach (var movie in myPicks)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_WriteCodedOutput_ToddThatcher()
		{
			var test = ConstructTest();

			foreach (var movie in test.Miners[TODD_INDEX].Movies)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}

		//----==== PRIVATE ====---------------------------------------------------------------------------

		private MinerModel ConstructTest()
		{
			return new MinerModel(true, null);
		}

		private List<int> CreateDefaultWeights()
		{
			return new List<int>
			{
				3,			// Todd Thatcher
				3,			// Box Office Pro
				4,			// Box Office Mojo
				3,			// Coupe (was Cultured Vultures)
				1,			// Box Office Prophet
				6			// Box Office Report
			};
		}

		private void SetWeights(MinerModel model, List<int> weights)
		{
			if (weights.Count != MAX_MINERS)
			{
				throw new ArgumentException($"The weight list MUST contain {MAX_MINERS} weights.");
			}

			int index = TODD_INDEX;

			foreach (var weight in weights)
			{
				model.Miners[index++].Weight = weight;
			}
		}
	}
}