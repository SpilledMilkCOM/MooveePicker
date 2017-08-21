using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineBoxOfficeMojo : MinerBase
	{
		private const string DEFAULT_URL = "http://boxofficemojo.com/";

		private readonly DateTime? _weekendEnding;

		public MineBoxOfficeMojo(DateTime? weekendEnding = null)
			: base(DEFAULT_URL)
		{
			_weekendEnding = weekendEnding?.Date;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			// TODO: Use the weekend ending to figure out the actuals.

			var doc = web.Load($"{Url}weekend/chart/");

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
								movie = new Movie { Name = HttpUtility.HtmlDecode(column.InnerText) };
							}
							else if (columnCount == 4)
							{
								movie.Earnings = decimal.Parse(column.InnerText?.Replace("$", string.Empty));
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

		public async override Task<List<IMovie>> MineAsync()
		{
			throw new NotImplementedException();
		}
	}
}