using MoviePicker.WebApp.Interfaces;
using System;

namespace MoviePicker.WebApp.Models
{
	public class ServerInfoModel : IServerInfoModel
	{
		public int MoviePosterFileCount { get; set; }

		public DateTime Now { get; set; }

		public DateTime NowUtc { get; set; }
	}
}