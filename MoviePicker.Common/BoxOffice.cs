using System;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Common
{
	public class BoxOffice : IBoxOffice
	{
		public decimal Earnings { get; set; }

		int Rank { get; set; }

		public int TheaterCount { get; set; }

		public DateTime WeekendEnding { get; set; }
	}
}