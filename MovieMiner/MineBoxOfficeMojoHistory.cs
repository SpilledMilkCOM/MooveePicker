using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;
using System;

namespace MovieMiner
{
	public class MineBoxOfficeMojoHistory : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficemojo.com/";
		private const string DELIMITER = "- $";
		private const string NO_DATA = "No Data";

		private Dictionary<string, int> _monthMap = new Dictionary<string, int>(12);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeMojoHistory(string identifier = null)
			: base($"Box Office Mojo '{identifier}'"
				  , $"BO Mojo {identifier}", DEFAULT_URL)
		{
			Identifier = identifier;
			TwitterID = "BoxOfficeMojo";

			var beginOfYear = new DateTime(DateTime.Now.Year, 1, 1);

			for (int index = 0; index < 12; index++)
			{
				_monthMap.Add(beginOfYear.AddMonths(index).ToString("MMM"), index + 1);
			}
		}

		public string Identifier { get; set; }

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeMojoHistory(Identifier);

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var boxOfficeHistory = new List<IBoxOffice>();

			// https://www.boxofficemojo.com/movies/?page=weekend&id=alita.htm

			string url = $"{Url}movies/?page=weekend&id={Identifier}.htm";
			var web = new HtmlWeb();

			ContainsEstimates = false;

			var doc = web.Load(url);

			UrlSource = url;

			// Get table rows (skipping the first row - header)

			var tableRows = doc.DocumentNode?.SelectNodes("//table[@class='chart-wide']//tr[position()>1]");

			if (tableRows != null)
			{
				foreach (var row in tableRows)
				{
					IBoxOffice boxOffice = null;
					var rowColumns = row.SelectNodes("td");

					if (rowColumns != null)
					{
						int columnCount = 0;

						foreach (var column in rowColumns)
						{
							if (columnCount == 0)
							{
								var href = column.GetAttributeValue("href", null);      // The year is contained within the link.
								var year = DateTime.Now.Year;
								var weekendEnding = ParseEndDate(HttpUtility.HtmlDecode(column.InnerText), year);

								boxOffice = new BoxOffice { WeekendEnding = weekendEnding };
							}
							else if (columnCount == 1)
							{
								boxOffice.Rank = Convert.ToInt32(column.InnerText);
							}
							else if (columnCount == 2)
							{
								boxOffice.Earnings = ParseEarnings(column.InnerText);
							}
							else if (columnCount == 4)
							{
								boxOffice.TheaterCount = ParseInt(column.InnerText);

								break;
							}

							columnCount++;
						}
					}

					if (boxOffice != null)
					{
						boxOfficeHistory.Add(boxOffice);
					}
				}

				var movie = new Movie();

				movie.SetBoxOfficeHistory(boxOfficeHistory);

				result.Add(movie);		// A list of one (just to conform to the miner interface)
			}

			return result;
		}

		private int ParseInt(string number)
		{
			return Convert.ToInt32(number.Replace(",", string.Empty));
		}

		/// <summary>
		/// Parse a short date
		/// </summary>
		/// <param name="shortDate">A string/date range in the format MMM dd-dd</param>
		/// <param name="year"></param>
		/// <returns></returns>
		private DateTime ParseEndDate(string shortDate, int year)
		{
			var monthDays = shortDate.Split();
			var days = monthDays[1].Split(new char[] { '\u0096' });     // One of those extra long hyphens.
			var month = _monthMap[monthDays[0]];

			return new DateTime(year, month, int.Parse(days[1]));
		}
	}
}