using System;
using System.Diagnostics;

namespace MooveePicker
{
	[DebuggerDisplay("Name = {Name}")]
    public class Movie : IMovie
    {
		public decimal Cost { get; set; }

		public DateTime WeekendEnding { get; set; }

		public decimal Earnings { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}