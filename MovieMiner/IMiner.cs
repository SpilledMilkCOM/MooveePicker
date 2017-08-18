using System.Collections.Generic;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	interface IMiner
	{
		string Url { get; }

		List<IMovie> Mine();

		Task<List<IMovie>> MineAsync();

		List<IMovie> Parse(string innerHtml);
	}
}