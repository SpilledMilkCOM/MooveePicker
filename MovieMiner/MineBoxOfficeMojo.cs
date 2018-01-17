using System;
using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeMojo : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficemojo.com/";
		private const string DELIMITER = "- $";

		private readonly DateTime? _weekendEnding;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeMojo(DateTime? weekendEnding = null)
			: base($"Box Office Mojo {weekendEnding?.ToShortDateString()}", $"BO Mojo {weekendEnding?.ToShortDateString()}", DEFAULT_URL)
		{
			_weekendEnding = weekendEnding?.Date;
		}

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeMojo();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			Movies = new List<IMovie>();

			if (_weekendEnding.HasValue)
			{
				Movies = MineDate();
			}
			else
			{
				Movies = MineForecast();
			}

			return Movies;
		}

		private List<IMovie> MineDate()
		{
			var result = new List<IMovie>();
			string url = $"{Url}weekend/chart/";
			var web = new HtmlWeb();

			// Might have to tweak this offset a bit to get the numbers to match.
			var sundayOffset = (int)new DateTime(_weekendEnding.Value.Year, 1, 1).DayOfWeek;

			url = $"{Url}weekend/chart/?view={_weekendEnding.Value.Year}&yr={_weekendEnding.Value.Year}&wknd={(_weekendEnding.Value.DayOfYear - sundayOffset) / 7}&p=.htm";

			var doc = web.Load(url);

			UrlSource = url;

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			// TODO: Parse the header for column titles for mapping.

			//var tableRows = doc.DocumentNode?.SelectNodes("//table[@border='0' and @cellspacing='1' and @cellpadding='5']/tbody/tr[position()>1]");
			var tableRows = doc.DocumentNode?.SelectNodes("//table[@cellpadding='5']//tr[position()>1]");

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
								movie = new Movie { Name = RemovePunctuation(HttpUtility.HtmlDecode(column.InnerText)) };

								if (_weekendEnding.HasValue)
								{
									movie.WeekendEnding = _weekendEnding.Value;
								}
							}
							else if (columnCount == 4)
							{
								movie.Earnings = decimal.Parse(column.InnerText?.Replace("$", string.Empty).Replace("-", "0"));
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

			Movies = result;

			return result;
		}

		private List<IMovie> MineForecast()
		{
			var result = new List<IMovie>();
			string url = Url;
			var web = new HtmlWeb();

			var doc = web.Load(url);

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			var node = doc.DocumentNode.SelectSingleNode("//body//div[@id='storyspc']/h2/a");

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

					node = doc.DocumentNode.SelectSingleNode("//body//font[@size='1']");

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

					// Need to scan for the <p> tag that contains "This weekend's forecast is directly below."

					// The movies are just in a <ul> tag (unsorted list)

					var movieNodes = doc.DocumentNode?.SelectNodes("//body//table//li");

					if (movieNodes == null)
					{
						Error = "No Data";
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

								if (!string.IsNullOrEmpty(movieName))
								{
									var name = MapName(RemovePunctuation(HttpUtility.HtmlDecode(movieName)));
									var movie = new Movie
									{
										MovieName = name,
										Earnings = decimal.Parse(estimatedBoxOffice) * (valueInMillions.Value ? 1000000 : 1)
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
										else
										{
											var found = result.Find(item => item.Equals(movie));

											if (found != null && found.EarningsBase < movie.EarningsBase)
											{
												// Replace the movie if a larger value was found. (4 day weekend versus 3 day)

												result.Remove(found);
												result.Add(movie);

												Error = "4-day";
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