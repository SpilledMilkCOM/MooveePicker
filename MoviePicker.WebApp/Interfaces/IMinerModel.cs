using MovieMiner;
using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IMinerModel
	{
		/// <summary>
		/// The list of miners with their data.
		/// </summary>
		List<IMiner> Miners { get; }

		/// <summary>
		/// Clones all of the miners.  If the data is stale, then the miner will be populated fresh data.
		/// </summary>
		/// <returns></returns>
		IMinerModel Clone();

		List<IMiner> CreateMinersWithData();

		List<IMovie> CreateWeightedList();

		void Expire();
	}
}