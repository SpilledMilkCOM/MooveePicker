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
	public class MineBonusBar : MinerBase
	{
		private const string DEFAULT_URL = "https://fantasymovieleague.com";
		private const string DELIMITER = "|";
		private readonly string _articleTitle;
		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		public MineBonusBar(string articleTitle = null)
			: base("Guyett's Bonus Bar", "Bar", DEFAULT_URL)
		{
			TwitterID = "";

			_articleTitle = articleTitle ?? "Bonus Bar and Contenders";

			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				{" FRI", DayOfWeek.Friday},
				{" SAT", DayOfWeek.Saturday},
				{" SUN", DayOfWeek.Sunday},
				{" MON", DayOfWeek.Monday}
			};
		}

		public override IMiner Clone()
		{
			var result = new MineBonusBar();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/chatter/searchmessages?boardId=fml-main-chatter&query=bonus%20bar");

			// Lookup XPATH to get the right node that matches.
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode($"//body//div/h3[@class='topic-item__title']");

			if (node != null)
			{
				// Traverse up to the <div>
				node = node.ParentNode;
			}

			if (node != null)
			{
				var href = node.GetAttributeValue("data-href", null);

				if (href != null)
				{
					DateTime? articleDate = null;

					// Now retrieve the article page.

					UrlSource = $"{Url}/{href}";

					doc = web.Load(UrlSource);

					// Get the date of the article

					node = doc.DocumentNode.SelectSingleNode("//body//span[@class='topic-item__attribution']/span[@class='time-date']");

					if (node != null)
					{
						// Remove the first child span.

						if (node.HasChildNodes)
						{
							var articleText = HttpUtility.HtmlDecode(node.FirstChild.InnerText.Replace(",", string.Empty)).Trim();

							// Remove the text after the year.

							var year = DateTime.Now.Year;
							var index = articleText.IndexOf(year.ToString());

							if (index > 0)
							{
								articleText = articleText.Substring(0, index + year.ToString().Length);
							}

							DateTime parsedDateTime;

							if (DateTime.TryParse(articleText, out parsedDateTime))
							{
								articleDate = parsedDateTime;
							}
						}
					}

					// Get the data

					node = doc.DocumentNode.SelectSingleNode("//body//div[@class='topic-item__body']");

					if (node != null)
					{
						var movieNodes = node.SelectNodes($"//p/strong");     // Find all of the estimate paragraphs (are in BOLD)

						foreach (var movieNode in movieNodes)
						{
							var nodeText = HttpUtility.HtmlDecode(movieNode.InnerHtml);

							var index = nodeText.IndexOf(DELIMITER);
							var index2 = nodeText.IndexOf(DELIMITER, index + 1);

							if (index > 0 && index2 > 0)
							{
								var movieName = nodeText.Substring(0, index);

								// Might switch this to RegEx...

								var estimatedBoxOffice = nodeText.Substring(index + 1, index2 - (index + 1))?.Replace("$", string.Empty);

								var trimIndex = estimatedBoxOffice.IndexOf("(");

								if (trimIndex > 0)
								{
									// Trim out the drop percentage (and everything after).
									estimatedBoxOffice = estimatedBoxOffice.Substring(0, trimIndex - 1);
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
											Earnings = ParseEarnings(estimatedBoxOffice)
										};

										if (movie.Day.HasValue)
										{
											CompoundLoaded = true;
										}
									}
									catch (Exception exception)
									{
										Error = "Some bad data";
										ErrorDetail = $"The movie did not parse correctly \"{name}\" - {exception.Message}";
										movie = null;
									}

									if (movie != null)
									{
										if (!result.Contains(movie))
										{
											if (articleDate.HasValue)
											{
												movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
											}

											result.Add(movie);
										}
										else if (movie.Day.HasValue)
										{
											if (articleDate.HasValue)
											{
												movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate);
											}

											result.Add(movie);

											// Remove the movie that does NOT have a day.

											var toRemove = result.FirstOrDefault(item => item.Equals(movie) && !item.Day.HasValue);

											if (toRemove != null)
											{
												result.Remove(toRemove);
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

		public void SetBar(decimal barEfficiency)
		{
			Movies.ForEach(movie => movie.Earnings = movie.Cost * barEfficiency * 1000);
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