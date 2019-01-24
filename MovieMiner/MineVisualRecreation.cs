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
		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		private bool _mineData = false;
		//private bool _mineData = true;

		public MineVisualRecreation()
			: base("Visual Recreation", "Vis Rec", "https://twitter.com/VisRecVids")
		{
			TwitterID = "VisRecVids";

			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				{" fri", DayOfWeek.Friday},
				{" sat", DayOfWeek.Saturday},
				{" sun", DayOfWeek.Sunday},
				{" mon", DayOfWeek.Monday}
			};
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
				var weekend = new DateTime(2019, 1, 20);
				UrlSource = "https://twitter.com/VisRecVids/status/1086047552829558787";
				return new List<IMovie>
						{
								new Movie { MovieName = "Glass", Earnings = 35000000m, WeekendEnding = weekend, Day = DayOfWeek.Friday },
								new Movie { MovieName = "Glass", Earnings = 21000000m, WeekendEnding = weekend, Day = DayOfWeek.Sunday },
								new Movie { MovieName = "The Upside", Earnings = 16500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Aquaman", Earnings = 13500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "A Dogs Way Home", Earnings = 9800000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "SpiderMan Into the SpiderVerse", Earnings = 9000000m, WeekendEnding = weekend },
								new Movie { MovieName = "Dragon Ball Super Broly", Earnings = 5500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Mary Poppins Returns", Earnings = 6000000m, WeekendEnding = weekend },
								new Movie { MovieName = "Bumblebee", Earnings = 5800000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Escape Room", Earnings = 5500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 3800000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "The Mule", Earnings = 4200000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "On the Basis of Sex", Earnings = 4500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "If Beale Street Could Talk", Earnings = 2500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Green Book", Earnings = 2300000.0m, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			UrlSource = $"{Url}/status/1086047552829558787";
			//UrlSource = $"https://twitter.com/VisRecVids";

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
								var earnings = tokens.Length == 3 ? ParseEarnings(tokens[2]) : ParseEarnings(tokens[1]);

								if (earnings > 0)
								{
									var dayOfWeek = ParseDayOfWeek(tokens[1]);
									var movie = new Movie() { MovieName = ParseName(tokens[0], dayOfWeek), Day = dayOfWeek, Earnings = earnings };

									result.Add(movie);
								}
							}
						}
					}
				}
			}

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private DayOfWeek? ParseDayOfWeek(string name)
		{
			DayOfWeek? result = null;

			if (name != null)
			{
				name = name.ToLower();

				foreach (var pair in _daysOfWeek)
				{
					if (name.EndsWith(pair.Key))
					{
						result = pair.Value;
						break;
					}
				}

				if (result == null)
				{
					var tokens = name.Split(new char[] { '+' });

					if (tokens.Length > 0)
					{
						foreach (var pair in _daysOfWeek)
						{
							if (tokens[0].StartsWith(pair.Key.Replace(" ", string.Empty)))
							{
								result = pair.Value;
								break;
							}
						}
					}
				}
			}

			return result;
		}

		private string ParseName(string name, DayOfWeek? dayOfWeek)
		{
			var result = name;

			if (result != null && dayOfWeek.HasValue)
			{
				// Remove the day of week token.

				var index = name.LastIndexOf(' ');

				if (index > 0)
				{
					result = result.Substring(0, index);
				}
			}

			return result;
		}
	}
}