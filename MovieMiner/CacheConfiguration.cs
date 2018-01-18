using System;

namespace MovieMiner
{
	public class CacheConfiguration : ICacheConfiguration
	{
		public CacheConfiguration()
		{
			Duration = new TimeSpan(0, 30, 0);      // 30 minutes
			EmptyDuration = new TimeSpan(0, 2, 0);  // 2 minutes
		}

		public TimeSpan EmptyDuration { get; set; }

		public TimeSpan Duration { get; set; }
	}
}