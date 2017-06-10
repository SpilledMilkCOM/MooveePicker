using System;

namespace MoviePicker.Common.Interfaces
{
	public interface IMovie
	{
		decimal Cost { get; set; }

		decimal Efficiency { get; }

		DateTime WeekendEnding { get; set; }

		decimal Earnings { get; set; }

		int Id { get; set; }

		string Name { get; set; }

		IMovie Clone();

		int GetHashCode();
	}
}