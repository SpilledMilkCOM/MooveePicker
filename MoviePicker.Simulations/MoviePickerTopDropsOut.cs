using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MooveePicker
{
	/// <summary>
	/// Vary the earnings by some offsets and compute the BEST out of how many of that list WINS.
	/// </summary>
	public class MoviePickerTopDropsOut : IMoviePicker
	{
		private const decimal EARNINGS_ADJUSTMENT = 0.1m;

		private readonly IMovieList _movieListPrototype;
		private readonly IMoviePicker _moviePicker;
		private readonly List<IMovie> _baselineMovies;

		public MoviePickerTopDropsOut(IMovieList movieListPrototype)
		{
			_moviePicker = new MoviePicker(new MovieList());
			_baselineMovies = new List<IMovie>();

			_movieListPrototype = movieListPrototype;
		}

		public IEnumerable<IMovie> Movies => _moviePicker.Movies;

		public int TotalComparisons { get; set; }

		public int TotalSubProblems { get; set; }

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
			var earningsAdjustment = EARNINGS_ADJUSTMENT * 1000000;
			var maxValue = _baselineMovies.Max(movie => movie.Earnings);
			var tentPoleMovie = _baselineMovies.First(movie => movie.Earnings == maxValue);

			_moviePicker.AddMovies(_baselineMovies);

			var best = _moviePicker.ChooseBest();

			TotalComparisons += _moviePicker.TotalComparisons;
			TotalSubProblems += _moviePicker.TotalSubProblems;

			while (best.Movies.FirstOrDefault(movie => movie.Id == tentPoleMovie.Id) != null)
			{
				maxValue = tentPoleMovie.Earnings;

				// Keep changing the tentPoleMovie until it is no longer in the list.

				tentPoleMovie.Earnings -= earningsAdjustment;

				best = _moviePicker.ChooseBest();

				TotalComparisons += _moviePicker.TotalComparisons;
				TotalSubProblems += _moviePicker.TotalSubProblems;
			}

			tentPoleMovie.Earnings = maxValue;

			return _moviePicker.ChooseBest();
		}

		//----==== PRIVATE ====----------------------------------------------------------------------

		private List<IMovie> Copy(IEnumerable<IMovie> toCopy)
		{
			return toCopy.Select(movie => movie.Clone()).ToList();
		}
	}
}