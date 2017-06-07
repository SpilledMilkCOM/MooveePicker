using System;
using System.Diagnostics;

namespace MooveePicker
{
	[DebuggerDisplay("Name = {Name}")]
	public class Movie : IMovie
	{
		private decimal _cost;
		private decimal _earnings;
		private decimal _efficiency;

		public decimal Cost
		{
			get { return _cost; }
			set
			{
				_cost = value;
				UpdateEfficiency();
			}
		}

		public decimal Efficiency => _efficiency;

		public DateTime WeekendEnding { get; set; }

		public decimal Earnings
		{
			get { return _earnings; }
			set
			{
				_earnings = value;
				UpdateEfficiency();
			}
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		private void UpdateEfficiency()
		{
			if (Cost != 0)
			{
				_efficiency = Earnings / Cost;
			}
		}
	}
}