using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MooviePicker.Tests;
using MoviePicker.Tests;
using SM.Common.Utils;

namespace TestRunner
{
	public class TestHarness
	{
		public void Invoke()
		{
			ElapsedTime elapsed = new ElapsedTime();

			Console.WriteLine("Constructing test fixture...\n");

			var fixture = new MoviePickerVariantsAllTests();

			Console.WriteLine(elapsed.Elapsed);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Setting up test fixture...\n");

			MoviePickerVariantsAllTests.InitializeBeforeAllTests(null);          //<<<<< Static class needs to match constructor above.

			Console.WriteLine("AFTER - InitializeBeforeAllTests {0}", elapsed.Elapsed);

			fixture.MoviePickerVariantsAll_ChooseBest_Parker_20170813();               //<<<<< Test method.

			Console.WriteLine("AFTER TEST - MoviePickerVariantsAll_ChooseBest_Parker_20170813 - {0}", elapsed.ElapsedSinceLastElapsed);

			//MoviePickerTest_20170813.CleanupAfterAllTests();

			Console.WriteLine("AFTER - CleanupAfterAllTests {0}", elapsed.Elapsed);

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		private void InvokeAllTestMethodsSynchronous(object testFixture)
		{
			MethodInfo[] methods = testFixture.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

			foreach (MethodInfo methodInfo in methods)
			{
				TestMethodAttribute[] testMethodAttributes = methodInfo.GetCustomAttributes(typeof(TestMethodAttribute), false) as TestMethodAttribute[];
				IgnoreAttribute[] ignoreAttributes = methodInfo.GetCustomAttributes(typeof(IgnoreAttribute), false) as IgnoreAttribute[];
				ConsoleColor saveColor = Console.ForegroundColor;

				if (ignoreAttributes != null && ignoreAttributes.Length > 0)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("\nSKIPPED test method '{0}'...\n", methodInfo.Name);
				}
				else if (testMethodAttributes != null && testMethodAttributes.Length > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("\nExecuting test method '{0}'...\n", methodInfo.Name);

					try
					{
						methodInfo.Invoke(testFixture, null);
					}
					catch (Exception ex)
					{
						Console.ForegroundColor = ConsoleColor.Red;

						if (ex.InnerException != null && ex.InnerException is AssertFailedException)
						{
							Console.WriteLine("\nAssert failed {0}\n", ex.InnerException.Message);
						}
						else
						{
							throw ex;
						}
					}
				}

				Console.ForegroundColor = saveColor;
			}
		}
	}
}