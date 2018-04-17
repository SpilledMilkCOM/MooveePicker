using System;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IServerInfoModel
	{
		int MoviePosterFileCount { get; set; }

		long ProcessBytes { get; set; }

		/// <summary>
		/// Local server "Now"
		/// </summary>
		DateTime Now { get; set; }

		/// <summary>
		/// Mountain Standard Time (MST) "Now" (should adjust for daylight savings?)
		/// </summary>
		DateTime NowMst { get; set; }

		/// <summary>
		/// Coordinated Universal Time (UTC) "Now"
		/// </summary>
		DateTime NowUtc { get; set; }

		int SharedFileCount { get; set; }

		int TwitterFileCount { get; set; }
	}
}