using System.Collections.Generic;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public class MineFandangoTicketSalesFuture : MinerBase
	{
		/// <summary>
		/// Inject the URL to keep is "secret"
		/// </summary>
		/// <param name="weekendEnding">If this is null then the forecast will be mined.</param>
		public MineFandangoTicketSalesFuture(string url)
			: base("Fandango Tickets Sales (future)", "Fandango Futures", url)
		{
		}

		public override IMiner Clone()
		{
			return null;
		}

		public override List<IMovie> Mine()
		{
			// The file format of this file is the same as the "by day" one.

			var miner = new MineFandangoTicketSalesByDay(UrlSource);

			return miner.Mine();
		}
	}
}