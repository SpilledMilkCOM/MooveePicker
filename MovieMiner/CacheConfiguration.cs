using System;

namespace MovieMiner
{
	public class CacheConfiguration : ICacheConfiguration
	{
		public CacheConfiguration()
		{
			Duration = new TimeSpan(1, 0, 0);       // 1 hour
			EmptyDuration = new TimeSpan(0, 2, 0);  // 2 minutes
		}

		public TimeSpan EmptyDuration { get; set; }

		public TimeSpan Duration { get; set; }
	}
}