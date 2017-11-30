using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using System.Linq;

namespace MovieMiner
{
	public class MineToddThatcher : MinerBase
	{
		private const string DEFAULT_URL = "https://fantasymovieleague.com";
		private const string DELIMITER = "- $";

		private readonly string _articleTitle;
		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		public MineToddThatcher(string articleTitle = null)
			: base("Todd M. Thatcher", "Todd", DEFAULT_URL)
		{
			_articleTitle = articleTitle ?? $"Week {MovieDateUtil.DateToWeek()}";

			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				{" Friday", DayOfWeek.Friday},
				{" Saturday", DayOfWeek.Saturday},
				{" Sunday", DayOfWeek.Sunday}
			};
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/news");

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode($"//body//a[contains(@title, '{_articleTitle}')]");

			if (node != null)
			{
				var href = node.GetAttributeValue("href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					UrlSource = $"{Url}/{href}";

					doc = web.Load(UrlSource);

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

						// As of 11/2/2017 Todd is separating things with <br /> now.

						if (movieNodes.Count == 1)
						{
							var innerHtml = HttpUtility.HtmlDecode(movieNodes.First().InnerHtml);
							var delimiters = new string[] { "<br><br>" };
							var tokens = innerHtml.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

							foreach (var token in tokens)
							{
								if (token.StartsWith("\"") && token.EndsWith("million"))
								{
									AddMovie(token, articleDate, result);
								}
							}
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

									var valueInMillions = nodeText.Substring(index, nodeText.Length - index)?.Contains("million");

									var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace("million", string.Empty);

									var parenIndex = estimatedBoxOffice.IndexOf("(");

									if (parenIndex > 0)
									{
										// Trim out the FML bux.
										estimatedBoxOffice = estimatedBoxOffice.Substring(0, parenIndex - 1);
									}

									if (!string.IsNullOrEmpty(movieName))
									{
										var name = RemovePunctuation(HttpUtility.HtmlDecode(movieName));
										var movie = new Movie
										{
											MovieName = MapName(ParseName(name)),
											Day = ParseDayOfWeek(name),
											Earnings = decimal.Parse(estimatedBoxOffice) * (valueInMillions.Value ? 1000000 : 1)
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
			}

			Movies = result;

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AddMovie(string nodeText, DateTime? articleDate, List<IMovie> result)
		{
			int index = nodeText.IndexOf(DELIMITER);
			var movieName = nodeText.Substring(0, index);

			// Might switch this to RegEx...

			var valueInMillions = nodeText.Substring(index, nodeText.Length - index)?.Contains("million");

			var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace("million", string.Empty);

			var parenIndex = estimatedBoxOffice.IndexOf("(");

			if (parenIndex > 0)
			{
				// Trim out the FML bux.
				estimatedBoxOffice = estimatedBoxOffice.Substring(0, parenIndex - 1);
			}

			if (!string.IsNullOrEmpty(movieName))
			{
				var name = RemovePunctuation(HttpUtility.HtmlDecode(movieName));
				var movie = new Movie
				{
					MovieName = MapName(ParseName(name)),
					Day = ParseDayOfWeek(name),
					Earnings = decimal.Parse(estimatedBoxOffice) * (valueInMillions.Value ? 1000000 : 1)
				};

				if (articleDate.HasValue)
				{
					movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
				}

				result.Add(movie);
			}
		}

		private DayOfWeek? ParseDayOfWeek(string name)
		{
			DayOfWeek? result = null;

			if (name != null)
			{
				foreach (var pair in _daysOfWeek)
				{
					if (name.EndsWith(pair.Key))
					{
						// Remove the key

						result = pair.Value;
						break;
					}
				}
			}

			return result;
		}

		private string ParseName(string name)
		{
			var result = name;

			if (result != null)
			{
				foreach (var key in _daysOfWeek.Keys)
				{
					if (result.EndsWith(key))
					{
						// Remove the key

						result = result.Substring(0, result.Length - key.Length);
						break;
					}
				}
			}

			return result;
		}
	}
}