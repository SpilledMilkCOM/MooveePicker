using System;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IServerInfoModel
	{
		int MoviePosterFileCount { get; set; }

		DateTime Now { get; set; }

		DateTime NowUtc { get; set; }
	}
}