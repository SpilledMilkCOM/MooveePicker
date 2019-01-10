using System;
using System.Collections.Generic;

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using HtmlAgilityPack;
using System.Web;

namespace MovieMiner
{
	public class MineVisualRecreation : MinerBase
	{
		private const string DEFAULT_URL = "https://www.youtube.com/channel/UCy3oMGqYJiPWQ030mYbaZig";
		private const decimal MBAR = 1000000;       // The Roman numeral M with a bar over it is a million.

		//private bool _mineData = false;
		private bool _mineData = true;

		public MineVisualRecreation()
			: base("Visual Recreation", "Vis Rec", "https://twitter.com/VisRecVids")
		{
			TwitterID = "VisRecVids";
		}

		public override IMiner Clone()
		{
			var result = new MineVisualRecreation();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			if (_mineData)
			{
				return MineData();
			}
			else
			{
				var weekend = new DateTime(2019, 1, 13);
				UrlSource = "https://twitter.com/VisRecVids/status/1083349593272799232";
				return new List<IMovie>
						{
								new Movie { MovieName = "Aquaman", Earnings = 17 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "A Dogs Way Home", Earnings = 13.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Upside", Earnings = 14.2m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Escape Room", Earnings = 8.2m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "On the Basis of Sex", Earnings = 7 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Mary Poppins Returns", Earnings = 8.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "SpiderMan Into the SpiderVerse", Earnings = 9.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Bumblebee", Earnings = 7.2m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Mule", Earnings = 5.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Vice", Earnings = 4.3m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Replicas", Earnings = 3.6m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 3.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "If Beale Street Could Talk", Earnings = 2.8m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Second Act", Earnings = 2.3m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Ralph Breaks the Internet", Earnings = 0 * MBAR, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			UrlSource = $"{Url}/status/1083349593272799232";

			var doc = web.Load(UrlSource);

			var xpathNav = doc.CreateNavigator();

			// Select the first <a> node that contains the title attribute.

			//			var nodes = doc.DocumentNode.SelectNodes($"//div[contains(@class, 'js-tweet-text-container')]");
			var nodes = doc.DocumentNode.SelectNodes($"//body//div[@class='tweet-text']");

			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					var innerText = HttpUtility.HtmlDecode(node.InnerText);

					if (innerText.Contains("Predictions"))
					{
						var lines = innerText.Split(new char[] { '#' });

						foreach (var line in lines)
						{
							var tokens = line.Split(new char[] { ' ' });

							if (tokens.Length >= 2)
							{
								var earnings = ParseEarnings(tokens[1]);

								if (earnings > 0)
								{
									var movie = new Movie() { MovieName = tokens[0], Earnings = earnings };

									result.Add(movie);
								}
							}
						}
					}
				}
			}

			return result;
		}
	}
}