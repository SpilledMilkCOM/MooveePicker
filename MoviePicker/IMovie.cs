using System;

namespace MooveePicker
{
	public interface IMovie
	{
		decimal Cost { get; set; }
		DateTime WeekendEnding { get; set; }
		decimal Earnings { get; set; }
		int Id { get; set; }
		string Name { get; set; }
	}
}