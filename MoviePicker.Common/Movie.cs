using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MoviePicker.Common
{
	[DebuggerDisplay("Id = {Id} - Name = {Name} ${Earnings}")]
	public class Movie : IMovie
	{
		private const decimal BEST_PERFORMER_BONUS = 2000000;
		private const string HASHTAG = "#";

		private decimal _cost;
		private decimal _earnings;
		private bool _isBestPerformer;
		private string _movieName;
		private IList<IBoxOffice> _boxOfficeHistory;

		public Movie()
		{
			AdjustEarnings = true;

			ScreenCount = -1;       // Not set.
			TheaterCount = -1;
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
				ControlId = toCopy.ControlId;
				Day = toCopy.Day;
				_earnings = toCopy._earnings;
				_cost = toCopy._cost;
				Id = toCopy.Id;
				ImageUrl = toCopy.ImageUrl;
				MovieName = toCopy.MovieName;
				EarningsBase = toCopy.EarningsBase;
				TheaterCount = toCopy.TheaterCount;
				WeekendEnding = toCopy.WeekendEnding;

				SetBoxOfficeHistory(toCopy.BoxOfficeHistory);

				UpdateEfficiency();

				IsBestPerformer = toCopy.IsBestPerformer;
			}
		}

		public string Abbreviation
		{
			get
			{
				var tokens = Day.HasValue ? MovieName.Split() : Name.Split();
				var result = new StringBuilder();

				if (tokens.Length > 1)
				{
					foreach (var token in tokens)
					{
						if (!string.IsNullOrEmpty(token))
						{
							result.Append(token.Substring(0, 1).ToUpper());
						}
					}
				}
				else if (tokens[0].Length > 5)
				{
					var vowels = new string[] { "a", "e", "i", "o", "u", "y" };
					var name = tokens[0];

					// Remove the vowels. (if the word is longer than 5 characters)

					foreach (var vowel in vowels)
					{
						name = name.Replace(vowel, string.Empty).Replace(vowel.ToUpper(), string.Empty);
					}

					result.Append(name);
				}

				if (Day.HasValue)
				{
					var textInfo = new CultureInfo("en-US", false).TextInfo;

					result.Append($"-{textInfo.ToTitleCase(Day.Value.ToString().Substring(0, 3))}");
				}

				return result.ToString();
			}
		}

		/// <summary>
		/// Used within simulations to whether or not that this movie's earnings is adjusted (for the simulation)
		/// </summary>
		public bool AdjustEarnings { get; set; }

		public IEnumerable<IBoxOffice> BoxOfficeHistory => _boxOfficeHistory;

		public int ControlId { get; set; }

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

		public decimal Efficiency { get; private set; }

		public decimal Earnings
		{
			get { return _earnings; }
			set
			{
				_earnings = value;
				EarningsBase = value;
				UpdateEfficiency();
			}
		}

		/// <summary>
		/// The base earnings which does NOT include any bonus (also "original earnings")
		/// </summary>
		public decimal EarningsBase { get; private set; }

		public string Hashtag { get; private set; }

		public int Id { get; set; }

		public string Identifier { get; set; }

		public string ImageUrl { get; set; }

		public string ImageUrlSource { get; set; }

		public bool IsBestPerformer
		{
			get { return _isBestPerformer; }
			set
			{
				_earnings = EarningsBase + ((value) ? BEST_PERFORMER_BONUS : 0m);
				UpdateEfficiency();
				_isBestPerformer = value;
			}
		}

		public bool IsNew { get; set; }

		public string MovieName
		{
			get
			{
				return _movieName;
			}
			set
			{
				_movieName = value;
				UpdateHashtag();
			}
		}

		public string Name
		{
			get { return (Day.HasValue) ? $"{MovieName} [{Day.Value}]" : MovieName; }
			set
			{
				MovieName = value;
			}
		}

		public int ScreenCount { get; set; }

		public int TheaterCount { get; set; }

		public decimal TheaterEfficiency => TheaterCount > 0 ? EarningsBase / TheaterCount : 0;

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
			var compareTo = obj as IMovie;

			if (compareTo != null)
			{
				if (Id != 0 && Id == compareTo.Id)
				{
					result = true;
				}
				else if(Identifier != null && compareTo.Identifier != null)
				{
					// The Identifier will trump all other equalities if they are set.
					result = Identifier == compareTo.Identifier;

					if (result && Day.HasValue && compareTo.Day.HasValue)
					{
						// If both days have values then they HAVE TO MATCH.

						result = Day.Value == compareTo.Day.Value;
					}
				}
				else
				{
					// Make all the tests case insensitive.

					var movieName = MovieName.ToLower().Replace(" ", string.Empty);
					var testMovieName = compareTo.MovieName.ToLower().Replace(" ", string.Empty);

					result = movieName.Equals(testMovieName);

					if (!result)
					{
						// Not an exact match so try starts with (limited contains)

						result = movieName.StartsWith(testMovieName) || testMovieName.StartsWith(movieName);
					}

					if (!result)
					{
						// Not an exact match so try ends with (limited contains)

						result = movieName.EndsWith(testMovieName) || testMovieName.EndsWith(movieName);
					}

					if (!result)
					{
						// Not an exact match so try "contains"

						result = movieName.IndexOf(testMovieName) > 1 || testMovieName.IndexOf(movieName) > 1;
					}

					if (result)
					{
						// Fail if there is a lot of noise.

						if (movieName.Length > testMovieName.Length * 2 || testMovieName.Length > movieName.Length * 2)
						{
							// Try  to remove some of the noise.

							var thisYear = $" {DateTime.Now.Year}";

							movieName = movieName.Replace(thisYear, string.Empty);
							testMovieName = testMovieName.Replace(thisYear, string.Empty);

							if (movieName.Length > testMovieName.Length * 2 || testMovieName.Length > movieName.Length * 2)
							{
								result = false;
							}
						}
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

					if (!result)
					{
						// Try to compare the names without the word "the"

						result = movieName.Replace("the ", string.Empty).Replace(" the", string.Empty).Equals(testMovieName.Replace("the ", string.Empty).Replace(" the", string.Empty));
					}

					//if (!result)
					//{
					//	char[] delimiters = " ".ToCharArray();

					//	// Compare words.
					//	var movieWords = MovieName.Split(delimiters);
					//	var testWords = testMovieName.Split(delimiters);
					//	int matches = 0;

					//	for (int index = 0; index < movieWords.Length; index++)
					//	{
					//		for (int i = 0; i < testWords.Length; i++)
					//		{
					//			if(movieWords[index] != null && movieWords[index] == testWords[i])
					//			{
					//				// Only needs to match once.
					//				matches++;
					//				break;
					//			}
					//		}
					//	}

					//	result = movieWords.Length > 0 && matches / movieWords.Length >= 0.666666;
					//}

					if (result && Day.HasValue && compareTo.Day.HasValue)
					{
						// If both days have values then they HAVE TO MATCH.

						result = Day.Value == compareTo.Day.Value;
					}
				}
			}

			return result;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		/// <summary>
		/// Copy/Clone the history passed in.  (supports null too)
		/// </summary>
		/// <param name="history"></param>
		public void SetBoxOfficeHistory(IEnumerable<IBoxOffice> history)
		{
			if (history == null)
			{
				_boxOfficeHistory = null;
			}
			else
			{
				_boxOfficeHistory = new List<IBoxOffice>();

				foreach (var item in history)
				{
					_boxOfficeHistory.Add(item.Clone());
				}
			}
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void UpdateHashtag()
		{
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

			// The movie name should already have the punctuation removed.
			// Add "Movie" to the end?

			Hashtag = HASHTAG + textInfo.ToTitleCase(_movieName).Replace(" ", string.Empty);
		}

		private void UpdateEfficiency()
		{
			if (Cost != 0)
			{
				Efficiency = EarningsBase / Cost;
			}

			_isBestPerformer = false;
		}
	}
}