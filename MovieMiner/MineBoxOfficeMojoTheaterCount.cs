using System;
using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeMojoTheaterCount : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficemojo.com/";
		private const string DELIMITER = "- $";
		private const string NO_DATA = "No Data";

		private DateTime? _weekendEnding;

		/// <summary>
		/// Gather the theater counts from Box Office Mojo
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineBoxOfficeMojoTheaterCount(DateTime? weekendEnding = null)
			: base($"Box Office Mojo {weekendEnding?.ToShortDateString()}"
				  , $"BO Mojo {weekendEnding?.ToShortDateString()}", DEFAULT_URL)
		{
			TwitterID = "BoxOfficeMojo";
			ContainsEstimates = false;
			_weekendEnding = weekendEnding?.Date;
		}

		public override IMiner Clone()
		{
			var result = new MineBoxOfficeMojoTheaterCount();

			Clone(result);

			result._weekendEnding = _weekendEnding;

			return result;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();

			if (_weekendEnding.HasValue)
			{
				var lastSunday = MovieDateUtil.GameSunday(null, ContainsEstimates).AddDays(-1);

				// Check to see if the weekend ending is out of date.

				if (ContainsEstimates || (_weekendEnding.Value < lastSunday && !ContainsEstimates))
				{
					_weekendEnding = lastSunday;
				}

				result = MineDate();
			}
			else
			{
				_weekendEnding = MovieDateUtil.GameSunday(null, ContainsEstimates);

				result = MineDate();
			}

			return result;
		}

		private List<IMovie> MineDate()
		{
			var result = new List<IMovie>();
			string url = $"{Url}counts/";
			var web = new HtmlWeb();

			ContainsEstimates = false;

			// Might have to tweak this offset a bit to get the numbers to match.
			var sundayOffset = (int)new DateTime(_weekendEnding.Value.Year, 1, 1).DayOfWeek;

			url = $"{Url}counts/chart/?yr={_weekendEnding.Value.Year}&wk={((_weekendEnding.Value.DayOfYear - sundayOffset) / 7) + 1}&p=.htm";

			var doc = web.Load(url);

			UrlSource = url;

			// Lookup XPATH to get the right node that matches.
			// Select all of the <script> nodes that are children of <body> with an attribute of "src"
			// REF: https://www.w3schools.com/xml/xpath_syntax.asp

			// TODO: Parse the header for column titles for mapping.

			//var tableRows = doc.DocumentNode?.SelectNodes("//table[@border='0' and @cellspacing='1' and @cellpadding='5']/tbody/tr[position()>1]");
			var tableRows = doc.DocumentNode?.SelectNodes("//table[@cellpadding='3']//tr[position()>1]");

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
								decimal theaterCount = 0;

								if (decimal.TryParse(column.InnerText?.Replace("-", "0"), out theaterCount))
								{
									movie.TheaterCount = (int)theaterCount;
								}
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

			return result;
		}
	}
}