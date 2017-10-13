using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;      // Handles crappy (NOT well formed) HTML

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineFantasyMovieLeagueBoxOffice : MinerBase
	{
		private const string DEFAULT_URL = "https://fantasymovieleague.com";

		public MineFantasyMovieLeagueBoxOffice(string articleTitle = null)
			: base("FML Box Office", "FMLBO", DEFAULT_URL)
		{
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var web = new HtmlWeb();

			var doc = web.Load($"{Url}/researchvault?section=box-office");

			//TODO - Put contains in here.

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}