using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MoviePicker.Common.Interfaces;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MineCulturedVulturesTests : MineTestBase
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

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineCulturedVultures_Mine()
		{
			var test = new MineCulturedVultures();

			var actual = test.Mine();

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Any(), "The list was empty.");

			WriteMovies(actual.OrderByDescending(item => item.Earnings));
		}

		[TestMethod, TestCategory(PRIMARY_TEST_CATEGORY)]
		public void MineCulturedVultures_ParseHoldovers()
		{
			string innerText = "2. Annabelle: Creation (Warner Bros.) – $15.8 million (-55%), $64.3m cume 4. Dunkirk (Warner Bros.) – $7.6 million (-30%), $166.3m cume 5. The Nut Job 2: Nutty by Nature (Open Road) – $4.4 million (-47%), $16.6m cume 6. Girls Trip (Universal) – $3.9 million (-40%), $103.8m cume 7. Spider-Man: Homecoming (Sony) – $3.7 million (-39%), $313.5m cume 8. The Dark Tower (Sony) – $3.3 million (-58%), $40.7m cume 9. The Emoji Movie (Sony) – $3.2 million (-51%), $69.9m cume 10. The Glass Castle (Lionsgate) – $2.7 million (-42%), $9.7m cume";

			// Match one or more digits, followed by a period and a space.
			// Gobble up (non-greedy using the ?) to 'million'
			var matches = Regex.Matches(innerText, @"\d+\.\s.*?million");

			foreach (Match match in matches)
			{
				var titleMatch = Regex.Match(match.Value, @"\s.*\s.\s\$");
				//var earningsMatch = Regex.Match(match.Value, @"\$\d+\smillion");
				var earningsMatch = Regex.Match(match.Value, @"\$\d+\.*\d*");

				if (!string.IsNullOrEmpty(earningsMatch.Value))
				{
					var earnings = Convert.ToDecimal(earningsMatch.Value.Replace("$", string.Empty));
				}

				if (!string.IsNullOrEmpty(titleMatch.Value))
				{
					var title = titleMatch.Value.Trim().Replace(" $", string.Empty);
				}

				var dbg = match;
			}
		}
	}
}