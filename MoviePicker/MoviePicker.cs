using System.Collections.Generic;
using System.Linq;

namespace MooveePicker
{
	public class MoviePicker : IMoviePicker
	{
		private readonly List<IMovie> _movies;
		private readonly IMovieList _movieListPrototype;

		// Store the already processed sub-problems
		private readonly HashSet<int> _processedHashes;

		public MoviePicker(IMovieList movieListPrototype)
		{
			_movies = new List<IMovie>();
			_processedHashes = new HashSet<int>();

			_movieListPrototype = movieListPrototype;

			ScreenCount = 8;
			TotalCost = 1000;
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
		}

		public IMovieList ChooseBest()
		{
			IMovieList best = _movieListPrototype.Clone();

			var availableMovies = AvailableMovies(_movies, TotalCost).ToList();

			foreach (var movie in availableMovies)
			{
				best = ChooseBest(best, movie, null, availableMovies, TotalCost);
			}

			return best;
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

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
				sample.Add(movieToAdd);

				TotalComparisons++;

				if (best.TotalEarnings < sample.TotalEarnings)
				{
					best = sample.Clone();
				}

				var sampleHashCode = sample.GetHashCode();

				// DON'T PROCESS the sample:
				//		If the sample is already full
				//		Already been processed.

				if (!sample.IsFull && !_processedHashes.Contains(sampleHashCode))
				{
					var availableMovies = AvailableMovies(movies, remainingBudget).ToList();

					remainingBudget -= movieToAdd.Cost;

					foreach (var movie in availableMovies)
					{
						// Cloning is expensive.  Better to remove the added movie when done with this current method call.
						//best = ChooseBest(best, movie, sample.Clone(), availableMovies, remainingBudget);

						best = ChooseBest(best, movie, sample, availableMovies, remainingBudget);
					}

					// When finished processing this sub-list, store the list in the saved hashes.

					_processedHashes.Add(sampleHashCode);
				}

				sample.Remove(movieToAdd);
			}

			return best;
		}
	}
}