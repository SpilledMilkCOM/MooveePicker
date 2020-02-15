using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineTheNumbers : MinerBase
	{
		public const string DEFAULT_URL = "https://www.the-numbers.com/";
		private const string DELIMITER = "- $";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineTheNumbers(DateTime? weekendEnding = null)
			: base($"The Numbers", $"#'s", DEFAULT_URL)
		{
			TwitterID = "BoxOfficeMojo";
			WeekendEnding = weekendEnding?.Date;
		}

		public DateTime? WeekendEnding { get; private set; }

		public override IMiner Clone()
		{
			var result = new MineTheNumbers();

			Clone(result);

			result.WeekendEnding = WeekendEnding;

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			string url = $"{Url}/daily-box-office-chart";
			var web = new HtmlWeb();

			ContainsEstimates = false;
			WeekendEnding = MovieDateUtil.GameSunday();		// This page should always have the "current" theater count.

			//  https://www.the-numbers.com/daily-box-office-chart

			var doc = web.Load(url);

			UrlSource = url;

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			//var tableRows = doc.DocumentNode?.SelectNodes("//body//table//tr[position()>1]");
			var tableRows = doc.DocumentNode?.SelectNodes("//body//table//tr");

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
								movie = new Movie
								{
									Name = RemovePunctuation(MapName(HttpUtility.HtmlDecode(column.InnerText)))
								};

								if (WeekendEnding.HasValue)
								{
									movie.WeekendEnding = WeekendEnding.Value;
								}
							}
							else if (columnCount == 4)
							{
								movie.Earnings = ParseEarnings(column.InnerText);
							}
							else if (columnCount == 7)
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

			if (articleNumber == 1)
			{
				//node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, '/news/?id=')]");
				node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, '/article/')]");
			}
			else
			{
				//var nodes = doc.DocumentNode.SelectNodes("//body//a[contains(@href, '/news/?id=')]");
				var nodes = doc.DocumentNode.SelectNodes("//body//a[contains(@href, '/article/')]");

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