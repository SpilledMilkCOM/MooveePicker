using System;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IServerInfoModel
	{
		int MoviePosterFileCount { get; set; }

		DateTime Now { get; set; }

		DateTime NowUtc { get; set; }

		int SharedFileCount { get; set; }

		int TwitterFileCount { get; set; }
	}
}