using System.Collections.Generic;
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

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			UrlSource = $"{Url}/researchvault?section=box-office";

			var doc = web.Load(UrlSource);

			// Get the data in the table.

			var tableNode = doc.DocumentNode.SelectSingleNode("//body//table[@class='tableType-group hasGroups']");
			var tableRows = tableNode?.SelectNodes("thead//th[contains(@class, 'group')]");

			// Figure out which column to mine from the column title.

			//int columnToMine = 0;

			//tableRows = tableNode?.SelectNodes("tbody//td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");
			tableRows = tableNode?.SelectNodes("tbody//tr[contains(@class, 'group-')]");

			foreach (var tableRow in tableRows)
			{
				var nameNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'title')]");

				var movie = new Movie { Name = RemovePunctuation(HttpUtility.HtmlDecode(nameNode?.InnerText)) };

				// Grab the first one for now.

				var earningsNode = tableRow?.SelectSingleNode("td[@class='movie-earnings numeric stat']");

				if (earningsNode != null)
				{
					movie.Earnings = ParseEarnings(earningsNode.InnerText);
				}

				// Might as well grab the bux so the pick can be determined stand-alone

				var buxNode = tableRow?.SelectSingleNode("td[contains(@class, 'movie-title')]//span[contains(@class, 'cost')]");

				if (buxNode != null)
				{
					movie.Cost = ParseEarnings(HttpUtility.HtmlDecode(buxNode.InnerText).Replace("FB", string.Empty));
				}

				result.Add(movie);
			}

			foreach (var movie in result)
			{
				// Search for the table row that contains the name of the movie.
				var columns = tableNode?.SelectNodes($"tbody//tr[//span[text() = '{movie.Name}']]");
			}

			Movies = result;

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}