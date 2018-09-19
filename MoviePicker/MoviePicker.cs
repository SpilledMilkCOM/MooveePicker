using System;
using System.Collections.Generic;
using System.Linq;

using MoviePicker.Common.Interfaces;

namespace MooveePicker
{
	public class MoviePicker : IMoviePicker
	{
		private readonly List<IMovie> _movies;              // The list of baseline movies.
		private readonly IMovieList _movieListPrototype;

		// Store the already processed sub-problems
		private readonly HashSet<int> _processedHashes;

		private IMovie _bestPerformer;
		private List<IMovie> _bestPerformers;
		private int _topCount;
		private bool _topListFull;
		private List<IMovieList> _topLists;
		private decimal _topMin;                        // The minimum of the list is only calculated when items are added/updated (not ALL the time)
		private bool _enableBestPerformer;

		public MoviePicker(IMovieList movieListPrototype)
		{
			_enableBestPerformer = true;
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

			var availableMovies = AvailableMovies(_movies, TotalCost).ToList();

			TotalComparisons = 0;
			_processedHashes.Clear();

			foreach (var movie in availableMovies)
			{
				best = ChooseBest(best, movie, null, availableMovies, TotalCost);
			}

			return best;
		}

		public List<IMovieList> ChooseBest(int topCount)
		{
			_topLists = new List<IMovieList>();
			_topCount = topCount;
			_topListFull = false;

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

		private void AddToTopList(IMovieList sample)
		{
			if (_topLists != null)
			{
				if (!_topListFull)
				{
					_topLists.Add(sample.Clone());

					_topMin = _topLists.Min(list => list.TotalEarnings);
					_topListFull = _topLists.Count >= _topCount;
				}
				else
				{
					// Is the sample greater than the top min?
					// Don't add it if it's already in the list.

					if (sample.TotalEarnings > _topMin
					&& !_topLists.Any(list => list.GetHashCode() == sample.GetHashCode()))
					{
						// Remove the min and add a clone of the sample.

						_topLists.Remove(_topLists.First(list => list.TotalEarnings == _topMin));
						_topLists.Add(sample.Clone());

						_topMin = _topLists.Min(list => list.TotalEarnings);
					}
				}
			}
		}

		/// <summary>
		/// Return a list of movies each within the budget (sorted by efficiency)
		/// </summary>
		/// <param name="movies">A list of movies to scan</param>
		/// <param name="budget">The budget for a single pick</param>
		/// <returns>List of movies each within the budget (sorted by efficiency)</returns>
		private IEnumerable<IMovie> AvailableMovies(IEnumerable<IMovie> movies, decimal budget)
		{
			// Sorting by efficiency will help a "greedy" method.

			// Not really doing anything with efficiency just yet, so don't sort it.
			//return movies.Where(movie => movie.Cost <= budget).OrderByDescending(movie => movie.Efficiency);
			return movies.Where(movie => movie.Cost <= budget);
		}

		/// <summary>
		/// The recursive call to find the best of the sample.
		/// </summary>
		/// <param name="best">The BEST earnings so far.</param>
		/// <param name="movieToAdd">The movie to be added to the list.</param>
		/// <param name="sample">Current movie list</param>
		/// <param name="movies">Sending in a smaller set each time reduces the scan on the full movie list.</param>
		/// <param name="remainingBudget"></param>
		/// <returns></returns>
		private IMovieList ChooseBest(IMovieList best, IMovie movieToAdd, IMovieList sample, IEnumerable<IMovie> movies, decimal remainingBudget)
		{
			if (sample == null)
			{
				sample = _movieListPrototype.Clone();
			}

			if (sample.CanAdd(movieToAdd))
			{
				// Add the movie to the clone that was passed in.
				sample.Add(movieToAdd);

				TotalComparisons++;

				var sampleHashCode = sample.GetHashCode();

				var beenProcessed = _processedHashes.Contains(sampleHashCode);

				// DON'T PROCESS the sample:
				//		If the sample is already full
				//		Already been processed.

				if (_topLists != null && !beenProcessed)
				{
					AddToTopList(sample);
				}
				else if (best.TotalEarnings < sample.TotalEarnings)
				{
					best = sample.Clone();
				}

				if (!sample.IsFull && !beenProcessed)
				{
					remainingBudget -= movieToAdd.Cost;

					var availableMovies = AvailableMovies(movies, remainingBudget).ToList();

					foreach (var movie in availableMovies)
					{
						// Cloning is now cheaper than adding and removing.
						best = ChooseBest(best, movie, sample.Clone(), availableMovies, remainingBudget);

						//best = ChooseBest(best, movie, sample, availableMovies, remainingBudget);
					}

					// When finished processing this sub-list, store the list in the saved hashes.

					_processedHashes.Add(sampleHashCode);
				}

				//sample.Remove(movieToAdd);
			}

			return best;
		}
	}
}