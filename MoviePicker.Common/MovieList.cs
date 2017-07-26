using System;
using System.Collections.Generic;
using System.Linq;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Common
{
    public class MovieList : IMovieList
    {
        private const decimal COST_MAX = 1000;
        private const int MOVIE_MAX = 8;
        private const int MISSING_THEATER_EARNINGS = 2000000;       // 2 million for each missing theater.
        private const int TOP_PERFORMER_BONUS = 2000000;            // 2 million for each top performer.

        private int _hashCode;
        private readonly List<IMovie> _movies;
        private decimal _totalCost;
        private int _totalCount;
        private decimal _totalEarnings;

        public MovieList()
        {
            _movies = new List<IMovie>();

            // If there are NO theaters, then you're running at a deficit.
            //_totalEarnings = -(MOVIE_MAX - _totalCount) * MISSING_THEATER_EARNINGS;
        }

        // You'd think that Unity would be smart enough to NOT use this copy constructor when resolving an object with no parameters.
        // I guess the rule of thumb is DON'T use copy constructors when using an IoC container (or make them private since by default Unity can only see public constructors)
        private MovieList(MovieList toCopy)
            : this()
        {
            if (!ReferenceEquals(this, toCopy))
            {
                _movies = new List<IMovie>(toCopy.Movies);

                UpdateTotals();
            }
        }

        public bool IsFull => _movies.Count >= MOVIE_MAX;

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
            // Should NOT have to recount because checking IsFull BEFORE another recursive call.
            //return TotalCost + movie.Cost <= COST_MAX && _movies.Count() < MOVIE_MAX;
            return TotalCost + movie.Cost <= COST_MAX;
        }

        public void Clear()
        {
            _movies.Clear();
        }

        public IMovieList Clone()
        {
            return new MovieList(this);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void Remove(IMovie movie)
        {
            _movies.Remove(movie);

            UpdateTotals();

            //_totalCount--;
            //_totalCost -= movie.Cost;
            //_totalEarnings -= movie.Earnings + MISSING_THEATER_EARNINGS;
        }

        //----==== PRIVATE ====---------------------------------------------------------

        private void UpdateTotals()
        {
            // "Greedy" method is not used (yet)
            // Keep the list sorted by efficiency so we don't need to always sort the list in the algoritm.

            //_movies.Sort((left, right) =>
            //{
            //	if (left.Efficiency == right.Efficiency)
            //	{
            //		return 0;
            //	}
            //	if (left.Efficiency < right.Efficiency)
            //	{
            //		return -1;
            //	}

            //	return 1;
            //});

            // Keep this list sorted by Id so the hashing won't care about the order added.

            _movies.Sort((left, right) =>
            {
                if (left.Id == right.Id)
                {
                    return 0;
                }
                if (left.Id < right.Id)
                {
                    return -1;
                }

                return 1;
            });

            _totalCost = 0;
            _totalEarnings = 0;
            _hashCode = 19;
            _totalCount = 0;

            // Only iterate the list once.

            foreach (var item in _movies)
            {
                // Order does NOT matter for this hash code.
                // The hash for the following lists will produce the same hash:
                // 1,2,3,3,3
                // 1,3,2,3,3
                // 1,3,3,2,3
                // 1,3,3,3,2

                _hashCode = _hashCode * 31 + item.GetHashCode();

                _totalCount++;
                _totalCost += item.Cost;
                _totalEarnings += item.Earnings;

                if (item.IsBestPerformer)
                {
                    _totalEarnings += TOP_PERFORMER_BONUS;
                }
            }

            _totalEarnings -= (MOVIE_MAX - _totalCount) * MISSING_THEATER_EARNINGS;

            if (_totalEarnings < 0)
            {
                _totalEarnings = 0;
            }
        }

        private void UpdateTotals(IMovie movie, bool added)
        {
            // Keep the list sorted by efficiency so we don't need to always sort the list in the algoritm.

            //_movies.Sort((left, right) =>
            //{
            //	if (left.Efficiency == right.Efficiency)
            //	{
            //		return 0;
            //	}
            //	if (left.Efficiency < right.Efficiency)
            //	{
            //		return -1;
            //	}

            //	return 1;
            //});

            _totalCost += (added) ? movie.Cost : -movie.Cost;
            _totalEarnings = _movies.Sum(item => item.Earnings) - (MOVIE_MAX - _movies.Count) * 2000000;

            if (_totalEarnings < 0)
            {
                _totalEarnings = 0;
            }
        }
    }
}