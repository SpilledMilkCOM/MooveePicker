using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MovieMiner.Browser
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			var web = new HtmlWeb();
			var doc = web.Load(@"C:\temp\MooveePickerData.html");        // Load main page.
			var movieLinks = doc.DocumentNode.SelectNodes("//body//div[contains(@href, '/movie/')]");
			
			if (movieLinks != null)
			{
				foreach (var movieNode in movieLinks)
				{
					Debug.WriteLine(movieNode.GetAttributeValue("href", null));
					Debug.WriteLine("=================================================================");

					//Browser.Navigate($@"https://cinemadraft.com{movieNode.GetAttributeValue("href", null)}");

					Browser.Navigate("https://cinemadraft.com/movie/297802");
					//Browser.Refresh(WebBrowserRefreshOption.Completely);

					break;
				}
			}
		}

		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
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