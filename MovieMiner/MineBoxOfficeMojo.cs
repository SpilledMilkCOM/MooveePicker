﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeMojo : MinerBase
	{
		public const string DEFAULT_URL = "https://boxofficemojo.com/";
		private const string DELIMITER = "- $";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeMojo(DateTime? weekendEnding = null)
			: base($"Box Office Mojo {weekendEnding?.ToShortDateString()}"
				  , $"BO Mojo {weekendEnding?.ToShortDateString()}", DEFAULT_URL)
		{
			TwitterID = "BoxOfficeMojo";
			WeekendEnding = weekendEnding?.Date;
		}

		public DateTime? WeekendEnding { get; private set; }

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeMojo();

			Clone(result);

			result.WeekendEnding = WeekendEnding;

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();

			if (WeekendEnding.HasValue)
			{
				var lastSunday = MovieDateUtil.LastSunday(MovieDateUtil.GameSunday(null, ContainsEstimates).AddDays(-1));

				// Check to see if the weekend ending is out of date.

				if (ContainsEstimates || (WeekendEnding.Value < lastSunday && !ContainsEstimates))
				{
					WeekendEnding = lastSunday;
				}

				result = MineDate();
			}
			else
			{
				result = MineForecast();

				if (Error == NO_DATA || !result.Any())
				{
					// Retry until you get some data.

					for (int pastArticles = 2; pastArticles < 4; pastArticles++)
					{
						Error = string.Empty;
						result = MineForecast(pastArticles);

						if (result.Count > 0 && (string.IsNullOrEmpty(Error) || Error == FOUR_DAY))
						{
							break;
						}
					}
				}
			}

			return result;
		}

		public List<IMovie> MineWeekend(DateTime? date = null)
		{
			WeekendEnding = date;
			return MineDate();
		}

		public void SetCompoundLoaded(bool compoundLoaded)
		{
			CompoundLoaded = compoundLoaded;
		}

		private List<IMovie> MineDate()
		{
			var result = new List<IMovie>();
			string url = $"{Url}weekend/chart/";
			var web = new HtmlWeb();

			ContainsEstimates = false;

			// Might have to tweak this offset a bit to get the numbers to match.
			var sundayOffset = (int)new DateTime(WeekendEnding.Value.Year, 1, 1).DayOfWeek;

			//  https://www.boxofficemojo.com/weekend/2019W42/

			//url = $"{Url}weekend/chart/?yr={WeekendEnding.Value.Year}&wknd={((WeekendEnding.Value.DayOfYear - sundayOffset) / 7) + 1}&p=.htm";
			//url = $"{Url}weekend/{WeekendEnding.Value.Year}W{((WeekendEnding.Value.DayOfYear - sundayOffset) / 7) + 1}/";
			url = $"{Url}weekend/{WeekendEnding.Value.Year}W{((WeekendEnding.Value.DayOfYear - sundayOffset) / 7) + 1:00}/";

			var doc = web.Load(url);

			UrlSource = url;

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			// TODO: Parse the header for column titles for mapping.

			//var tableRows = doc.DocumentNode?.SelectNodes("//table[@cellpadding='5']//tr[position()>1]");
			//var tableRows = doc.DocumentNode?.SelectNodes("//body//table[@class='a-bordered a-horizontal-stripes a-size-base a-span12 mojo-body-table mojo-body-table-compact scrolling-data-table']");
			//var tableRows = doc.DocumentNode?.SelectNodes("//body//table[contains(@class, 'scrolling-data-table')]");

			var tableRows = doc.DocumentNode?.SelectNodes("//body//table//tr[position()>1]");

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
							if (columnCount == 2)
							{
								var anchor = column.SelectSingleNode(".//a");
								var movieDetailUrl = anchor?.GetAttributeValue("href", null);

								movie = new Movie
								{
									Name = MapName(RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText))),
									Identifier = movieDetailUrl.Replace(DEFAULT_URL, string.Empty)
								};

								if (WeekendEnding.HasValue)
								{
									movie.WeekendEnding = WeekendEnding.Value;
								}
							}
							else if (columnCount == 3)
							{
								movie.Earnings = decimal.Parse(column.InnerText?.Replace("$", string.Empty).Replace("-", "0"));
							}
							else if (columnCount == 6)
							{
								decimal theaterCount = 0;

								if (decimal.TryParse(column.InnerText?.Replace("-", "0"), out theaterCount))
								{
									movie.TheaterCount = (int)theaterCount;
								}
								break;
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

			return result;
		}

		private List<IMovie> MineForecast(int articleNumber = 1)
		{
			var result = new List<IMovie>();
			string url = Url + "news/";
			var web = new HtmlWeb();

			var doc = web.Load(url);        // Load main page.

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			HtmlNode node = null;
			const string XPATH = "//body//a[contains(@href, '/article/')]";

			if (articleNumber == 1)
			{
				//node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, '/news/?id=')]");
				node = doc.DocumentNode.SelectSingleNode(XPATH);
			}
			else
			{
				//var nodes = doc.DocumentNode.SelectNodes("//body//a[contains(@href, '/news/?id=')]");
				var nodes = doc.DocumentNode.SelectNodes(XPATH);

				if (nodes != null && articleNumber <= nodes.Count)
				{
					node = nodes[articleNumber - 1];
				}
			}

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					UrlSource = $"{Url}/{href}";

					doc = web.Load(UrlSource);

					// Get the date of the article (hoping that the date is the ONLY thing in such a small font)

					//node = doc.DocumentNode.SelectSingleNode("//body//font[@size='1']");
					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='mojo-news-byline']");

					if (node != null)
					{
						// Remove the first child span.

						if (node.ChildNodes.Count > 1)
						{
							string articleText = HttpUtility.HtmlDecode(node.ChildNodes[1].InnerText).Trim();
							var tokens = articleText.Split(new char[] { '-' });
							DateTime parsedDateTime;

							if (tokens.Length > 0 && DateTime.TryParse(tokens[0].Replace("PDT", string.Empty).Replace("PST", string.Empty), out parsedDateTime))
							{
								articleDate = parsedDateTime.Date;
							}
						}
					}

					// Need to scan for the <p> tag that contains "This weekend's forecast is directly below."

					// The movies are just in a <ul> tag (unsorted list)

					var movieNodes = doc.DocumentNode?.SelectNodes("//body//ul/li/span[@class='a-list-item']");

					if (movieNodes == null)
					{
						Error = NO_DATA;
					}
					else
					{
						foreach (var movieNode in movieNodes)
						{
							int index = movieNode.InnerText.IndexOf(DELIMITER);

							if (index > 0)
							{
								var nodeText = movieNode.InnerText;
								var movieName = nodeText.Substring(0, index);

								// Might switch this to RegEx...

								var valueInMillions = nodeText.Substring(index, nodeText.Length - index)?.Contains("M");

								var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace("M", string.Empty);

								var parenIndex = movieName.IndexOf("(");

								if (parenIndex > 0)
								{
									// Trim out the THEATERS (for now).
									movieName = movieName.Substring(0, parenIndex - 1).Trim();
								}

								parenIndex = estimatedBoxOffice.IndexOf("(");

								if (parenIndex > 0)
								{
									// Trim out the multi-day value.
									estimatedBoxOffice = estimatedBoxOffice.Substring(0, parenIndex - 1).Trim();
								}

								decimal estBoxOffice;

								if (!string.IsNullOrEmpty(movieName) && decimal.TryParse(estimatedBoxOffice, out estBoxOffice))
								{
									var name = MapName(RemovePunctuation(HttpUtility.HtmlDecode(movieName)));
									var movie = new Movie
									{
										MovieName = name,
										Earnings = estBoxOffice * (valueInMillions.Value ? 1000000 : 1)
									};

									if (articleDate.HasValue)
									{
										movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
									}

									if (movie != null)
									{
										if (!result.Contains(movie))
										{
											result.Add(movie);
										}
										else if (GameDays > 3)
										{
											// It's OK to override the BO value if the game days is MORE than the default.

											// Need to use "fuzzy" logic here because the names may have dates as suffixes and those should match.
											var found = result.Find(item => item.Equals(movie));

											if (found != null && found.EarningsBase < movie.EarningsBase)
											{
												// Replace the movie if a larger value was found. (4 day weekend versus 3 day)

												result.Remove(found);
												result.Add(movie);

												Error = FOUR_DAY;
											}
										}
									}
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