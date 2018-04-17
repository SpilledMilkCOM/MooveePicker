using MoviePicker.WebApp.Interfaces;
using System;

namespace MoviePicker.WebApp.Models
{
	public class ServerInfoModel : IServerInfoModel
	{
		public int MoviePosterFileCount { get; set; }

		public DateTime Now { get; set; }

		public DateTime NowMst { get; set; }

		public DateTime NowUtc { get; set; }

		public long ProcessBytes { get; set; }

		public int SharedFileCount { get; set; }
	
		public int TwitterFileCount { get; set; }
	}
}