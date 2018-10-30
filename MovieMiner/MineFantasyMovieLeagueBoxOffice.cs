using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

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

					ContainsEstimates = tableHeader.InnerText.IndexOf("Estimated") >= 0;
					var dateText = tableHeader.InnerText.Replace("Estimated", string.Empty);

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

			//tableRows = tableNode?.SelectNodes("tbody//td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
			tableRows = tableNode?.SelectNodes("tbody//tr[contains(@class, 'group-')]");

			foreach (var tableRow in tableRows)
			{
				var id = GetIdFromClass(tableRow?.Attributes["class"]?.Value);
				var nameNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
				var imageNode = tableRow?.SelectSingleNode("td//div[contains(@class, 'proxy-img')]");
				var name = RemovePunctuation(HttpUtility.HtmlDecode(nameNode?.InnerText));

				var movie = new Movie
				{
					Id = id,
					Day = ParseDayOfWeek(name),
					Name = ParseName(name)
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
					//movie.WeekendEnding = MovieDateUtil.GameSunday(null, isEstimate);
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

		private DayOfWeek? ParseDayOfWeek(string name)
		{
			DayOfWeek? result = null;

			if (name != null)
			{
				foreach (var pair in _daysOfWeek)
				{
					if (name.StartsWith(pair.Key))
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
					if (result.StartsWith(key))
					{
						// Remove the key

						result = result.Substring(key.Length, result.Length - key.Length);
						break;
					}
				}
			}

			return result;
		}
	}
}