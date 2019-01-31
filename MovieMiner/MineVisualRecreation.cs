﻿using System;
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
				var weekend = new DateTime(2019, 2, 3);
				UrlSource = "https://twitter.com/VisRecVids/status/1090962665160232960";
				return new List<IMovie>
						{
								new Movie { MovieName = "Glass", Earnings = 9150000.00m, WeekendEnding = weekend },
								new Movie { MovieName = "The Upside", Earnings = 7800000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Miss Bala", Earnings = 7300000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Aquaman", Earnings = 4100000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Green Book", Earnings = 3900000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "SpiderMan Into the SpiderVerse", Earnings = 4500000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "The Kid Who Would Be King", Earnings = 4100000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "A Dogs Way Home", Earnings = 3300000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Escape Room", Earnings = 2450000.00m, WeekendEnding = weekend },
								new Movie { MovieName = "The Favourite", Earnings = 2000000m, WeekendEnding = weekend },
								new Movie { MovieName = "Serenity", Earnings = 1700000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Mary Poppins Returns", Earnings = 1960000.00m, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 1600000.0m, WeekendEnding = weekend },
								new Movie { MovieName = "Bumblebee", Earnings = 1500000.0m, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			UrlSource = $"{Url}/status/1090962665160232960";
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

					if (innerText.Contains("Predictions") || innerText.Contains("Estimates"))
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