using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Tests;
using MoviePicker.WebApp.Models;
using MoviePicker.WebApp.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity;

using TopMoviePicker = MooveePicker.MoviePicker;

namespace MoviePicker.WebApp.Tests.Models
{
	[TestClass]
	public class ImageUtilityTests : MoviePickerTestBase
	{
		private const string TEST_CATEGORY = "Integration";

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ImageUtility>();
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void ImageUtility_CombineImages_8xAntman()
		{
			var cwd = Directory.GetCurrentDirectory() + "\\..\\..";
			var test = CreateTestObject();
			var files = new List<string>();

			for (int count = 0; count < 8; count++)
			{
				files.Add($"{cwd}\\Images\\TestPoster_antman_and_the_wasp_ver2.jpg");
			}

			var filePath = test.CombineImages(cwd, files);

			Assert.IsNotNull(filePath);
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void ImageUtility_CombineImages_8xSearching()
		{
			var cwd = Directory.GetCurrentDirectory() + "\\..\\..";
			var test = CreateTestObject();
			var files = new List<string>();

			for (int count = 0; count < 8; count++)
			{
				files.Add($"{cwd}\\Images\\TestPoster_searchingposter2.jpg");
			}

			var filePath = test.CombineImages(cwd, files);

			Assert.IsNotNull(filePath);
		}

		[TestMethod, TestCategory(TEST_CATEGORY)]
		public void ImageUtility_CombineImages_4xAntman4xSearching()
		{
			//TestPoster_searchingposter2.jpg

			var cwd = Directory.GetCurrentDirectory() + "\\..\\..";
			var test = CreateTestObject();
			var files = new List<string>();

			for (int count = 0; count < 4; count++)
			{
				files.Add($"{cwd}\\Images\\TestPoster_antman_and_the_wasp_ver2.jpg");
			}
			for (int count = 0; count < 4; count++)
			{
				files.Add($"{cwd}\\Images\\TestPoster_searchingposter2.jpg");
			}
			var filePath = test.CombineImages(cwd, files);

			Assert.IsNotNull(filePath);
		}

		private ImageUtility CreateTestObject()
		{
			return _unity.Resolve<ImageUtility>();
		}
	}
}