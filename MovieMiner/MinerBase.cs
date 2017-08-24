using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class MinerBase : IMiner
	{
		protected MinerBase(string name, string url)
		{
			Name = name;
			Url = url;
			Weight = 1;
		}

		public string Name { get; private set; }

		public string Url { get; private set; }

		public int Weight { get; private set; }

		public abstract List<IMovie> Mine();

		public abstract Task<List<IMovie>> MineAsync();

		protected string RemovePunctuation(string text)
		{
			return Regex.Replace(text, "[^\\w\\s]", string.Empty).Replace("-", string.Empty);
		}
	}
}