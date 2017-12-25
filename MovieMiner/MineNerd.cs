using System;
using System.Collections.Generic;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML
using Newtonsoft.Json;

using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public class MineNerd : MinerBase
	{
		//private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups/MonCompare/MonCompare2017SummerWeek12.js";
		private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups";
		private const int DAY_KEY_LENGTH = 3;

		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		public MineNerd()
			: base("FML Nerd (Pete Johnson)", "FML Nerd", DEFAULT_URL)
		{
			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				//{"FRI ", DayOfWeek.Friday},
				//{"SAT ", DayOfWeek.Saturday},
				//{"SUN ", DayOfWeek.Sunday}
				{"FRI", DayOfWeek.Friday},
				{"SAT", DayOfWeek.Saturday},
				{"SUN", DayOfWeek.Sunday}
			};

			UrlSource = DEFAULT_URL;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();
			var doc = web.Load(DEFAULT_URL);

			// TODO: Somehow parse the page title from "Summer Week 13" into a Sunday date for each movie.

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			//var node = doc.DocumentNode.SelectSingleNode("body/script[@src='*/MonCompare*']");
			var node = doc.DocumentNode.SelectSingleNode("//body/script[contains(@src, 'MonCompare')]");

			if (node != null)
			{
				var src = node.GetAttributeValue("src", null);

				if (src != null)
				{
					// Now retrieve the JSON (.js) page/file.

					//doc = web.Load($"{DEFAULT_URL}/{src}");

					var jsonData = HttpRequestUtil.DownloadString($"{DEFAULT_URL}/{src}");

					// The string is not really JSON, but CLOSE
					// Might want to use Regex to change this.

					jsonData = jsonData.Replace("year =", "\"year\":");
					jsonData = jsonData.Replace("season =", "\"season\":");
					jsonData = jsonData.Replace("week =", "\"week\":");
					jsonData = jsonData.Replace("movies=", "\"movies\":");
					// Adjust the "JSON" array.
					jsonData = jsonData.Replace("'[' +", "[").Replace("';", string.Empty).Replace(";", ",");
					jsonData = jsonData.Replace("'+", string.Empty).Replace("'{", "{");

					var movieData = JsonConvert.DeserializeObject<MineNerdData>($"{{{jsonData}}}");
					int id = 1;

					foreach (var movie in movieData.Movies)
					{
						var name = RemovePunctuation(HttpUtility.HtmlDecode(movie.Title));
						var newMovie = new Movie
						{
							Id = id++,
							Name = MapName(ParseName(name)),
							Day = ParseDayOfWeek(name),
							Earnings = movie.OriginalEstimatedBoxOffice * 1000,
							Cost = movie.Bux,
							//WeekendEnding = MovieDateUtil.NextSunday().Date
							WeekendEnding = MovieDateUtil.ThisSunday().Date
						};

						result.Add(newMovie);
					}
				}
			}

			Movies = result;

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private DayOfWeek? ParseDayOfWeek(string name)
		{
			DayOfWeek? result = null;
			DayOfWeek dayOfWeek;

			if (name.Length >= DAY_KEY_LENGTH && _daysOfWeek.TryGetValue(name.Substring(0, DAY_KEY_LENGTH), out dayOfWeek))
			{
				result = dayOfWeek;
			}

			return result;
		}

		private string ParseName(string name)
		{
			var result = name;

			if (result != null)
			{
				foreach (var key in _daysOfWeek.Keys)
				{
					if (result.StartsWith(key))
					{
						// Remove the key

						result = result.Substring(key.Length, result.Length - key.Length);
						break;
					}
				}
			}

			return result;
		}
	}
}