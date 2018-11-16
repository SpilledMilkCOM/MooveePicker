using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Cognitive;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Unity;
using Unity.Injection;

namespace MoviePicker.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	[DeploymentItem("app.secret.config")]
	public class PosterRecognitionTests
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			var apiKey = ConfigurationManager.AppSettings["APIKey"];

			_unity = new UnityContainer();

			_unity.RegisterType<ICognitiveConfiguration, CognitiveConfiguration>();
			_unity.RegisterType<IPosterRecognition, PosterRecognition>();
			_unity.RegisterType<IRestClient, RestClient>(new InjectionProperty("APIKey", apiKey) );
		}

		[TestMethod, TestCategory("Integration")]
		public void PosterRecognition_AnylizePoster()
		{
			var test = ConstructTestObject();

			var actual = test.AnalyzePoster("http://mooveepicker.azurewebsites.net/images/MoviePoster_86067c35-d745-4643-abd8-3f348cc9e817.jpg");

			Assert.IsNotNull(actual);
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private IPosterRecognition ConstructTestObject()
		{
			return _unity.Resolve<IPosterRecognition>();
		}
	}
}