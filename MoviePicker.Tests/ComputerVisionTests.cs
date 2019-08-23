﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Cognitive;
using MoviePicker.Cognitive.Parameters;
using SM.Common.Interfaces;
using SM.Common.REST;
using SM.Common.REST.Interfaces;
using SM.Common.Tests;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Unity;
using Unity.Injection;

namespace MoviePicker.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	[DeploymentItem("app.config")]
	[DeploymentItem("appSettings.secret.config")]
	public class ComputerVisionTests : TestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		// Allows the base to access this static container
		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			var apiKey = ConfigurationManager.AppSettings["APIKey"];

			_unity = new UnityContainer();

			_unity.RegisterType<ICognitiveConfiguration, CognitiveConfiguration>();
			_unity.RegisterType<IComputerVision, ComputerVision>();
			_unity.RegisterType<IRestClient, RestClient>(new InjectionProperty("APIKey", apiKey) );
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_AnylizeTable()
		{
			var test = ConstructTestObject();

			var actual = test.Analyze("https://www.boxofficepro.com/wp-content/uploads/2019/03/Table-300x119.png");

			Assert.IsNotNull(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze()
		{
			var test = ConstructTestObject();

			var actual = test.Analyze("https://mooveepicker.com/Images/MoviePoster_p16311223_p_v12_ac.jpg");

			Assert.IsNotNull(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithCelebrityFaces()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Faces };
			var details = new List<Detail> { Detail.Celebrities };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360"
									, visualFeatures, details);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithCelebrityFaces_Dora()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Faces };
			var details = new List<Detail> { Detail.Celebrities };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/137736/standard/dora-and-the-lost-city-of-gold-2019-poster-2.jpg?1562688856"
									, visualFeatures, details);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithFaces()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Faces };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360", visualFeatures);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithAdultDescriptionFaces()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Adult, VisualFeature.Description, VisualFeature.Faces };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360", visualFeatures);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithTags()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Tags };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360"
							, visualFeatures);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_WithTagsSpanish()
		{
			var visualFeatures = new List<VisualFeature> { VisualFeature.Tags };
			var test = ConstructTestObject();

			var actual = test.Analyze("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360"
							, visualFeatures, language: Language.es);

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Describe()
		{
			var test = ConstructTestObject();

			var actual = test.Describe("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360");

			Assert.IsNotNull(actual);

			Logger.WriteLine(actual);
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private IComputerVision ConstructTestObject()
		{
			return _unity.Resolve<IComputerVision>();
		}
	}
}