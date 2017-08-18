using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
    public class MineNerd : Miner
    {
		//private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups/MonCompare/MonCompare2017SummerWeek12.js";
		private const string DEFAULT_URL = "http://analyzer.fmlnerd.com/lineups";

		public MineNerd()
			: base(DEFAULT_URL)
	    {
	    }

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();
			var xml = HttpRequestUtil.DownloadString(Url);

			Debug.WriteLine(xml);

			return result;
		}

		public async override Task<List<IMovie>> MineAsync()
	    {
		    var result = new List<IMovie>();
		    var xml = await HttpRequestUtil.DownloadStringAsync(Url);

			Debug.WriteLine(xml);

		    return result;
	    }

	    public override List<IMovie> Parse(string innerHtml)
	    {
		    return null;
	    }
    }
}