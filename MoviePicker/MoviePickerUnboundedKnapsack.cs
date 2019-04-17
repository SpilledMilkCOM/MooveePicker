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
	/// </summary>
	public class MoviePickerUnboundedKnapsack : IMoviePicker
	{
		private const int MAX_MOVIES = 15;

		private readonly List<IMovie> _movies;              // The list of baseline movies.
		private readonly IMovieList _movieListPrototype;

		// Store the already processed sub-problems
		private readonly HashSet<int> _processedHashes;

		private IMovie _bestPerformer;
		private List<IMovie> _bestPerformers;
		private bool _enableBestPerformer;
		private IMovie[] _movieArray;
		private int _topCount;
		private List<IMovieList> _topLists;
		private decimal _topMin;                        // The minimum of the list is only calculated when items are added/updated (not ALL the time)

		public MoviePickerUnboundedKnapsack(IMovieList movieListPrototype)
		{
			_enableBestPerformer = true;
			_movieArray = new IMovie[MAX_MOVIES];
			_movies = new List<IMovie>();
			_processedHashes = new HashSet<int>();

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

		public int TotalSubProblems => _processedHashes.Count;

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

		public IMovieList ChooseBest()
		{
			IMovieList best = _movieListPrototype.Clone();
			var idx = 0;
			var maxMovies = _movies.Count;

			TotalComparisons = 0;
			_processedHashes.Clear();

			foreach (var movie in _movies)
			{
				_movieArray[idx++] = movie;
			}

			// Create all 15 ^ 8 possibilities. (Kinda brute force, but it's worth a try.)
			// Recursive calls COULD be slow, but investigate that later.
			// Trying to also minimize the memory footprint.
			// Figure out how to use Dominance Relations to clip out some of the noise.

			var topCineplexes = new int[_topCount == 0 ? 1 : _topCount, 9];      // Store the top cineplexes (8 screen IDs)
			var currentCineplex = new int[9];

			for (int cineIdx = 0; cineIdx < currentCineplex.Length; cineIdx++)
			{
				currentCineplex[cineIdx] = -1;
			}

			for (int dim1 = 0; dim1 < maxMovies; dim1++)
			{
				currentCineplex[0] = dim1;
				currentCineplex[8] = (int)(_movieArray[dim1]?.Earnings ?? 0m);

				// Only adding one movie shouldn't break the bank,
				// so you shouldn't have to check to see if it is over the TotalCost

				for (int dim2 = 0; dim2 < maxMovies; dim2++)
				{
					var totalCost2 = _movieArray[dim1]?.Cost + _movieArray[dim2]?.Cost;

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
					else if (currentCineplex[8] > _topMin)
					{
						for (int cineIdx = 0; cineIdx < currentCineplex.Length; cineIdx++)
						{
							topCineplexes[0, cineIdx] = currentCineplex[cineIdx];
						}

						_topMin = currentCineplex[8];
					}
				}
			}

			// Add the top cineplex to the result.

			for (int cineIdx = 0; cineIdx < currentCineplex.Length - 1; cineIdx++)
			{
				var movie = topCineplexes[0, cineIdx] >= 0 ? _movieArray[topCineplexes[0, cineIdx]] : null;

				if (movie != null)
				{
					best.Add(movie);
				}
			}

			return best;
		}

		public List<IMovieList> ChooseBest(int topCount)
		{
			_topLists = new List<IMovieList>();
			_topCount = topCount;

			ChooseBest();

			return _topLists.OrderByDescending(list => list.TotalEarnings).ToList();
		}

		public void Clear()
		{
			_enableBestPerformer = true;
			_bestPerformers = null;
			_movies.Clear();
			_processedHashes.Clear();
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		private decimal ReplaceTopMin(int[,] topCineplexes, int[] replaceWithCineplex)
		{
			// Find the smallest value in top cineplexes.

			// Then replace that cineplex

			// Find the newest smallest (which could be the one coming in)

			return 0m;
		}
	}
}