﻿using SM.Common.Interfaces;
using MoviePicker.Tests;
using SM.Common.Utils;
using System;
using Unity.Lifetime;

namespace TestRunner
{
	public class TestHarness
	{
		public void Invoke()
		{
			var logger = new StandardOutLogger();           // Uses Console
			ElapsedTime elapsed = new ElapsedTime();

			logger.WriteLine("Constructing test fixture...\n");

			var fixture = new MoviePickerVariantsAllTests();

			logger.WriteLine(elapsed.Elapsed.ToString());

			logger.ForegroundColor = ConsoleColor.Green;
			logger.WriteLine("Setting up test fixture...\n");

			MoviePickerVariantsAllTests.InitializeBeforeAllTests(null);          //<<<<< Static class needs to match constructor above.

			// Override the debug logger inside of the tests.

			fixture.UnityContainer.RegisterInstance(typeof(ILogger), null, logger, new ContainerControlledLifetimeManager());

			logger.WriteLine($"AFTER - InitializeBeforeAllTests {elapsed.Elapsed}");

			fixture.MoviePickerVariantsAll_ChooseBest_Parker_20180805_Percentage();               //<<<<< Test method.

			logger.WriteLine($"AFTER TEST - {elapsed.ElapsedSinceLastElapsed}");
			logger.WriteLine($"AFTER - CleanupAfterAllTests {elapsed.Elapsed}");
			logger.WriteLine("Press any key to continue...");

			Console.ReadKey();
		}
	}
}