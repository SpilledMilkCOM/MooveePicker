using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooveePicker
{
	public class MovieList : IMovieList
	{
		private const decimal COST_MAX = 1000;
		private const int MOVIE_MAX = 8;

		private readonly List<IMovie> _movies;
		private decimal _totalCost;
		private decimal _totalEarnings;

		public MovieList()
		{
			_movies = new List<IMovie>();
		}

		public MovieList(MovieList toCopy)
			: this()
		{
			if (!ReferenceEquals(this, toCopy))
			{
				_movies = new List<IMovie>(toCopy.Movies);

				UpdateTotals();
			}
		}

		public IEnumerable<IMovie> Movies => _movies;

		public decimal TotalCost => _totalCost;

		public decimal TotalEarnings => _totalEarnings;

		public void Add(IMovie movie)
		{
			if (movie == null)
			{
				throw new ArgumentNullException(nameof(movie), "The parameter cannot be null.");
			}

			if (_movies.Count == MOVIE_MAX)
			{
				throw new ArgumentOutOfRangeException(nameof(movie), $"The maximum movie limit of {MOVIE_MAX} has already been reached.");
			}

			_movies.Add(movie);

			UpdateTotals();
		}

		public void Add(IEnumerable<IMovie> movies)
		{
			if (movies == null)
			{
				throw new ArgumentNullException(nameof(movies), "The parameter cannot be null.");
			}

			if (_movies.Count + movies.Count() > MOVIE_MAX)
			{
				throw new ArgumentOutOfRangeException(nameof(movies), $"The maximum movie limit of {MOVIE_MAX} has already been reached.");
			}

			_movies.AddRange(movies);

			UpdateTotals();
		}

		public bool CanAdd(IMovie movie)
		{
			return TotalCost + movie.Cost <= COST_MAX && _movies.Count() < MOVIE_MAX;
		}

		public void Clear()
		{
			_movies.Clear();
		}

		public IMovieList Clone()
		{
			var result = new MovieList(this);

			return result;
		}

		public void Remove(IMovie movie)
		{
			_movies.Remove(movie);

			UpdateTotals();
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private void UpdateTotals()
		{
			// Keep the list sorted by cost so we don't need to always sort the list in the algoritm.

			_movies.Sort((left, right) =>
			{
				if (left.Cost == right.Cost)
				{
					return 0;
				}
				if (left.Cost < right.Cost)
				{
					return -1;
				}

				return 1;
			});

			_totalCost = _movies.Sum(item => item.Cost);
			_totalEarnings = _movies.Sum(item => item.Earnings) - (MOVIE_MAX - _movies.Count) * 2000000;

			if (_totalEarnings < 0)
			{
				_totalEarnings = 0;
			}
		}
	}
}