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
	public class TextRecognitionTests
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			var apiKey = ConfigurationManager.AppSettings["APIKey"];

			_unity = new UnityContainer();

			_unity.RegisterType<ICognitiveConfiguration, CognitiveConfiguration>();
			_unity.RegisterType<ITextRecognition, TextRecognition>();
			_unity.RegisterType<IRestClient, RestClient>(new InjectionProperty("APIKey", apiKey) );
		}

		[TestMethod, TestCategory("Integration")]
		public void TextRecognition_AnylizeText()
		{
			var test = ConstructTestObject();

			var actual = test.AnalyzeText("https://www.boxofficepro.com/wp-content/uploads/2019/03/Table-300x119.png");

			Assert.IsNotNull(actual);
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private ITextRecognition ConstructTestObject()
		{
			return _unity.Resolve<ITextRecognition>();
		}
	}
}