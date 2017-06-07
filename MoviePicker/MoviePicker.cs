using System.Collections.Generic;
using System.Linq;

namespace MooveePicker
{
	public class MoviePicker : IMoviePicker
	{
		// TODO: Compute Hash() for list to spot check

		private readonly List<IMovie> _movies;
		private readonly IMovieList _movieListPrototype;

		public MoviePicker(IMovieList movieListPrototype)
		{
			_movies = new List<IMovie>();

			_movieListPrototype = movieListPrototype;

			ScreenCount = 8;
			TotalCost = 1000;
		}

		public IEnumerable<IMovie> Movies => _movies;

		public int ScreenCount { get; set; }

		public int TotalCost { get; set; }

		public int TotalComparisons { get; set; }

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
			return movies.Where(movie => movie.Cost <= budget).OrderByDescending(movie => movie.Efficiency);
		}

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

				// If the sample is already full then don't try to add any more movies.

				if (!sample.IsFull)
				{
					var availableMovies = AvailableMovies(movies, remainingBudget).ToList();

					remainingBudget -= movieToAdd.Cost;

					foreach (var movie in availableMovies)
					{
						best = ChooseBest(best, movie, sample.Clone(), availableMovies, remainingBudget);
					}
				}
			}

			return best;
		}
	}
}