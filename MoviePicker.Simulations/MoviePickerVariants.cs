using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;

namespace MooveePicker
{
	/// <summary>
	/// Vary the earnings by some offsets and compute the BEST out of how many of that list WINS.
	/// The FULL version of this where it recursively tries ALL variants will require some SERIOUS compute power.
	/// </summary>
	public class MoviePickerVariants : IMoviePicker
	{
		private const decimal EARNINGS_ADJUSTMENT = 0.5m;
		private const decimal EARNINGS_VARIANT_MAX = 5.0m;

		private const decimal EARNINGS_ADJUSTMENT_PERCENT = 0.001m;
		private const decimal EARNINGS_VARIANT_PERCENT_MAX = 0.03m;

		private readonly List<IMovie> _baselineMovies;
		private readonly Dictionary<int, int> _bestListCounts;          // Keyed using the hash code.
		private readonly Dictionary<int, IMovieList> _bestLists;        // Keyed using the hash code.
		private readonly IMovieList _movieListPrototype;
		private readonly IMoviePicker _moviePicker;

		public MoviePickerVariants(IMovieList movieListPrototype)
		{
			_bestListCounts = new Dictionary<int, int>();
			_bestLists = new Dictionary<int, IMovieList>();
			_moviePicker = new MsfMovieSolver();
			//_moviePicker = new MoviePicker(new MovieList());
			_baselineMovies = new List<IMovie>();

			_movieListPrototype = movieListPrototype;
		}

		public IEnumerable<IMovie> Movies => _moviePicker.Movies;

		public int TotalComparisons { get; set; }

		public int TotalMovieLists { get; set; }

		public int TotalSubProblems { get; set; }

		public bool EarningsAdjustmentByPercent { get; set; }

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
			var movieLists = GenerateMovieLists();

			TotalMovieLists = movieLists.Count;

			// Walk through the movie list adjusting the movies by the variant and then choose the best.

			foreach (var movieList in movieLists)
			{
				_moviePicker.AddMovies(movieList);

				var best = _moviePicker.ChooseBest();
				var hashCode = best.GetHashCode();
				int value;

				TotalComparisons += _moviePicker.TotalComparisons;
				TotalSubProblems += _moviePicker.TotalSubProblems;

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

		public List<IMovieList> GetRankedMovieLists()
		{
			return _bestListCounts.OrderByDescending(pair => pair.Value)
								.Take(10)
								.Select(keyValuePair => _bestLists[keyValuePair.Key])
								.ToList();
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		private List<IMovie> Copy(IEnumerable<IMovie> toCopy)
		{
			return toCopy.Select(movie => movie.Clone()).ToList();
		}

		private List<List<IMovie>> GenerateMovieLists()
		{
			var result = new List<List<IMovie>>();
			decimal earningsAdjustment = (EarningsAdjustmentByPercent) ? EARNINGS_ADJUSTMENT_PERCENT : EARNINGS_ADJUSTMENT;
			decimal earningsAdjustmentMax = (EarningsAdjustmentByPercent) ? EARNINGS_VARIANT_PERCENT_MAX : EARNINGS_VARIANT_MAX;



			result.Add(_baselineMovies);        // Add the original list

			// Only tweak one movie in each list.

			foreach (var movie in _baselineMovies)
			{
				var increment = earningsAdjustment;

				while (increment <= earningsAdjustmentMax)
				{
					var list = Copy(_baselineMovies);
					var movieToAdjust = list.First(item => item.Id == movie.Id);

					if (EarningsAdjustmentByPercent)
					{
						movieToAdjust.Earnings += movieToAdjust.Earnings * increment;
					}
					else
					{
						movieToAdjust.Earnings += increment * 1000000m;
					}

					result.Add(list);

					var list2 = Copy(_baselineMovies);
					var movieToAdjust2 = list2.First(item => item.Id == movie.Id);

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
						result.Add(list2);
					}

					increment += earningsAdjustment;
				}
			}

			return result;
		}
	}
}