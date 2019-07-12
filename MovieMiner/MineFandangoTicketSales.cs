using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public class MineFandangoTicketSales : MinerBase
	{
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
				var id = 1;
				DateTime lastUpdated = DateTime.Now;

				if (DateTime.TryParse(lines[0].Replace("Updated by @akvalley:", string.Empty).Replace("Central (Lock time Fridays 11:00:00)", string.Empty), out lastUpdated))
				{
					LastUpdated = lastUpdated;
				}

				foreach (var line in lines.Skip(3))
				{
					var tokens = line?.Split(tokenDelimiters);

					if (tokens != null && tokens.Length == 3)
					{
						var movie = new Movie
						{
							Id = id,
							WeekendEnding = Convert.ToDateTime(tokens[0]),
							Earnings = Convert.ToDecimal(tokens[1]),
							Name = MapName(RemovePunctuation(tokens[2]))
						};
						result.Add(movie);
					}
				}
			}

			return result;
		}
	}
}