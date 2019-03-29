using System;

namespace MoviePicker.Common.Interfaces
{
	public interface IBoxOffice
	{
		decimal Earnings { get; set; }

		int Rank { get; set; }

		int TheaterCount { get; set; }

		DateTime WeekendEnding { get; set; }

		IBoxOffice Clone();
	}
}