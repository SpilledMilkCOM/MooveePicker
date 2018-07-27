using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using SM.Common.Utils;

namespace MooveePicker.Simulations
{
	/// <summary>
	/// Vary the earnings by some offsets and compute the BEST out of how many of that list WINS.
	/// The FULL version of this where it recursively tries ALL variants will require some SERIOUS compute power.
	/// </summary>
	public class MoviePickerVariantsAll : IMoviePicker
	{
		private const decimal EARNINGS_ADJUSTMENT = 0.03m;
		private const decimal EARNINGS_ADJUSTMENT_MAX = 0.03m;

		private readonly List<IMovie> _baselineMovies;
		private readonly Dictionary<int, int> _bestListCounts;          // Keyed using the hash code.
		private readonly Dictionary<int, IMovieList> _bestLists;        // Keyed using the hash code.
		private readonly ElapsedTime _elapsed;
		private readonly HashSet<int> _listGenerated;                   // Keyed using the hash code.
		private readonly ILogger _logger;
		private int _logMessagesCount;
		private readonly IMovieList _movieListPrototype;                // A Prototype injected so it can be cloned for new lists.
		private readonly IMoviePicker _moviePicker;

		public MoviePickerVariantsAll(IMovieList movieListPrototype, ILogger logger)
		{
			_bestListCounts = new Dictionary<int, int>();
			_bestLists = new Dictionary<int, IMovieList>();
			_elapsed = new ElapsedTime();
			_listGenerated = new HashSet<int>();
			_logMessagesCount = 0;
			_logger = logger;
			_moviePicker = new MsfMovieSolver { DisplayDebugMessage = false };
			//_moviePicker = new MoviePicker(new MovieList());
			_baselineMovies = new List<IMovie>();

			_movieListPrototype = movieListPrototype;

			EarningsAdjustmentByPercent = true;
			EarningsAdjustment = EARNINGS_ADJUSTMENT;
			EarningsAdjustmentMax = EARNINGS_ADJUSTMENT_MAX;
		}

		public IMovie BestPerformer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<IMovie> BestPerformers
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public decimal EarningsAdjustment { get; set; }

		public bool EarningsAdjustmentByPercent { get; set; }

		public decimal EarningsAdjustmentMax { get; set; }

		public bool EnableBestPerformer
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public int LogMessagesMax { get; set; }

		public IEnumerable<IMovie> Movies => _moviePicker.Movies;

		public int TotalComparisons { get; set; }

		public int TotalMovieLists { get; set; }

		public int TotalSubProblems { get; set; }

		public void AddMovies(IEnumerable<IMovie> movies)
		{
			// Need base copies of the movie earnings, so you need clones.

			foreach (var movie in movies)
			{
				_baselineMovies.Add(movie.Clone());
			}
		}

		public IMovieList ChooseBest()
		{
			int exponent = _baselineMovies.Count(item => item.AdjustEarnings);
			int toRaise = (int)(EarningsAdjustmentMax / EarningsAdjustment) * 2 + 1;

			_elapsed.Max = Math.Pow(toRaise, exponent);

			var movieLists = GenerateMovieLists(new List<IMovie>(), _baselineMovies);

			_logMessagesCount = 0;

			TotalMovieLists = movieLists.Count;

			_elapsed.Current = 0;
			_elapsed.Max = TotalMovieLists;
			_elapsed.Start();

			// Walk through the movie list adjusting the movies by the variant and then choose the best.

			foreach (var movieList in movieLists)
			{
				_moviePicker.AddMovies(movieList);

				if (CanLog)
				{
					// Only display this progress every LogMessagesMax time

					LogMovieList($"{nameof(ChooseBest)} - {_logMessagesCount}/{TotalMovieLists} - ", movieList);
				}

				_logMessagesCount++;
				_elapsed.Current = _logMessagesCount;

				var best = _moviePicker.ChooseBest();
				var hashCode = best.GetHashCode();
				int value;

				TotalComparisons += _moviePicker.TotalComparisons;
				TotalSubProblems += (_moviePicker.TotalSubProblems == 0) ? 1 : _moviePicker.TotalSubProblems;

				// Increment (or add to) the best list counts

				if (_bestListCounts.TryGetValue(hashCode, out value))
				{
					_bestListCounts[hashCode] = value + 1;
				}
				else
				{
					_bestListCounts.Add(hashCode, 1);
					_bestLists.Add(hashCode, best);
				}
			}

			// Sort through the MOST times a list is counted.

			var bestHash = _bestListCounts.OrderByDescending(item => item.Value).First().Key;

			return _bestLists[bestHash];
		}

		public int GetRankedMovieListCount(IMovieList movieList)
		{
			return _bestListCounts[movieList.GetHashCode()];
		}

		public List<IMovieList> GetRankedMovieLists(int takeCount = 10)
		{
			return _bestListCounts.OrderByDescending(pair => pair.Value)
								.Take(takeCount)
								.Select(keyValuePair => _bestLists[keyValuePair.Key])
								.ToList();
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		private bool CanLog => LogMessagesMax > 0 && _logMessagesCount > 0 && (_logMessagesCount % LogMessagesMax) == 0;

		public List<IMovieList> ChooseBest(int topCount)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Concatenate two lists and return a new list.
		/// </summary>
		/// <param name="list1"></param>
		/// <param name="list2"></param>
		/// <returns></returns>
		private List<IMovie> Concatenate(List<IMovie> list1, List<IMovie> list2)
		{
			var result = new List<IMovie>();

			result.AddRange(list1);
			result.AddRange(list2);

			return result;
		}

		private List<IMovie> Copy(IEnumerable<IMovie> toCopy)
		{
			return toCopy.Select(movie => movie.Clone()).ToList();
		}

		/// <summary>
		/// Generate a movie list given a fixed beginning of a list and the end of the list will be traversed and recursed.
		/// </summary>
		/// <param name="beginingOfList">The fixed part of the list</param>
		/// <param name="endOfList">Traverse the end of the list </param>
		/// <returns></returns>
		private List<List<IMovie>> GenerateMovieLists(List<IMovie> beginingOfList, List<IMovie> endOfList)
		{
			var result = new List<List<IMovie>>();
			var endOfListCopy = Copy(endOfList);

			if (CanLog)
			{
				// Only display this progress every LogMessagesMax time

				LogMovieList($"{nameof(GenerateMovieLists)} - {_logMessagesCount} - ", beginingOfList);
			}

			_logMessagesCount++;
			_elapsed.Current = _logMessagesCount;

			var movieToAdjust = RemoveFirst(endOfListCopy);

			if (endOfList.Count == 0)
			{
				result.Add(Concatenate(beginingOfList, endOfList));
			}
			else
			{
				var beginningOfListCopy = Copy(beginingOfList);     // Make copies since this is recursive.

				beginningOfListCopy.Add(movieToAdjust.Clone());

				result.AddRange(GenerateMovieLists(beginningOfListCopy, endOfListCopy));
			}

			// Loop through the end of the list and adjust the movies' earnings up and down by the earnings adjustment.

			if (movieToAdjust != null && movieToAdjust.AdjustEarnings)
			{
				var increment = EarningsAdjustment;

				// For the movie to adjust only go up or down by the increment (not to exceed the max)

				while (increment <= EarningsAdjustmentMax)
				{
					// Make a copy of the end of the list (since this is recursive), find the current movie, and remove it from the list and add it to the beginning of the list.
					var newBeginningOfList = Copy(beginingOfList);
					var movieToAdjustCopy = movieToAdjust.Clone();

					newBeginningOfList.Add(movieToAdjustCopy);

					// Adjust the earnings UP.

					if (EarningsAdjustmentByPercent)
					{
						movieToAdjustCopy.Earnings += movieToAdjustCopy.Earnings * increment;
					}
					else
					{
						movieToAdjustCopy.Earnings += increment;
					}

					result.AddRange(GenerateMovieLists(newBeginningOfList, endOfListCopy));

					movieToAdjustCopy = movieToAdjust.Clone();

					// Adjust the earnings DOWN.

					if (EarningsAdjustmentByPercent)
					{
						movieToAdjustCopy.Earnings -= movieToAdjustCopy.Earnings * increment;
					}
					else
					{
						movieToAdjustCopy.Earnings -= increment;
					}

					if (movieToAdjustCopy.Earnings > 0)
					{
							var newBeginningOfList2 = Copy(beginingOfList);

							newBeginningOfList2.Add(movieToAdjustCopy);

							// Recurse with NEW lists.

							result.AddRange(GenerateMovieLists(newBeginningOfList2, endOfListCopy));
					}

					increment += EarningsAdjustment;
				}
			}

			return result;
		}

		private void AddToListAlreadyGenerated(List<IMovie> movies)
		{
			var movieList = _movieListPrototype.Clone();

			movieList.Add(movies);

			_listGenerated.Add(movieList.GetHashCode());
		}

		/// <summary>
		///     Check the list's hash code to see if it's already been put into the list.
		/// </summary>
		/// <param name="movies">An unsorted list of movies.</param>
		/// <returns>True if the list has already been generated.</returns>
		private bool ListAlreadyGenerated(List<IMovie> movies)
		{
			var movieList = _movieListPrototype.Clone();

			movieList.Add(movies);

			return _listGenerated.Contains(movieList.GetHashCode());
		}

		private void LogMovieList(string operation, IEnumerable<IMovie> movies)
		{
			if (CanLog)
			{
				string list = operation;

				foreach (var movie in movies)
				{
					list += $"{movie.Abbreviation} ${movie.Earnings:N0}|";
				}

				_logger?.WriteLine(list);
				_logger?.WriteLine(_elapsed.FormatRemaining());
			}
		}

		private IMovie RemoveFirst(List<IMovie> list)
		{
			var result = list.FirstOrDefault();

			list.Remove(result);

			return result;
		}
	}
}