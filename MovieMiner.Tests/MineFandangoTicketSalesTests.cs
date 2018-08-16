using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineFandangoTicketSalesTests : MineTestBase
	{
		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<ILogger, DebugLogger>();
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_Mine()
		{
			// http://boxofficereport.com/

			var test = new MineFandangoTicketSales();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY), TestCategory("Single")]
		public void MineFandangoTicketSales_Totals()
		{
			// http://boxofficereport.com/

			var test = new MineFandangoTicketSales();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			actual = actual.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			WriteMovies(actual);
		}
	}
}