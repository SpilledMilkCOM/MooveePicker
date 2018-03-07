using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MovieMiner.Tests
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MRCMinerTests
	{
		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
		}

		[TestMethod, TestCategory("Integration")]
		public void MineMRCData_Mine()
		{
			var web = new HtmlWeb();
			var doc = web.Load(@"C:\temp\MooveePickerData.html");        // Load main page.
			var movieLinks = doc.DocumentNode.SelectNodes("//body//div[contains(@href, '/movie/')]");

			if (movieLinks != null)
			{
				var webBrowser = new WebBrowser();

				webBrowser.Show();
				webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;

				foreach (var movieNode in movieLinks)
				{
					Debug.WriteLine(movieNode.GetAttributeValue("href", null));
					Debug.WriteLine("=================================================================");

					webBrowser.Navigate($@"https://cinemadraft.com{movieNode.GetAttributeValue("href", null)}");

					break;
				}
			}
		}

		private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			// Parse the data

			var webBrowser = sender as WebBrowser;

			if (webBrowser != null)
			{
				var infoNode = webBrowser.Document;

				Debug.WriteLine($"Navigated to: {webBrowser.Url}");

				//infoNode = doc.DocumentNode.SelectSingleNode("//body//div[contains(@ng-bind, '::$ctrl.movie.overview')]");

				//Debug.WriteLine(infoNode?.InnerText);
			}
		}
	}
}