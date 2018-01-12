using System;

namespace MovieMiner
{
	/// <summary>
	/// A cache needs to be able to load its data, and reload if the data is spoiled.
	/// </summary>
	public interface ICache
	{
		ICacheConfiguration CacheConfiguration { get; }

		/// <summary>
		/// When this cache expires
		/// </summary>
		DateTime? Expiration { get; }

		DateTime? LastLoaded { get; }

		/// <summary>
		/// Load the cache with its data.
		/// </summary>
		/// <param name="force"></param>
		void Load();
	}
}