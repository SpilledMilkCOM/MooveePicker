using System;
using System.Diagnostics;
using System.Threading;
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
			Browser.Navigate("http://analyzer.fmlnerd.com/lineups/");
		}

		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var text = Browser.DocumentText;

			//Debug.WriteLine(text);

			//Thread.Sleep(5000);

			Debug.WriteLine(Browser.Document.Body.InnerHtml);

			var button = Browser.Document.GetElementById("bpToggle");

			var table = Browser.Document.GetElementById("BPtableBody");

			var rows = table.GetElementsByTagName("tr");

			//		XmlNode table = doc.SelectSingleNode("//table[@class='tableType-group hasGroups']");

			//		if (table == null)
			//		{
			//			table = doc.SelectSingleNode("//table[@class='tableType-group noGroups']");
			//		}

			//XmlNodeList items = table.SelectNodes("//td[@class='movie-title movie-title--with-cost-and-image']");

			//		if (items == null)
			//		{
			//			items = table.SelectNodes("//td[@class='movie-title movie-title--with-cost']");
			//		} 


		}
	}
}