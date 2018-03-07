using System;

namespace MovieMiner
{
	public class CacheConfiguration : ICacheConfiguration
	{
		public CacheConfiguration()
		{
			Duration = new TimeSpan(2, 0, 0);        // 2 hours
			EmptyDuration = new TimeSpan(0, 30, 0);  // 30 minutes
		}

		public TimeSpan EmptyDuration { get; set; }

		public TimeSpan Duration { get; set; }
	}
}