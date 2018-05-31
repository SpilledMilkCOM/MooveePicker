using System;
using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeReport : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficereport.com/";
		private const string DELIMITER = "- $";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeReport()
			: base("Box Office Report", "BO Rpt", DEFAULT_URL)
		{
			TwitterID = "BORReport";
		}

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeReport();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			string url = Url;
			var web = new HtmlWeb();

			var doc = web.Load(url);

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, 'predictions')]");

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					UrlSource = href;

					doc = web.Load(UrlSource);

					// Get the date of the article (hoping that the date is the ONLY thing in such a small font)

					node = doc.DocumentNode.SelectSingleNode("//body//h2[text()='Weekend Predictions']");

					if (node == null)
					{
						node = doc.DocumentNode.SelectSingleNode("//body//h2[text()='4-Day Weekend Predictions']");

						Error = "4-day";
					}

					if (node != null)
					{
						// Remove the first child span.

						if (node.HasChildNodes)
						{
							var articleText = HttpUtility.HtmlDecode(node.LastChild.InnerText).Trim();
							char[] delimiters = { '-' };
							var splitText = articleText.Split(delimiters);
							DateTime parsedDateTime;

							if (splitText != null && splitText.Length > 1 && DateTime.TryParse(splitText[1].Trim(), out parsedDateTime))
							{
								articleDate = parsedDateTime.AddDays(-1);
							}
						}
					}

					// Get the data in the table.
					// TODO: Parse the header for column titles for mapping.

					var tableRows = doc.DocumentNode?.SelectNodes("//body//table[@class='inlineTable']//tr[position()>1]");

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
									if (columnCount == 1)
									{
										var movieName = HttpUtility.HtmlDecode(column.InnerText);

										if (movieName != null)
										{
											// Remove the studio
											var parenIndex = movieName.IndexOf("(");

											if (parenIndex > 0)
											{
												// Trim out the FML bux.
												movieName = movieName.Substring(0, parenIndex).Trim();
											}

											movie = new Movie { Name = MapName(RemovePunctuation(movieName)) };

											if (articleDate.HasValue)
											{
												movie.WeekendEnding = MovieDateUtil.GameSunday(articleDate);
											}
										}
									}
									else if (columnCount == 2)
									{
										movie.Earnings = ParseEarnings(column.InnerText);
									}

									columnCount++;
								}
							}

							if (movie != null && !string.IsNullOrEmpty(movie.MovieName) && movie.EarningsBase > 0)
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