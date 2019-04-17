using System;
using System.Collections.Generic;
using System.Linq;

using MoviePicker.Common.Interfaces;

namespace MooveePicker
{
	/// <summary>
	/// Attempting to solve the problem using Unbounded Knapsack which is a variation on the 0-1 Knapsack but you have unlimited copies of an item.
	/// Use some of the dominance relations:
	///		Modular - Using the greatest density of value (most value for the cost - highest value/cost)
	///		
	/// REF: https://en.wikipedia.org/wiki/Knapsack_problem
	/// 
	/// NOTE: This does NOT solve the problem with the order of the movies (since order does NOT matter) there might be duplicates and this needs to be taken care of.
	///		
	/// </summary>
	public class MoviePickerUnboundedKnapsack : IMoviePicker
	{
		private const int MAX_MOVIES = 15;

		private readonly List<IMovie> _movies;              // The list of baseline movies.
		private readonly IMovieList _movieListPrototype;

		private IMovie _bestPerformer;
		private List<IMovie> _bestPerformers;
		private int _costIdx;
		private int[] _currentCineplex;
		private bool _enableBestPerformer;
		private int _earningsIdx;
		private int _maxMovies;
		private IMovie[] _movieArray;

		private int[,] _topCineplexes;
		private int _topCount;
		private List<IMovieList> _topLists;
		private decimal _topMin;                        // The minimum of the list is only calculated when items are added/updated (not ALL the time)

		public MoviePickerUnboundedKnapsack(IMovieList movieListPrototype)
		{
			_enableBestPerformer = true;
			_movieArray = new IMovie[MAX_MOVIES];
			_movies = new List<IMovie>();

			_movieListPrototype = movieListPrototype;

			ScreenCount = 8;
			TotalCost = 1000;
		}

		/// <summary>
		/// Best Performer will return a value ONLY if it is the BEST performer (and there are no ties)
		/// </summary>
		public IMovie BestPerformer => _bestPerformer != null && _bestPerformers == null ? _bestPerformer : null;

		/// <summary>
		/// A list of TIED Best Performers.
		/// </summary>
		public IEnumerable<IMovie> BestPerformers => _bestPerformers;

		public bool EnableBestPerformer
		{
			get
			{
				return _enableBestPerformer;
			}
			set
			{
				_enableBestPerformer = value;

				if (_enableBestPerformer)
				{
					// Re-add the movies.
				}
				else
				{
					_bestPerformer = null;
					_bestPerformers = null;

					foreach (var movie in _movies)
					{
						movie.IsBestPerformer = false;
					}
				}
			}
		}

		public IEnumerable<IMovie> Movies => _movies;

		public int ScreenCount { get; set; }

		public int TotalCost { get; set; }

		public int TotalComparisons { get; set; }

		public int TotalSubProblems { get; set; }

		public void AddMovies(IEnumerable<IMovie> movies)
		{
			_movies.Clear();

			foreach (var movie in movies)
			{
				_movies.Add(movie);
			}

			if (EnableBestPerformer)
			{
				_bestPerformer = null;
				_bestPerformers = null;

				foreach (var movie in _movies.OrderByDescending(item => item.Efficiency))
				{
					if (_bestPerformer == null)
					{
						// Assign the first one as the best performer.

						_bestPerformer = movie;
						movie.IsBestPerformer = true;
					}
					else if (_bestPerformer.Efficiency == movie.Efficiency)
					{
						// Check to see if there are MANY tied Best Performers

						if (_bestPerformers == null)
						{
							_bestPerformers = new List<IMovie> { _bestPerformer };
						}

						movie.IsBestPerformer = true;
						_bestPerformers.Add(movie);
					}
					else
					{
						break;
					}
				}
			}

			if (_bestPerformers != null)
			{
				_bestPerformer = null;      // There is NO one best performer.
			}
		}

		public IMovieList ChooseBest2()
		{
			IMovieList best = _movieListPrototype.Clone();
			var idx = 0;
			var maxMovies = _movies.Count;

			TotalComparisons = 0;

			foreach (var movie in _movies)
			{
				_movieArray[idx++] = movie;
			}

			// Create all 15 ^ 8 possibilities. (Kinda brute force, but it's worth a try.)
			// Recursive calls COULD be slow, but investigate that later.
			// Trying to also minimize the memory footprint.
			// Figure out how to use Dominance Relations to clip out some of the noise.

			_topCineplexes = new int[_topCount == 0 ? 1 : _topCount, ScreenCount + 1];      // Store the top cineplexes (add 1 for the total BO)
			_currentCineplex = new int[ScreenCount + 1];

			for (int cineIdx = 0; cineIdx < _currentCineplex.Length; cineIdx++)
			{
				_currentCineplex[cineIdx] = -1;
			}

			for (int dim1 = 0; dim1 < maxMovies; dim1++)
			{
				_currentCineplex[0] = dim1;
				_currentCineplex[8] = (int)(_movieArray[dim1]?.Earnings ?? 0m);

				// Only adding one movie shouldn't break the bank,
				// so you shouldn't have to check to see if it is over the TotalCost

				for (int dim2 = 0; dim2 < maxMovies; dim2++)
				{
					var totalCost2 = _movieArray[dim1]?.Cost + _movieArray[dim2]?.Cost;

					_currentCineplex[1] = dim2;
					_currentCineplex[8] += (int)(_movieArray[dim2]?.Earnings ?? 0m);

					if (totalCost2 < TotalCost)
					{
						for (int dim3 = 0; dim3 < maxMovies; dim3++)
						{
							var totalCost3 = totalCost2 + _movieArray[dim3]?.Cost;

							if (totalCost3 < TotalCost)
							{
								// Keep adding dimensions... 8 screens

								if (_movieArray[dim1]?.Earnings + _movieArray[dim2]?.Earnings + _movieArray[dim3]?.Earnings > _topMin)
								{

								}
							}
							else
							{
								// Can't add any more.
							}
						}
					}
					else if (_currentCineplex[8] > _topMin)
					{
						ReplaceTopMinWithCurrent();
					}

					TotalComparisons++;
				}
			}

			// Add the top cineplex to the result.

			for (int cineIdx = 0; cineIdx < _currentCineplex.Length - 1; cineIdx++)
			{
				var movie = _topCineplexes[0, cineIdx] >= 0 ? _movieArray[_topCineplexes[0, cineIdx]] : null;

				if (movie != null)
				{
					best.Add(movie);
				}
			}

			return best;
		}

		public List<IMovieList> ChooseBest2(int topCount)
		{
			_topLists = new List<IMovieList>();
			_topCount = topCount;

			ChooseBest();

			return _topLists.OrderByDescending(list => list.TotalEarnings).ToList();
		}

		public IMovieList ChooseBest()
		{
			var list = ChooseBest(1);

			return list.First();
		}

		public List<IMovieList> ChooseBest(int topCount)
		{
			var idx = 0;

			_costIdx = ScreenCount + 1;
			_earningsIdx = ScreenCount;
			_maxMovies = _movies.Count;
			_topCount = topCount;

			_topCineplexes = new int[_topCount == 0 ? 1 : _topCount, _costIdx + 1];      // Store the top cineplexes (8 screen IDs)
			_currentCineplex = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, 0, 0 };    // 8 screen idx, earnings, cost.

			// Initialize top cineplexes (some sort of memcopy would be useful)

			for (int topIdx = 0; topIdx < _topCount; topIdx++)
			{
				for (int currIdx = 0; currIdx < _currentCineplex.Length; currIdx++)
				{
					_topCineplexes[topIdx, currIdx] = _currentCineplex[currIdx];
				}
			}

			_topLists = new List<IMovieList>();
			TotalComparisons = 0;

			// Sort the movies in order of cost from least to greatest
			// TODO: Add efficiency as a second sort parameter to use the better of the two if costs match.

			foreach (var movie in _movies.OrderBy(item => item.Cost))
			{
				_movieArray[idx++] = movie;
			}

			ChoostBestRecurse(0);

			// Add the top cineplexes to the result.

			for (int topIdx = 0; topIdx < _topCount; topIdx++)
			{
				IMovieList best = _movieListPrototype.Clone();

				for (int cineIdx = 0; cineIdx < _currentCineplex.Length - 2; cineIdx++)
				{
					var movie = _topCineplexes[topIdx, cineIdx] >= 0 ? _movieArray[_topCineplexes[topIdx, cineIdx]] : null;

					if (movie != null)
					{
						best.Add(movie);
					}
				}

				_topLists.Add(best);
			}

			return _topLists;
		}

		public void Clear()
		{
			_enableBestPerformer = true;
			_bestPerformers = null;
			_movies.Clear();
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		/// <summary>
		/// Recursive call for subsets of screens.
		/// </summary>
		/// <param name="screen"></param>
		private void ChoostBestRecurse(int screen)
		{
			var tooCostly = false;

			// Check to see if the current setup fits in the top list.

			if (_currentCineplex[_costIdx] <= TotalCost && _currentCineplex[_earningsIdx] > _topMin)
			{
				ReplaceTopMinWithCurrent();
			}

			// Loop through all of the movies for this screen.
			// Since the movies are sorted by cost, then once you can't fit one in, you can't fit the rest of them in.

			for (int movieIdx = 0; movieIdx < _maxMovies && !tooCostly; movieIdx++)
			{
				var movieCost = (int)(_movieArray[movieIdx]?.Cost ?? 0m);
				var movieEarnings = (int)(_movieArray[movieIdx]?.Earnings ?? 0m);

				_currentCineplex[screen] = movieIdx;
				_currentCineplex[_costIdx] += movieCost;
				_currentCineplex[_earningsIdx] += movieEarnings;

				if (screen == 0)
				{
					// Keep checking subsets

					ChoostBestRecurse(screen + 1);

					TotalSubProblems++;
				}
				else if (screen < ScreenCount - 1)
				{
					if (_currentCineplex[_costIdx] <= TotalCost)
					{
						// Keep checking subsets

						ChoostBestRecurse(screen + 1);

						TotalSubProblems++;
					}
					else
					{
						tooCostly = true;
					}
				}
				// Check against top list
				else if (_currentCineplex[_earningsIdx] > _topMin)
				{
					ReplaceTopMinWithCurrent();
				}

				_currentCineplex[_costIdx] -= movieCost;
				_currentCineplex[_earningsIdx] -= movieEarnings;

				TotalComparisons++;
			}

			_currentCineplex[screen] = -1;
		}

		private decimal ReplaceTopMinWithCurrent()
		{
			var toReplaceIdx = -1;
			var firstZeroIdx = -1;

			// Find the smallest value in top cineplexes.  (match the min value or the first zero)

			for (int idx = 0; idx < _topCount; idx++)
			{
				if (_topCineplexes[idx, _earningsIdx] == _topMin)
				{
					toReplaceIdx = idx;
				}
				if (_topCineplexes[idx, _earningsIdx] == 0)
				{
					firstZeroIdx = idx;
					break;
				}
			}

			if (firstZeroIdx >= 0)
			{
				toReplaceIdx = firstZeroIdx;
			}

			// Then replace that cineplex

			if (toReplaceIdx >= 0)
			{
				for (int cineIdx = 0; cineIdx < _currentCineplex.Length; cineIdx++)
				{
					_topCineplexes[toReplaceIdx, cineIdx] = _currentCineplex[cineIdx];
				}
			}

			_topMin = _currentCineplex[8];

			return 0m;
		}
	}
}