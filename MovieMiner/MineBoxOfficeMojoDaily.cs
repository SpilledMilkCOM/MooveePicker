using System.Collections.Generic;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using System;

namespace MovieMiner
{
	public class MineBoxOfficeMojoDaily : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficemojo.com/";

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

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var boxOfficeHistory = new List<IBoxOffice>();

			//https://www.boxofficemojo.com/daily/chart/?sortdate=2019-04-26&track=marvel2019.htm

			var weekendEnding = WeekendEnding?.ToString("");
			string url = $"{Url}daily/chart/?sortdate={WeekendEnding?.ToString("yyyy-MM-dd")}&track={Identifier}.htm";
			var web = new HtmlWeb();

			ContainsEstimates = false;

			var doc = web.Load(url);

			UrlSource = url;

			// Need to get the dates out of the header row.

			// Need to find the movie row using the Identifier
			// Had some trouble finding the ancestor so just traverse up the document.

			var tableRow = doc.DocumentNode?.SelectSingleNode($"//tr/td/b/a[@href='/movies/?page=daily&id={Identifier}.htm']")?.ParentNode?.ParentNode?.ParentNode;

			if (tableRow != null)
			{
				// This row should contain Rank, Title, Friday, Saturday, Sunday

				var rowColumns = tableRow.SelectNodes("td");

				if (rowColumns != null)
				{
					int columnCount = 0;

					foreach (var column in rowColumns)
					{
						IMovie movie = null;

						if (columnCount == 2)       // Friday
						{
							movie = new Movie
							{
								Day = DayOfWeek.Friday,
								Earnings = ParseEarnings(FirstToken(column.InnerText)),
								Identifier = Identifier,
								WeekendEnding = WeekendEnding.Value
							};
						}
						else if (columnCount == 3)  // Saturday
						{
							movie = new Movie
							{
								Day = DayOfWeek.Saturday,
								Earnings = ParseEarnings(FirstToken(column.InnerText)),
								Identifier = Identifier,
								WeekendEnding = WeekendEnding.Value
							};
						}
						else if (columnCount == 4)  // Sunday
						{
							movie = new Movie
							{
								Day = DayOfWeek.Sunday,
								Earnings = ParseEarnings(FirstToken(column.InnerText)),
								Identifier = Identifier,
								WeekendEnding = WeekendEnding.Value
							};
						}

						columnCount++;

						if (movie != null)
						{
							result.Add(movie);
						}
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

		private int ParseInt(string number)
		{
			return Convert.ToInt32(number.Replace(",", string.Empty));
		}
	}
}