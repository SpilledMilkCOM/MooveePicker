using System;

namespace MovieMiner
{
	/// <summary>
	/// A cache needs to be able to load its data, and reload if the data is spoiled.
	/// </summary>
	public interface ICacheConfiguration
	{
		/// <summary>
		/// Amount of time between reread attempts when the cache is empty.
		/// </summary>
		TimeSpan EmptyDuration { get; set; }

		/// <summary>
		/// Amount of time to expire the loaded cache.
		/// </summary>
		TimeSpan Duration { get; set; }
	}
}