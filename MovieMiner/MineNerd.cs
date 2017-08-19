using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using HtmlAgilityPack;		// Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;


namespace MovieMiner
{
	public class MineNerd : Miner
    {
		//private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups/MonCompare/MonCompare2017SummerWeek12.js";
		private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups";

		public MineNerd()
			: base(DEFAULT_URL)
	    {
	    }

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load(DEFAULT_URL);

			// Lookup XPATH to get the right node that matches.
			var node = doc.DocumentNode.SelectSingleNode("//body/script");

			return result;
		}

		public async override Task<List<IMovie>> MineAsync()
	    {
		    var result = new List<IMovie>();
		    var xml = await HttpRequestUtil.DownloadStringAsync(Url);
			string dataUrl = null;

			// Only match the "well formed" body.
			//var regEx = new Regex(@"((?:.(?!<\s*body[^>]*>))+.<\s*body[^>]*>)|(<\s*/\s*body\s*\>.+)");
			var regEx = new Regex(@"<body>*.</body>");

			var match = regEx.Match(xml);

			Debug.WriteLine(match.Value);

			var doc = new XmlDocument();

			doc.LoadXml(xml);

			var scriptElements = doc.GetElementsByTagName("script");

			foreach (XmlNode scriptNode in scriptElements)
			{
				var src = scriptNode.Attributes["src"].Value;

				if (src != null && src.StartsWith("./MonCompare/"))
				{
					dataUrl = $"{DEFAULT_URL}/{scriptNode.Attributes["src"].Value}";
				}
			}

			if (dataUrl != null)
			{
				var json = await HttpRequestUtil.DownloadStringAsync(dataUrl);
			}

			return result;
	    }
    }
}