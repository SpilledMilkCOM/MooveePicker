using System;

namespace MovieMiner
{
	/// <summary>
	/// A cache needs to be able to load its data, and reload if the data is spoiled.
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// The cache configuration (how long until the data spoils, how often to check empty data)
		/// </summary>
		ICacheConfiguration CacheConfiguration { get; }

		/// <summary>
		/// When this cache expires
		/// </summary>
		DateTime? Expiration { get; }

		/// <summary>
		/// When this cache was loaded last.
		/// </summary>
		DateTime? LastLoaded { get; }

		void Expire();

		/// <summary>
		/// Load the cache with its data if the time is past the expiration.
		/// </summary>
		/// <param name="force"></param>
		void Load();
	}
}