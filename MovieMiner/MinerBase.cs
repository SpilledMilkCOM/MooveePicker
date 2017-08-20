using System.Collections.Generic;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class MinerBase : IMiner
	{
		protected MinerBase(string url)
		{
			Url = url;
		}

		public string Url { get; private set; }

		public abstract List<IMovie> Mine();

		public abstract Task<List<IMovie>> MineAsync();
	}
}