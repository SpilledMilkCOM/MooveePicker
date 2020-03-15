using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SM.Common.Tests;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using Unity;
using Unity.Injection;

namespace MoviePicker.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	[DeploymentItem("app.config")]
	[DeploymentItem("appSettings.secret.config")]
	public class ComputerVisionClientTests : TestBase
	{
		private const string IMAGES_FOLDER = "Images";

		private static string _cwd;         // Current Working Directory

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		// ComputerVision Reference: 

		// Allows the base to access this static container
		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			var apiKey = ConfigurationManager.AppSettings["APIKey"];
			var disposeHttpClient = false;       // Can't run all tests if the client disposes of the HttpClient

			_unity = new UnityContainer();

			_unity.RegisterType<IComputerVisionClient, ComputerVisionClient>(
						  new InjectionConstructor(new ApiKeyServiceClientCredentials(apiKey), new HttpClient(), disposeHttpClient)
						, new InjectionProperty("Endpoint", "https://southcentralus.api.cognitive.microsoft.com"));

			_cwd = Directory.GetCurrentDirectory() + $"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}MoviePicker.Tests";
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_Dora()
		{
			using (var test = ConstructTestObject())
			{
				using (var stream = new FileStream($"{ImagesFolder}MoviePoster_dora-and-the-lost-city-of-gold-2019-poster-2.temp.jpg", FileMode.Open))
				{
					var visualFeatures = new List<VisualFeatureTypes>() { VisualFeatureTypes.Adult, VisualFeatureTypes.Description, VisualFeatureTypes.Faces };
					var details = new List<Details> { Details.Celebrities };

					var imageAnalysisTask = test.AnalyzeImageInStreamAsync(stream, visualFeatures, details);

					// Maybe write out entire object as JSON.

					imageAnalysisTask.Wait();

					Logger.WriteLine(JsonConvert.SerializeObject(imageAnalysisTask.Result));
				}
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_FandF()
		{
			using (var test = ConstructTestObject())
			{
				using (var stream = new FileStream($"{ImagesFolder}MoviePoster_fast-furious-presents-hobbs-shaw-2019-poster-2.temp.jpg", FileMode.Open))
				{
					var visualFeatures = new List<VisualFeatureTypes>() { VisualFeatureTypes.Adult, VisualFeatureTypes.Description, VisualFeatureTypes.Faces };
					var details = new List<Details> { Details.Celebrities };

					var imageAnalysisTask = test.AnalyzeImageInStreamAsync(stream, visualFeatures, details);

					// Maybe write out entire object as JSON.

					imageAnalysisTask.Wait();

					Logger.WriteLine(JsonConvert.SerializeObject(imageAnalysisTask.Result));
				}
			}
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze_OnceUponATime()
		{
			using (var test = ConstructTestObject())
			{
				using (var stream = new FileStream($"{ImagesFolder}MoviePoster_once-upon-a-time-in-hollywood_v3.temp.jpg", FileMode.Open))
				{
					var visualFeatures = new List<VisualFeatureTypes>() { VisualFeatureTypes.Adult, VisualFeatureTypes.Description, VisualFeatureTypes.Faces};
					var details = new List<Details> { Details.Celebrities };

					var imageAnalysisTask = test.AnalyzeImageInStreamAsync(stream, visualFeatures, details);

					// Maybe write out entire object as JSON.

					imageAnalysisTask.Wait();

					Logger.WriteLine(JsonConvert.SerializeObject(imageAnalysisTask.Result));
				}
			}
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private string ImagesFolder => $"{_cwd}{Path.DirectorySeparatorChar}{IMAGES_FOLDER}{Path.DirectorySeparatorChar}";

		private IComputerVisionClient ConstructTestObject()
		{
			return _unity.Resolve<IComputerVisionClient>();
		}
	}
}