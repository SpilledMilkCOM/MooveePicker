using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Common.Interfaces;
using SM.Common.Tests;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;
using Unity.Injection;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineBoxOfficeMojoDailyTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
			_unity.RegisterType<IMiner, MineBoxOfficeMojoDaily>(new InjectionConstructor("marvel2019", new DateTime(2019, 4, 28)));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineBoxOfficeMojoDaily_Mine()
		{
			var test = ConstructTestObject();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		//----==== PRIVATE ====--------------------------------------------------------------------------

		private IMiner ConstructTestObject()
		{
			AddDefaultLogger();

			return UnityContainer.Resolve<IMiner>();
		}
	}
}