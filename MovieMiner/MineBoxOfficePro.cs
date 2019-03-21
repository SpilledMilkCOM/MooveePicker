using System;
using System.Collections.Generic;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficePro : MinerBase
	{
		private const string DEFAULT_URL = "https://pro.boxoffice.com/";
		private const decimal MBAR = 1000000;       // The Roman numeral M with a bar over it is a million.

		private bool _mineData = false;
		//private bool _mineData = true;

		public MineBoxOfficePro()
			: base("Box Office Pro", "BO Pro", DEFAULT_URL)
		{
			TwitterID = "BoxOffice";
		}

		public override IMiner Clone()
		{
			var result = new MineBoxOfficePro();

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
				var weekend = new DateTime(2019, 3, 24);
				UrlSource = "https://www.boxofficepro.com/weekend-forecast-jordan-peeles-us-poised-for-breakout-debut";
				return new List<IMovie>
						{
								new Movie { MovieName = "Us", Earnings = 54 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Captain Marvel", Earnings = 35 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Wonder Park", Earnings = 9.8m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Five Feet Apart", Earnings = 7.4m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "How to Train Your Dragon The Hidden World", Earnings = 6.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Tyler Perrys A Madea Family Funeral", Earnings = 3.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "No Manches Frida 2", Earnings = 2 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Captive State", Earnings = 1.4m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The LEGO Movie 2 The Second Part", Earnings = 1.6m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Alita Battle Angel", Earnings = 1.1m * MBAR, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load(Url);
			// Can't instanciate an Active-X control within a web application.
			//var doc = web.LoadFromBrowser(Url);		

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			//var node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, 'estimates-weekend')]");
			var node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, 'weekend-forecast')]");

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					UrlSource = href;

					// Now retrieve the article page.

					doc = web.Load(UrlSource);

					// Get the date of the article

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='entry-meta']/span[@class='entry-meta__post-date']");

					if (node != null)
					{
						string articleText = node.InnerText.Replace("&nbsp;•&nbsp;", string.Empty);
						DateTime parsedDateTime;

						if (DateTime.TryParse(articleText, out parsedDateTime))
						{
							articleDate = parsedDateTime;
						}
					}

					// Get the data in the table.

					//node = doc.DocumentNode.SelectSingleNode("//body//div[@class='post-container']/table");
					//node = doc.DocumentNode.SelectSingleNode("//body//table[@class='wp-block-table aligncenter']");
					node = doc.DocumentNode.SelectSingleNode("//body//table[@width='726']");

					// TODO: Parse the header for column titles for mapping.

					var tableRows = node?.SelectNodes("tbody/tr[position()>1]");        // Skips the table header row (row 0)

					if (tableRows != null)
					{
						foreach (var row in tableRows)
						{
							Movie movie = null;
							var rowColumns = row.SelectNodes("td");

							if (rowColumns != null)
							{
								int columnCount = 0;

								foreach (var column in rowColumns)
								{
									if (columnCount == 0)
									{
										movie = new Movie { Name = MapName(RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText))) };

										if (articleDate.HasValue)
										{
											movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
										}
									}
									else if (columnCount == 2)
									{
										//movie.Earnings = decimal.Parse(column.InnerText?.Replace("$", string.Empty));

										var rawText = RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText));

										if (rawText.Contains("4day"))
										{
											var tokens = rawText.Split();

											if (tokens.Length > 2)
											{
												rawText = tokens[tokens.Length - 2];
											}
										}

										movie.Earnings = decimal.Parse(rawText);
									}

									columnCount++;
								}
							}

							if (movie != null)
							{
								result.Add(movie);
							}
						}
					}
				}
			}

			return result;
		}
	}
}