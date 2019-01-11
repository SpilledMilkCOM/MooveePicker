using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public class MineFandangoTicketSalesByDay : MinerBase
	{
		private const decimal AVERAGE_COST_PER_TICKET = 10;

		private const string DEFAULT_URL = "http://akvalley.pythonanywhere.com/static/fandango_report.txt";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineFandangoTicketSalesByDay()
			: base("Fandango Tickets Sales (by day)", "Fandango Dailies", DEFAULT_URL)
		{
		}

		public override IMiner Clone()
		{
			return null;
		}

		public override List<IMovie> Mine()
		{
			var lineDelimiters = new[] { '\n' };
			var tokenDelimiters = new[] { '\t' };
			var result = new List<IMovie>();
			var data = HttpRequestUtil.DownloadTextFile(Url);
			var lines = data?.Split(lineDelimiters);

			if (lines != null)
			{
				DateTime lastUpdated = DateTime.Now;

				if (DateTime.TryParse(lines[0].Replace("Updated by @akvalley:", string.Empty).Replace("Central", string.Empty), out lastUpdated))
				{
					LastUpdated = lastUpdated;
				}

				foreach (var line in lines.Skip(3))
				{
					var tokens = line?.Split(tokenDelimiters);

					if (tokens != null && tokens.Length == 4)
					{
						var movie = new Movie
						{
							WeekendEnding = Convert.ToDateTime(tokens[1]),
							Earnings = Convert.ToDecimal(tokens[0]) * AVERAGE_COST_PER_TICKET,
							Name = tokens[3]
						};
						result.Add(movie);
					}
				}
			}

			return result;
		}
	}
}