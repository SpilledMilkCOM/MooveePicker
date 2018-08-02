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

		//public override List<IMovie> Mine()
		//{
		//	DateTime weekend = new DateTime(2018, 7, 29);

		//	return new List<IMovie>
		//	{
		//		new Movie { MovieName = "Mission Impossible  Fallout", Earnings = 70000000, WeekendEnding = weekend },
		//		new Movie { MovieName = "Teen Titans GO to the Movies", Earnings = 17500000, WeekendEnding = weekend },
		//		new Movie { MovieName = "Mamma Mia Here We Go Again", Earnings = 16500000, WeekendEnding = weekend },
		//		new Movie { MovieName = "The Equalizer 2", Earnings = 15700000, WeekendEnding = weekend },
		//		new Movie { MovieName = "Hotel Transylvania 3 Summer Vacation", Earnings = 12800000, WeekendEnding = weekend }
		//	};
		//}

		public override List<IMovie> Mine()
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

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='post-meta']/span[@class='date']");

					if (node != null)
					{
						string articleText = node.InnerText.Replace("Published ", string.Empty);
						DateTime parsedDateTime;

						if (DateTime.TryParse(articleText, out parsedDateTime))
						{
							articleDate = parsedDateTime;
						}
					}

					// Get the data in the table.

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='post-container']/table");

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
										movie.Earnings = decimal.Parse(RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText)));
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