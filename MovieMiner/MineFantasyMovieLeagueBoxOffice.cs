﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using MovieMiner.Util;

namespace MovieMiner
{
	public class MineFantasyMovieLeagueBoxOffice : MinerBase
	{
		// TODO: Add a miner for BO Mojo  https://www.boxofficemojo.com/weekend/chart/
		// Estimates: https://www.boxofficemojo.com/weekend/chart/?view=studioest&yr=2018&wknd=43&p=.htm

		private const string DEFAULT_URL = "https://fantasymovieleague.com";

		private readonly string _columnTitle;
		private readonly Dictionary<string, DayOfWeek> _daysOfWeek;

		public MineFantasyMovieLeagueBoxOffice(string columnTitle = null)
			: base("FML Box Office", "FMLBO", DEFAULT_URL)
		{
			_columnTitle = columnTitle;

			_daysOfWeek = new Dictionary<string, DayOfWeek>
			{
				{"FRI ", DayOfWeek.Friday},
				{"SAT ", DayOfWeek.Saturday},
				{"SUN ", DayOfWeek.Sunday},
				{"MON ", DayOfWeek.Monday}
			};
		}

		public override IMiner Clone()
		{
			var result = new MineFantasyMovieLeagueBoxOffice();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();
			DateTime? weekendEnding = null;

			UrlSource = $"{Url}/researchvault?section=box-office";

			var doc = web.Load(UrlSource);

			// Get the data in the table.

			var tableNode = doc.DocumentNode.SelectSingleNode("//body//table[@class='tableType-group hasGroups']");
			var tableRows = tableNode?.SelectNodes("thead//th[contains(@class, 'group')]");

			// Figure out which column to mine from the column title.

			if (tableRows != null)
			{
				foreach (var tableHeader in tableRows)
				{
					// Grab the first one for now.

					ContainsEstimates = tableHeader.InnerText.ToLower().IndexOf("estimated") >= 0;
					var dateText = tableHeader.InnerText.ToLower().Replace("estimated", string.Empty);

					if (dateText != null)
					{
						char[] delimiter = { '-' };
						var dateChunks = dateText.Split(delimiter);

						if (dateChunks.Length > 0)
						{
							weekendEnding = Convert.ToDateTime(dateChunks[0]);
							weekendEnding = MovieDateUtil.ThisSunday(weekendEnding);
						}
					}

					break;
				}
			}

			tableRows = tableNode?.SelectNodes("tbody//tr[contains(@class, 'group-')]");

			foreach (var tableRow in tableRows)
			{
				var id = GetIdFromClass(tableRow?.Attributes["class"]?.Value);
				var nameNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
				var imageNode = tableRow?.SelectSingleNode("td//div[contains(@class, 'proxy-img')]");
				var name = RemovePunctuation(HttpUtility.HtmlDecode(nameNode?.InnerText));
				var dayOfWeek = ParseDayOfWeek(name);
				var movieName = ParseName(MapName(name), dayOfWeek);

				var movie = new Movie
				{
					Id = id,
					Day = dayOfWeek,
					Name = movieName
				};

				// Grab the first one for now.

				var earningsNode = tableRow?.SelectSingleNode("td[@class='movie-earnings numeric stat']");

				if (earningsNode != null)
				{
					movie.Earnings = ParseEarnings(earningsNode.InnerText);
				}

				if (imageNode != null)
				{
					movie.ImageUrl = imageNode?.Attributes["data-img-src"]?.Value;

					// Not able to download using https.

					movie.ImageUrlSource = movie.ImageUrl.Replace("https://", "http://");
				}

				// Might as well grab the bux so the pick can be determined stand-alone

				var buxNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'cost')]");

				if (buxNode != null)
				{
					movie.Cost = ParseEarnings(HttpUtility.HtmlDecode(buxNode.InnerText).Replace("FB", string.Empty));
				}

				if (weekendEnding.HasValue)
				{
					// This weekend ending date is used to populate the "custom" box office weekend ending date.
					movie.WeekendEnding = ContainsEstimates ? weekendEnding.Value : MovieDateUtil.GameSunday(null, ContainsEstimates);
				}

				result.Add(movie);
			}

			foreach (var movie in result)
			{
				// Search for the table row that contains the name of the movie.
				var columns = tableNode?.SelectNodes($"tbody//tr[//span[text() = '{movie.Name}']]");
			}

			result = result.OrderByDescending(movie => movie.Cost).ToList();

			// Assign the control ids for the HTML controls, (if movie controls are in an array).

			var controlIndex = 1;

			result.ForEach(movie => movie.ControlId = controlIndex++);

			var gameDateRange = GetGameDateRange(web);

			if (gameDateRange.End.HasValue)
			{
				result.ForEach(item => item.WeekendEnding = gameDateRange.End.Value);
			}

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private int GetIdFromClass(string nodeClass)
		{
			int result = -1;
			var tokens = nodeClass.Split(new char[] { '-', ' ' });

			if (tokens.Length >= 2)
			{
				int parsed = 0;

				if (int.TryParse(tokens[1], out parsed))
				{
					result = parsed;
				}
			}

			return result;
		}

		private DateRange GetGameDateRange(HtmlWeb web)
		{
			DateRange result = new DateRange();

			UrlSource = $"{Url}/researchvault?section=pick-rate";

			var doc = web.Load(UrlSource);

			// Get the data in the table.

			var tableNode = doc.DocumentNode.SelectSingleNode("//body//table[@class='tableType-group hasGroups']");
			var tableRows = tableNode?.SelectNodes("thead//th[contains(@class, 'group')]");

			// Figure out which column to mine from the column title.

			if (tableRows != null && tableRows.Count > 0)
			{
				var dateText = tableRows[0].InnerText;

				if (dateText != null)
				{
					// Examples of dateText:
					//		Jan 1 - 3
					//		Jan 30 - Feb 1
					//		Dec 30 - Jan 1		(could span a year)
					//		May 31 - 2			(WHY do this now? 05/31/2019, but they DID IT!)

					char[] delimiter = { '-' };
					var dateChunks = dateText.Split(delimiter);

					if (dateChunks.Length > 1)
					{
						result.Start = Convert.ToDateTime(dateChunks[0]);
						DateTime parsedDate = result.Start.Value;

						result.End = parsedDate;

						if (DateTime.TryParse(dateChunks[1], out parsedDate))
						{
							if (result.Start.Value.Month == 12)
							{
								new DateTime(result.Start.Value.Year + 1, parsedDate.Month, parsedDate.Day);
							}
							else
							{
								result.End = parsedDate;
							}
						}
						else
						{
							// The second piece was just a number.

							var dayOfMonth = Convert.ToInt32(dateChunks[1]);

							if (dayOfMonth > result.Start.Value.Day)
							{
								result.End = new DateTime(result.Start.Value.Year, result.Start.Value.Month, dayOfMonth);
							}
							else
							{
								// The number was smaller which signifies the NEXT month.

								var nextMonth = result.Start.Value.AddMonths(1);

								result.End = new DateTime(nextMonth.Year, nextMonth.Month, dayOfMonth);
							}
						}
					}
				}
			}

			return result;
		}

		private DayOfWeek? ParseDayOfWeek(string name)
		{
			DayOfWeek? result = null;

			if (name != null)
			{
				foreach (var pair in _daysOfWeek)
				{
					if (name.StartsWith(pair.Key))
					{
						result = pair.Value;
						break;
					}
				}

				if (result == null)
				{
					var tokens = name.Split();

					if (tokens.Length > 1)
					{
						foreach (var pair in _daysOfWeek)
						{
							if (name.StartsWith(pair.Key.Replace(" ", string.Empty)))
							{
								result = pair.Value;
								break;
							}
						}
					}
				}
			}

			return result;
		}

		private string ParseName(string name, DayOfWeek? dayOfWeek)
		{
			var result = name;

			if (result != null && dayOfWeek != null)
			{
				var idx = name.IndexOf(' ');

				if (idx > 0)
				{
					idx++;
					result = result.Substring(idx, name.Length - idx);
				}
			}

			return result;
		}
	}
}