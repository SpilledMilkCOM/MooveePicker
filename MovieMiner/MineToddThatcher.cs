using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineToddThatcher : MinerBase
	{
		private const string DEFAULT_URL = "https://fantasymovieleague.com";
		private const string DELIMITER = " - $";

		private readonly string _articleTitle;

		public MineToddThatcher(string articleTitle = "Week 13 Estimates")
			: base(DEFAULT_URL)
		{
			_articleTitle = articleTitle;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/news");

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode($"//body//a[@title='{_articleTitle}']");

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					doc = web.Load($"{Url}/{href}");

					// Get the date of the article

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='credits']/span[@class='date']");

					if (node != null)
					{
						// Remove the first child span.

						if (node.HasChildNodes)
						{
							string articleText = HttpUtility.HtmlDecode(node.FirstChild.InnerText).Trim();
							DateTime parsedDateTime;

							if (DateTime.TryParse(articleText, out parsedDateTime))
							{
								articleDate = parsedDateTime;
							}
						}
					}

					// Get the data

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='article__content']");

					if (node != null)
					{
						var movieNodes = node.SelectNodes($"//p[contains(., '{DELIMITER}')]");     // Find all of the estimate paragraphs

						foreach (var movieNode in movieNodes)
						{
							int index = movieNode.InnerText.IndexOf(DELIMITER);

							if (index > 0)
							{
								var nodeText = movieNode.InnerText;
								var movieName = nodeText.Substring(0, index);
								var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace("million", string.Empty);

								if (!string.IsNullOrEmpty(movieName))
								{
									var movie = new Movie
									{
										Name = RemovePunctuation(HttpUtility.HtmlDecode(movieName)),
										Earnings = decimal.Parse(estimatedBoxOffice) * 1000000
									};

									if (articleDate.HasValue)
									{
										movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
									}

									result.Add(movie);
								}
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