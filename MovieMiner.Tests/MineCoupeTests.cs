﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	public class MineCoupeTests : MineTestBase
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
		public void MineCoupe_Mine()
		{
			var test = new MineCoupe();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			Logger.WriteLine("\n==== Coupe ====\n");
			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}
	}
}