using System.Collections.Generic;
using System.Linq;

namespace MooveePicker
{
	public class MoviePicker : IMoviePicker
	{
		// TODO: Compute Hash() for list to spot check

		private readonly List<IMovie> _movies;

		public MoviePicker()
		{
			_movies = new List<IMovie>();

			ScreenCount = 8;
			TotalCost = 1000;
		}

		public IEnumerable<IMovie> Movies => _movies;

		public int ScreenCount { get; set; }

		public int TotalCost { get; set; }

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
			// TODO: Use Unity.
			IMovieList best = new MovieList();

			foreach (var movie in AvailableMovies(TotalCost))
			{
				best = ChooseBest(best, movie, null, TotalCost);
			}

			return best;
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		private IEnumerable<IMovie> AvailableMovies(decimal budget)
		{
			return _movies.Where(movie => movie.Cost <= budget);
		}

		private IMovieList ChooseBest(IMovieList best, IMovie movieToAdd, IMovieList sample, decimal remainingBudget)
		{
			if (sample == null)
			{
				sample = new MovieList();
			}

			if (sample.CanAdd(movieToAdd))
			{
				sample.Add(movieToAdd);

				remainingBudget -= movieToAdd.Cost;

				if (best.TotalEarnings < sample.TotalEarnings)
				{
					best = sample.Clone();
				}

				foreach (var movie in AvailableMovies(remainingBudget))
				{
					best = ChooseBest(best, movie, sample, remainingBudget);
				}
			}

			return best;
		}
	}
}