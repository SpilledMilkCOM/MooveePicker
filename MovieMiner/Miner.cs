using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class Miner : IMiner
	{
		protected Miner(string url)
		{
			Url = url;
		}

		public string Url { get; private set; }

		public abstract List<IMovie> Mine();

		public abstract Task<List<IMovie>> MineAsync();

		public virtual List<IMovie> Parse(string innerHtml) { throw new NotImplementedException(); }
	}
}