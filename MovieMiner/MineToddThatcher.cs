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
		private const string DELIMITER2 = "-$";
		private readonly string _articleTitle;
		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		public MineToddThatcher(string articleTitle = null)
			: base("Todd M. Thatcher", "Todd", DEFAULT_URL)
		{
			TwitterID = "tthizz";

			_articleTitle = articleTitle ?? $"Week {MovieDateUtil.DateToWeek()}";

			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				{" Friday", DayOfWeek.Friday},
				{" Saturday", DayOfWeek.Saturday},
				{" Sunday", DayOfWeek.Sunday},
				{" Monday", DayOfWeek.Monday}
			};
		}

		public override IMiner Clone()
		{
			var result = new MineToddThatcher();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/news");

			// Lookup XPATH to get the right node that matches.
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			// Select the first <a> node that contains the title attribute.

			var node = doc.DocumentNode.SelectSingleNode($"//body//a[contains(@title, '{_articleTitle}')]");

			if (node == null)
			{
				node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@title, 'Box Office Estimates') and not(contains(@title, 'Perri'))]|//body//a[contains(@title, 'Box Office Predictions') and not(contains(@title, 'Perri'))]");
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

					// Get the date of the article

					//node = doc.DocumentNode.SelectSingleNode("//body//div[@class='credits']/span[@class='date']");
					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='post__credits']/div[@class='post__date-time']/div");

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

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='post__content']");

					if (node != null)
					{
						var movieNodes = node.SelectNodes($"//p[contains(., '{DELIMITER}')]|//p[contains(., '{DELIMITER2}')]");     // Find all of the estimate paragraphs

						// As of 11/2/2017 Todd is separating things with <br /> now.

						if (movieNodes.Count == 1)
						{
							var innerHtml = HttpUtility.HtmlDecode(movieNodes.First().InnerHtml);
							var delimiters = new string[] { "\n", "<br>", "<br>\n", "<br><br>" };
							var tokens = innerHtml.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

							foreach (var token in tokens)
							{
								if (token != null && token.StartsWith("\""))// && token.EndsWith("million"))
								{
									AddMovie(token.Replace("<br>", string.Empty), articleDate, result);
								}
							}
						}
						else
						{
							foreach (var movieNode in movieNodes)
							{
								int index = movieNode.InnerText.IndexOf(DELIMITER);

								if (index < 0)
								{
									index = movieNode.InnerText.IndexOf(DELIMITER2);
								}

								if (index > 0)
								{
									var nodeText = movieNode.InnerText;
									var movieName = nodeText.Substring(0, index);

									// Might switch this to RegEx...

									var multiplier = Multiplier(nodeText.Substring(index, nodeText.Length - index));
									var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace(DELIMITER2, string.Empty).Replace("million", string.Empty).Replace("k", string.Empty);

									var parenIndex = estimatedBoxOffice.IndexOf("(");

									if (parenIndex > 0)
									{
										// Trim out the FML bux.
										estimatedBoxOffice = estimatedBoxOffice.Substring(0, parenIndex - 1);
									}

									if (!string.IsNullOrEmpty(movieName))
									{
										var name = RemovePunctuation(HttpUtility.HtmlDecode(movieName));
										Movie movie = null;

										try
										{
											movie = new Movie
											{
												MovieName = MapName(ParseName(name)),
												Day = ParseDayOfWeek(name),
												Earnings = decimal.Parse(estimatedBoxOffice) * multiplier
											};

											if (movie.Day.HasValue)
											{
												CompoundLoaded = true;
											}
										}
										catch(Exception exception)
										{
											Error = "Some bad data";
											ErrorDetail = $"The movie did not parse correctly \"{name}\" - {exception.Message}";
											movie = null;
										}

										if (movie != null)
										{
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
			}

			return result;
		}

		private decimal Multiplier(string boxOffice)
		{
			decimal result = 1;

			if (boxOffice != null)
			{
				if (boxOffice.Contains("million"))
				{
					result = 1000000;
				}
				else if (boxOffice.Contains("k"))
				{
					result = 1000;
				}
			}

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AddMovie(string nodeText, DateTime? articleDate, List<IMovie> result)
		{
			int index = nodeText.IndexOf(DELIMITER);
			var movieName = nodeText.Substring(0, index);

			// Might switch this to RegEx...

			var valueInMillions = (nodeText.Substring(index, nodeText.Length - index)?.Contains("million") ?? false)
								|| (nodeText.Substring(index, nodeText.Length - index)?.Contains("milllion") ?? false);

			var estimatedBoxOffice = nodeText.Substring(index, nodeText.Length - index)?.Replace(DELIMITER, string.Empty).Replace("million", string.Empty).Replace("milllion", string.Empty);

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
					Earnings = decimal.Parse(estimatedBoxOffice) * (valueInMillions ? 1000000 : 1)
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