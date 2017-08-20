using System.Collections.Generic;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	// TODO: Might want to change the interface to send in a list of movies to look for to help out the scan.

	public interface IMiner
	{
		string Url { get; }

		List<IMovie> Mine();

		Task<List<IMovie>> MineAsync();
	}
}