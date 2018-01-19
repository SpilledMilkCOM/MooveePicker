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
		private const string DEFAULT_URL = "https://fantasymovieleague.com";

		private readonly string _columnTitle;

		public MineFantasyMovieLeagueBoxOffice(string columnTitle = null)
			: base("FML Box Office", "FMLBO", DEFAULT_URL)
		{
			_columnTitle = columnTitle;
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
			bool isEstimate = false;

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

					isEstimate = tableHeader.InnerText.IndexOf("Estimated") >= 0;
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

			//int columnToMine = 0;

			//tableRows = tableNode?.SelectNodes("tbody//td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
			tableRows = tableNode?.SelectNodes("tbody//tr[contains(@class, 'group-')]");

			foreach (var tableRow in tableRows)
			{
				var id = GetIdFromClass(tableRow?.Attributes["class"]?.Value);
				var nameNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
				var imageNode = tableRow?.SelectSingleNode("td//div[contains(@class, 'proxy-img')]");

				var movie = new Movie { Id = id, Name = RemovePunctuation(HttpUtility.HtmlDecode(nameNode?.InnerText)) };

				// Grab the first one for now.

				var earningsNode = tableRow?.SelectSingleNode("td[@class='movie-earnings numeric stat']");

				if (earningsNode != null && isEstimate)
				{
					movie.Earnings = ParseEarnings(earningsNode.InnerText);
				}

				if (imageNode != null)
				{
					movie.ImageUrl = imageNode?.Attributes["data-img-src"]?.Value;
				}

				// Might as well grab the bux so the pick can be determined stand-alone

				var buxNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'cost')]");

				if (buxNode != null)
				{
					movie.Cost = ParseEarnings(HttpUtility.HtmlDecode(buxNode.InnerText).Replace("FB", string.Empty));
				}

				if (weekendEnding.HasValue)
				{
					//movie.WeekendEnding = weekendEnding.Value;
					movie.WeekendEnding = MovieDateUtil.GameSunday();
				}

				result.Add(movie);
			}

			foreach (var movie in result)
			{
				// Search for the table row that contains the name of the movie.
				var columns = tableNode?.SelectNodes($"tbody//tr[//span[text() = '{movie.Name}']]");
			}

			result = result.OrderByDescending(movie => movie.Cost).ToList();

			return result;
		}

		private int GetIdFromClass(string nodeClass)
		{
			int result = -1;
			var tokens = nodeClass.Split(new char[] { '-', ' ' });

			if (tokens.Length >=2)
			{
				int parsed = 0;

				if (int.TryParse(tokens[1], out parsed))
				{
					result = parsed;
				}
			}

			return result;
		}
	}
}