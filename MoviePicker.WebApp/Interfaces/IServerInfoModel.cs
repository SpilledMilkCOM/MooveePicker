using System;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IServerInfoModel
	{
		IEnumerable<string> MoviePosterFiles { get; set; }

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

		IEnumerable<string> SharedFiles { get; set; }

		IEnumerable<string> TwitterFiles { get; set; }
	}
}