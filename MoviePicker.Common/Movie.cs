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
				Day = toCopy.Day;
				_earnings = toCopy._earnings;
				_cost = toCopy._cost;
				Id = toCopy.Id;
				MovieName = toCopy.MovieName;
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

		public DayOfWeek? Day { get; set; }

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

		public string MovieName { get; set; }

		public string Name
		{
			get { return (Day.HasValue) ? $"{MovieName} [{Day.Value}]" : MovieName; }
			set
			{
				// TODO: Possibly parse the name to find DayOfWeek.

				MovieName = value;
			}
		}

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
			var test = obj as IMovie;
			
			if (test != null)
			{
				// Make all the tests case insensitive.

				var movieName = MovieName.ToLower();
				var testMovieName = test.MovieName.ToLower();

				result = movieName.Equals(testMovieName);

				if (!result)
				{
					// Not an exact match so try starts with (limited contains)

					result = movieName.StartsWith(testMovieName) || testMovieName.StartsWith(movieName);
				}

				if (!result)
				{
					// Compare the first X characters

					int length = 10;

					if (movieName.Length < length)
					{
						length = movieName.Length;
					}

					if (testMovieName.Length < length)
					{
						length = testMovieName.Length;
					}

					result = movieName.Substring(0, length) == testMovieName.Substring(0, length);
				}

				if (result && Day.HasValue && test.Day.HasValue)
				{
					// If both days have values then they HAVE TO MATCH.

					result = Day.Value == test.Day.Value;
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