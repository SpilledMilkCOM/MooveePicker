using System.Collections.Generic;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using System;
using System.Web;

namespace MovieMiner
{
	public class MineBoxOfficeMojoDaily : MinerBase
	{
		private const string DEFAULT_URL = MineBoxOfficeMojo.DEFAULT_URL;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeMojoDaily(string identifier = null, DateTime? weekendEnding = null)
			: base($"Box Office Mojo Daily '{identifier}'"
				  , $"BO Mojo Dly {identifier}", DEFAULT_URL)
		{
			Identifier = identifier;
			TwitterID = "BoxOfficeMojo";
			WeekendEnding = weekendEnding;
		}

		public string Identifier { get; set; }

		public DateTime? WeekendEnding { get; set; }

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeMojoDaily(Identifier);

			Clone(result);

			return result;
		}

		/// <summary>
		/// Returns a list of daily values based on the WeekendEnding
		/// </summary>
		/// <returns></returns>
		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();

			//https://www.boxofficemojo.com/daily/chart/?sortdate=2019-04-26&track=marvel2019.htm

			string url = $"{Url}{Identifier}";
			var web = new HtmlWeb();

			ContainsEstimates = false;

			var doc = web.Load(url);

			UrlSource = url;

			// Need to get the dates out of the header row.

			// Need to find the movie row using the Identifier
			// Had some trouble finding the ancestor so just traverse up the document.

			var tableRow = doc.DocumentNode?.SelectSingleNode($"//tr[position()>1]");		// The most recent one.

			if (tableRow != null)
			{
				// This row should contain Rank, Title, Friday, Saturday, Sunday

				var rowColumns = tableRow.SelectNodes("td");

				if (rowColumns != null)
				{
					IMovie movie = null;
					int columnCount = 0;

					foreach (var column in rowColumns)
					{
						if (columnCount == 0)       // Date
						{
							movie = new Movie
							{
								Identifier = Identifier,
								WeekendEnding = ParseEndDate(HttpUtility.HtmlDecode(column.InnerText))
							};

							movie.Day = movie.WeekendEnding.DayOfWeek;
							movie.WeekendEnding = MovieDateUtil.GameSunday(movie.WeekendEnding);		// These movies should all have the same WeekendEnding
						}
						else if (columnCount == 3)  // Earnings
						{
							movie.Earnings = ParseEarnings(FirstToken(column.InnerText));
						}

						columnCount++;
					}

					if (movie != null && movie.WeekendEnding == WeekendEnding)      // Only want the matching movies in the Weekend provided.
					{
						result.Add(movie);
					}
				}
			}

			return result;
		}

		private string FirstToken(string text, char delimiter = '\n')
		{
			var tokens = text.Split(new char[] { delimiter });

			return tokens[0];
		}

		private DateTime ParseEndDate(string date)
		{
			var result = new DateTime();

			DateTime.TryParse(date, out result);        // Won't throw error.

			return result;
		}
	}
}