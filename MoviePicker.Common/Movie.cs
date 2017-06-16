using MoviePicker.Common.Interfaces;
using System;
using System.Diagnostics;

namespace MoviePicker.Common
{
	[DebuggerDisplay("Name = {Name}")]
	public class Movie : IMovie
	{
		private decimal _cost;
		private decimal _earnings;
		private decimal _efficiency;

		public Movie()
		{
			AdjustEarnings = true;
		}

		/// <summary>
		/// Copy constructor for Clone() needs to be private, otherwise the IoC will be confused.
		/// </summary>
		/// <param name="toCopy"></param>
		private Movie(Movie toCopy)
			: this()
		{
			if (!ReferenceEquals(this, toCopy))
			{
				AdjustEarnings = toCopy.AdjustEarnings;
				_earnings = toCopy._earnings;
				_cost = toCopy._cost;
				Id = toCopy.Id;
				Name = toCopy.Name;
				WeekendEnding = toCopy.WeekendEnding;

				UpdateEfficiency();
			}
		}

		public bool AdjustEarnings { get; set; }

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

		public DateTime WeekendEnding { get; set; }

		public IMovie Clone()
		{
			return new Movie(this);
		}

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