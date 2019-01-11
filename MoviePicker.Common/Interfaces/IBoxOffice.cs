using System;

namespace MoviePicker.Common.Interfaces
{
	public interface IBoxOffice
	{
		Decimal Earnings { get; set; }

		int Rank { get; set; }

		int TheaterCount { get; set; }

		DateTime WeekendEnding { get; set; }
	}
}