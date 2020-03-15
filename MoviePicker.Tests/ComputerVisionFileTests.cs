using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Cognitive;
using MoviePicker.Cognitive.Parameters;
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
	public class ComputerVisionFileTests : TestBase
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
			_unity.RegisterType<IRestClient, RestClient>(new InjectionProperty("APIKey", apiKey));
		}

		[TestMethod, TestCategory("Integration")]
		public void ComputerVision_Analyze()
		{
			var test = ConstructTestObject();

			//var actual = test.Analyze(TEST_POSTER_SINGLE_FACE);

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