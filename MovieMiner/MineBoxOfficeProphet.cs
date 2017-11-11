using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeProphet : MinerBase
	{
		private const string DEFAULT_URL = "http://www.boxofficeprophets.com/";

		public MineBoxOfficeProphet()
			: base("Box Office Prophet", "Prophet", DEFAULT_URL)
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

			//var node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, 'http://www.boxofficeprophets.com/column') and //strong='Weekend Forecast']");
			HtmlNode node = null;

			var nodes = doc.DocumentNode.SelectNodes("//body//a[contains(@href, 'http://www.boxofficeprophets.com/column')]");

			foreach (var aNode in nodes)
			{
				if (aNode.InnerText.Contains("Weekend Forecast"))
				{
					node = aNode;
					break;
				}
			}

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					doc = web.Load(href);

					// Get the date of the article

					node = doc.DocumentNode.SelectSingleNode("//body//div[@id='EchoTopic']//h4");

					if (node != null)
					{
						string articleText = node.InnerText;
						DateTime parsedDateTime;

						if (DateTime.TryParse(articleText, out parsedDateTime))
						{
							articleDate = parsedDateTime;
						}
					}

					// Get the data in the table.

					node = doc.DocumentNode.SelectSingleNode("//body//div[@id='EchoTopic']//table[@width='100%']");

					// TODO: Parse the header for column titles for mapping.

					//var tableRows = node?.SelectNodes("//tr[position()>2]");
					var tableRows = node?.SelectNodes("//tr[@bgcolor='eeeeee']");

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
										movie = new Movie { Name = RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText)) };

										if (articleDate.HasValue)
										{
											movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
										}
									}
									else if (columnCount == 4)
									{
										movie.Earnings = decimal.Parse(column.InnerText) * 1000000;
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

			Movies = result;

			return result;
		}
	}
}