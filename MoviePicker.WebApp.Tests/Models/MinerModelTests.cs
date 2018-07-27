﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		private const int TODD_INDEX = 2;

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
		public void MinerModel_ConstructWithDAta()
		{
			var test = new MinerModel(true);

			foreach (var miner in test.Miners)
			{
				Logger.WriteLine(string.Empty);
				Logger.WriteLine($"======== {miner.Name} ========");
				WriteMovies(miner.Movies);
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_To10pMoviePicker()
		{
			var test = new MinerModel(true);
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
		public void MinerModel_WriteCodedOutput()
		{
			var test = new MinerModel(true);

			test.Miners[TODD_INDEX].Weight = 3;
			test.Miners[3].Weight = 3;              // Box Office Pro
			test.Miners[4].Weight = 4;              // Box Office Mojo
			test.Miners[5].Weight = 1;              // Cultured Vultures
			test.Miners[6].Weight = 1;              // Box Office Prophet
			test.Miners[7].Weight = 6;              // Box Office Report

			var myPicks = test.CreateWeightedList();

			foreach (var movie in myPicks)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void MinerModel_WriteCodedOutput_ToddThatcher()
		{
			var test = new MinerModel(true);

			foreach (var movie in test.Miners[TODD_INDEX].Movies)
			{
				Logger.WriteLine($"movies.Add(ConstructMovie(id++, \"{movie.Name}\", {movie.EarningsBase / 1000000m}m, {movie.Cost}));");
			}
		}
	}
}