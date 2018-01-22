using System;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IServerInfoModel
	{
		DateTime Now { get; set; }

		DateTime NowUtc { get; set; }
	}
}