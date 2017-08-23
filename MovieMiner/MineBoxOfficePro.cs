using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficePro : MinerBase
	{
		private const string DEFAULT_URL = "http://pro.boxoffice.com/";

		public MineBoxOfficePro()
			: base(DEFAULT_URL)
		{
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load(Url);

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

					// Now retrieve the article page.

					doc = web.Load(href);

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

					var tableRows = node?.SelectNodes("tbody/tr[position()>1]");		// Skips the table header row (row 0)

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
										movie = new Movie { Name = RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText)) };

										if (articleDate.HasValue)
										{
											movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
										}
									}
									else if (columnCount == 2)
									{
										movie.Earnings = decimal.Parse(column.InnerText?.Replace("$", string.Empty));
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

		public async override Task<List<IMovie>> MineAsync()
		{
			throw new NotImplementedException();
		}
	}
}