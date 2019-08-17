using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Cognitive;
using SM.Common.REST;
using SM.Common.REST.Interfaces;
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

			var actual = test.AnalyzePoster("https://mooveepicker.com/Images/MoviePoster_p16311223_p_v12_ac.jpg");

			Assert.IsNotNull(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void PosterRecognition_DescribePoster()
		{
			var test = ConstructTestObject();

			var actual = test.DescribePoster("https://images.noovie.com/posters/movies/124620/standard/fast-furious-presents-hobbs-shaw-2019-poster-2.jpg?1561742360");

			Assert.IsNotNull(actual);
		}

		[TestMethod, TestCategory("Integration")]
		public void PosterRecognition_AnylizeTable()
		{
			var test = ConstructTestObject();

			var actual = test.AnalyzePoster("https://www.boxofficepro.com/wp-content/uploads/2019/03/Table-300x119.png");

			Assert.IsNotNull(actual);
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private IPosterRecognition ConstructTestObject()
		{
			return _unity.Resolve<IPosterRecognition>();
		}
	}
}