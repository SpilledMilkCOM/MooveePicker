using System;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Common
{
	public class BoxOffice : IBoxOffice
	{
		public BoxOffice() { }

		public BoxOffice(IBoxOffice toCopy)
		{
			if (toCopy != null)
			{
				Earnings = toCopy.Earnings;
				Rank = toCopy.Rank;
				TheaterCount = toCopy.TheaterCount;
				WeekendEnding = toCopy.WeekendEnding;
			}
		}

		public decimal Earnings { get; set; }

		public int Rank { get; set; }

		public int TheaterCount { get; set; }

		public DateTime WeekendEnding { get; set; }

		public IBoxOffice Clone()
		{
			return new BoxOffice(this);
		}
	}
}