using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using SM.Common.Utils;

namespace MooveePicker
{
    /// <summary>
    /// Vary the earnings by some offsets and compute the BEST out of how many of that list WINS.
    /// The FULL version of this where it recursively tries ALL variants will require some SERIOUS compute power.
    /// </summary>
    public class MoviePickerVariantsAll : IMoviePicker
    {
        private const decimal EARNINGS_ADJUSTMENT = 0.1m;
        private const decimal EARNINGS_VARIANT_MAX = 0.1m;

        private const decimal EARNINGS_ADJUSTMENT_PERCENT = 0.001m;
        private const decimal EARNINGS_VARIANT_PERCENT_MAX = 0.03m;

        private readonly List<IMovie> _baselineMovies;
        private readonly Dictionary<int, int> _bestListCounts;          // Keyed using the hash code.
        private readonly Dictionary<int, IMovieList> _bestLists;        // Keyed using the hash code.
        private readonly ElapsedTime _elapsed;
        private readonly HashSet<int> _listGenerated;                   // Keyed using the hash code.
	    private readonly ILogger _logger;
		private int _logMessagesCount;
		private readonly IMovieList _movieListPrototype;
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

        public bool EarningsAdjustmentByPercent { get; set; }

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
                    LogMovieList($"{nameof(ChooseBest)} - {_logMessagesCount}/{TotalMovieLists} - ", movieList);
                }

                _logMessagesCount++;
                _elapsed.Current = _logMessagesCount;

                var best = _moviePicker.ChooseBest();
                var hashCode = best.GetHashCode();
                int value;

                TotalComparisons += _moviePicker.TotalComparisons;
                TotalSubProblems += (_moviePicker.TotalSubProblems == 0) ? 1 : _moviePicker.TotalSubProblems;

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

		private bool CanLog => LogMessagesMax > 0 && (_logMessagesCount % LogMessagesMax) == 0;


		private List<IMovie> Copy(IEnumerable<IMovie> toCopy)
        {
            return toCopy.Select(movie => movie.Clone()).ToList();
        }

        private List<List<IMovie>> GenerateMovieLists(List<IMovie> beginingOfList, List<IMovie> endOfList)
        {
            var result = new List<List<IMovie>>();
            decimal earningsAdjustment = (EarningsAdjustmentByPercent) ? EARNINGS_ADJUSTMENT_PERCENT : EARNINGS_ADJUSTMENT;
            decimal earningsAdjustmentMax = (EarningsAdjustmentByPercent) ? EARNINGS_VARIANT_PERCENT_MAX : EARNINGS_VARIANT_MAX;

            // Prevent recursing on lists that have already been generated.
            AddToListAlreadyGenerated(beginingOfList);

            result.Add(Concatenate(beginingOfList, endOfList));

            if (CanLog)
            {
                LogMovieList($"{nameof(GenerateMovieLists)} - {_logMessagesCount} - ", beginingOfList);
            }

            _logMessagesCount++;

            foreach (var movie in endOfList)
            {
                if (movie.AdjustEarnings)
                {
                    var increment = earningsAdjustment;

                    while (increment <= earningsAdjustmentMax)
                    {
                        var newEndOfList = Copy(endOfList);
                        var movieToAdjust = newEndOfList.First(item => item.Id == movie.Id);

                        if (EarningsAdjustmentByPercent)
                        {
                            movieToAdjust.Earnings += movieToAdjust.Earnings * increment;
                        }
                        else
                        {
                            movieToAdjust.Earnings += increment * 1000000m;
                        }

                        newEndOfList.Remove(movieToAdjust);

                        var newBeginningOfList = Copy(beginingOfList);

                        newBeginningOfList.Add(movieToAdjust);

                        if (!ListAlreadyGenerated(newBeginningOfList))
                        {
                            result.AddRange(GenerateMovieLists(newBeginningOfList, newEndOfList));
                        }

                        var newEndOfList2 = Copy(endOfList);
                        var movieToAdjust2 = newEndOfList2.First(item => item.Id == movie.Id);

                        if (EarningsAdjustmentByPercent)
                        {
                            movieToAdjust.Earnings -= movieToAdjust.Earnings * increment;
                        }
                        else
                        {
                            movieToAdjust2.Earnings -= increment * 1000000m;
                        }

                        if (movieToAdjust2.Earnings > 0)
                        {
                            newEndOfList2.Remove(movieToAdjust2);

                            var newBeginningOfList2 = Copy(beginingOfList);

                            newBeginningOfList2.Add(movieToAdjust);

                            result.AddRange(GenerateMovieLists(newBeginningOfList2, newEndOfList2));
                        }

                        increment += earningsAdjustment;
                    }
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
                    list += movie.Name + ",";
                }

	            _logger?.WriteLine(list);
				_logger?.WriteLine(_elapsed.FormatRemaining());
            }
        }

		private List<IMovie> Concatenate(List<IMovie> list1, List<IMovie> list2)
        {
            var result = new List<IMovie>();

            result.AddRange(list1);
            result.AddRange(list2);

            return result;
        }
    }
}