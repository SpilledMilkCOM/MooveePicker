using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public class MineFandangoTicketSales : MinerBase
	{
		private const decimal AVERAGE_COST_PER_TICKET = 10;

		private const string DEFAULT_URL = "http://akvalley.pythonanywhere.com/static/Fandango_track.txt";
		private const string DELIMITER = "- $";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineFandangoTicketSales()
			: base("Fandango Tickets Sales", "Fandango", DEFAULT_URL)
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
				foreach (var line in lines.Skip(3))
				{
					var tokens = line?.Split(tokenDelimiters);

					if (tokens != null && tokens.Length == 3)
					{
						var movie = new Movie
						{
							WeekendEnding = Convert.ToDateTime(tokens[0]),
							Earnings = Convert.ToDecimal(tokens[1]) * AVERAGE_COST_PER_TICKET,
							Name = tokens[2]
						};
						result.Add(movie);
					}
				}
			}

			return result;
		}
	}
}