using MoviePicker.Common.Interfaces;
using System;
using System.Diagnostics;

namespace MoviePicker.Common
{
	[DebuggerDisplay("Name = {Name}")]
	public class Movie : IMovie
	{
		private const decimal BEST_PERFORMER_BONUS = 2000000;

		private decimal _cost;
		private decimal _earnings;
		private decimal _efficiency;
		private bool _isBestPerformer;
		private decimal _originalEarnings;

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
				_originalEarnings = toCopy._originalEarnings;
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
				_originalEarnings = value;
				UpdateEfficiency();
			}
		}

		public int Id { get; set; }

		public bool IsBestPerformer
		{
			get { return _isBestPerformer; }
			set
			{
				_earnings = _originalEarnings + ((value) ? BEST_PERFORMER_BONUS : 0m);
				UpdateEfficiency();
				_isBestPerformer = value;
			}
		}

        public string Name { get; set; }

		public DateTime WeekendEnding { get; set; }

        public IMovie Clone()
		{
			return new Movie(this);
		}

		/// <summary>
		/// I may move this to a utility that strictly maps movies versus this generic "fuzzy" logic.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool result = false;
			var test = obj as Movie;
			
			if (test != null)
			{
				result = Name.Equals(test.Name);

				if (!result)
				{
					// Not an exact match so try starts with (limited contains)

					result = Name.StartsWith(test.Name) || test.Name.StartsWith(Name);
				}

				if (!result)
				{
					// Compare the first X characters

					int length = 10;

					if (Name.Length < length)
					{
						length = Name.Length;
					}

					if (test.Name.Length < length)
					{
						length = test.Name.Length;
					}

					result = Name.Substring(0, length) == test.Name.Substring(0, length);
				}
			}

			return result;
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

		    _isBestPerformer = false;
		}
	}
}