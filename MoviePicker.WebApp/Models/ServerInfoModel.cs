using MoviePicker.WebApp.Interfaces;
using System;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class ServerInfoModel : IServerInfoModel
	{
		public IEnumerable<string> MoviePosterFiles { get; set; }

		public DateTime Now { get; set; }

		public DateTime NowMst { get; set; }

		public DateTime NowUtc { get; set; }

		public long ProcessBytes { get; set; }

		public IEnumerable<string> SharedFiles { get; set; }
	
		public IEnumerable<string> TwitterFiles { get; set; }
	}
}