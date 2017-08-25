using System.Collections.Generic;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	// TODO: Might want to change the interface to send in a list of movies to look for to help out the scan.

	public interface IMiner
	{
		/// <summary>
		/// Name of the Miner
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Typically the root Url for the initial page.
		/// </summary>
		string Url { get; }

		/// <summary>
		/// Used for weighted average.
		/// </summary>
		int Weight { get; set; }

		List<IMovie> Mine();

		Task<List<IMovie>> MineAsync();
	}
}