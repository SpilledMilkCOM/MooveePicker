using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineCulturedVultures : MinerBase
	{
		private const string DEFAULT_URL = "https://culturedvultures.com";

		public MineCulturedVultures()
			: base("Cultured Vultures", "CultVult", DEFAULT_URL)
		{
			TwitterID = "CultVultures";
		}

		public override IMiner Clone()
		{
			var result = new MineCulturedVultures();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/?s=weekend+box+office+predictions");

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode("//body//a[contains(@href, 'weekend-box-office-predictions-')]");

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

					node = doc.DocumentNode.SelectSingleNode("//body//time[@class='post-published updated']");

					if (node != null)
					{
						string articleText = node.InnerText.Replace("On ", string.Empty);
						DateTime parsedDateTime;

						if (DateTime.TryParse(articleText, out parsedDateTime))
						{
							articleDate = parsedDateTime;
						}
					}

					// Get the data in the article.
					// Rank might be important because the movies below the main ones are ranked as well

					var tableRows = doc.DocumentNode.SelectNodes("//body//article//h2[@style='text-align: center;'] | //body//article//p[contains(.,'Prediction:')]");

					if (tableRows != null)
					{
						string currentName = null;
						string currentValue = null;
						Movie currentMovie = null;

						foreach (var row in tableRows)
						{
							if (row.Name.ToLower() == "h2")
							{
								currentName = HttpUtility.HtmlDecode(row.InnerText);
							}
							else if (row.InnerText.StartsWith("Prediction:"))
							{
								currentValue = row.InnerText.Replace("Prediction:", string.Empty);
							}

							if (currentName != null && currentValue != null)
							{
								decimal earnings;
								var index = currentValue.IndexOf("million");

								if (index > 0)
								{
									currentValue = currentValue.Substring(0, index).Replace("$", string.Empty).Trim();
								}

								if (decimal.TryParse(currentValue, out earnings))
								{
									currentName = RemovePunctuation(RemoveStudio(currentName));

									currentMovie = new Movie
									{
										Name = MapName(currentName),
										Earnings = earnings * 1000000
									};

									if (articleDate.HasValue)
									{
										currentMovie.WeekendEnding = MovieDateUtil.NextSunday(articleDate.Value);
									}
								}

								currentName = null;
								currentValue = null;
							}

							if (currentMovie != null)
							{
								result.Add(currentMovie);
								currentMovie = null;
							}
						}
					}

					// TODO: Exclude movies that are already in the list.  (or seek out movies from a list that was passed in)

					tableRows = doc.DocumentNode.SelectNodes("//body//article//h2[@style='text-align: center;'] | //body//article//p[contains(.,'million')]");

					if (tableRows != null)
					{
						bool foundHoldovers = false;

						foreach (var row in tableRows)
						{
							if (foundHoldovers)
							{
								var index = row.InnerText.IndexOf("million", row.InnerText.IndexOf("million") + 1);

								// Only want the row (paragraph) with "million" in it multiple times.

								if (index > 0)
								{
									// Match one or more digits, followed by a period and a space.
									// Gobble up (non-greedy using the ?) to 'cume' (meaning cumulative)
									var matches = Regex.Matches(row.InnerText, @"\d+\.\s.*?cume");

									foreach (Match match in matches)
									{
										var movie = new Movie();
										// \s - Match up to white space
										var titleMatch = Regex.Match(match.Value, @"\s(.*?)\$");
										var earningsMatch = Regex.Match(match.Value, @"\$\d+.*?\(");
										var fourDayMatch = Regex.Match(match.Value, @",\s\$\d+\.*\d*.*day");

										if (!string.IsNullOrEmpty(fourDayMatch.Value))
										{
											movie.Earnings = ParseEarnings(fourDayMatch.Value.Replace(",", string.Empty).Replace("(4-day", string.Empty));
										}
										else if (!string.IsNullOrEmpty(earningsMatch.Value))
										{
											movie.Earnings = ParseEarnings(earningsMatch.Value.Replace("(", string.Empty));
										}

										if (!string.IsNullOrEmpty(titleMatch.Value))
										{
											movie.Name = titleMatch.Value.Trim().Replace(" $", string.Empty);

											index = movie.Name.LastIndexOf("(");

											if (index > 0)
											{
												movie.Name = RemovePunctuation(HttpUtility.HtmlDecode(movie.Name.Substring(0, index)));
											}
										}

										if (movie.Name != null && movie.Earnings > 0)
										{
											if (articleDate.HasValue)
											{
												movie.WeekendEnding = MovieDateUtil.NextSunday(articleDate.Value);
											}

											result.Add(movie);
										}
									}

									break;
								}
							}

							if (row.Name.ToLower() == "h2" && row.InnerText.ToLower().Contains("holdover"))
							{
								foundHoldovers = true;
							}
						}
					}
				}
			}

			return result;
		}
	}
}