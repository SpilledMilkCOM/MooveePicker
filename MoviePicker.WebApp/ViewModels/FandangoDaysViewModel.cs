using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.ViewModels
{
	public class FandangoDaysViewModel : IFandangoViewModel
	{
		private readonly IMiner _fmlMiner = null;
		private readonly IMoviePicker _moviePicker = null;

		private List<IMovie> _movies = null;

		public FandangoDaysViewModel()
		{
			Miner = new MineFandangoTicketSalesByDay();

			PastHours = 24;
		}

		public FandangoDaysViewModel(IMiner fmlMiner, IMoviePicker moviePicker)
			: this()
		{
			_fmlMiner = fmlMiner;
			_moviePicker = moviePicker;
		}

		public long Duration { get; set; }

		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		public bool IsTracking => _fmlMiner != null && _fmlMiner.ContainsEstimates;

		public IMiner Miner { get; private set; }

		public IMovieListModel MovieList { get; private set; }

		public IMovieListModel MovieListBonusOff { get; private set; }

		public IMovieListModel MovieListPerfectPick { get; set; }

		public IEnumerable<IMovie> Movies => _movies;

		public int PastHours { get; set; }

		public DateTime LastUpdated { get; private set; }

		public void Load()
		{
			((ICache)Miner).Load();

			LastUpdated = Miner.LastUpdated.Value;

			_movies = FilterMovies();
			MovieList = MakePick(true);
			MovieListBonusOff = MakePick(false);
		}

		public int Rank(IMovie movie)
		{
			int rank = 1;
			int movieCount = 0;
			decimal lastEfficiency = 0;

			foreach (var ranked in Movies.OrderByDescending(item => item.Efficiency))
			{
				movieCount++;

				if (lastEfficiency != ranked.Efficiency)
				{
					rank = movieCount;
				}

				if (movie.Equals(ranked))
				{
					break;
				}

				lastEfficiency = ranked.Efficiency;
			}

			return rank;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private List<IMovie> CloneList(IEnumerable<IMovie> list)
		{
			var result = new List<IMovie>();

			foreach (var movie in list.Where(item => item.Earnings > 0))
			{
				result.Add(movie.Clone());
			}

			return result;
		}

		private void CompressMovies(List<IMovie> list)
		{
			if (list != null)
			{
				// Remove the year if the movie ends with the year.

				var year = DateTime.Now.Year.ToString();

				foreach (var movie in list)
				{
					if (movie.MovieName.EndsWith(year))
					{
						movie.MovieName = movie.MovieName.Substring(0, movie.MovieName.Length - 5);
					}
				}

				var copy = new List<IMovie>(list);
				var toRemove = new List<IMovie>();

				foreach (var movie in list)
				{
					// Find a simlarly named movie (or possibly the same name - don't match on the same ID).

					var likeMovie = copy.FirstOrDefault(item => item.Equals(movie) && item.Id != movie.Id);

					if (likeMovie != null && !toRemove.Contains(likeMovie))
					{
						// Add the totals.

						movie.Earnings += likeMovie.Earnings;

						// Remove the movie so it can't be found again.

						RemoveSameName(copy, likeMovie);
						toRemove.Add(likeMovie);
					}
				}

				foreach (var movie in toRemove)
				{
					var found = list.FirstOrDefault(item => item.MovieName == movie.MovieName);

					if (found != null)
					{
						RemoveSameName(list, found);
					}
				}
			}
		}

		private List<IMovie> FilterMovies()
		{
			var now = DateTime.Now;
			var endDate = MovieDateUtil.GameSunday();
			var startDate = endDate.AddDays(-2);

			endDate = endDate.AddDays(1);           // Include Monday.

			// Filter the list (Friday <- Sunday)
			// Group the movies by name.

			var result = Miner.Movies.Where(movie => startDate <= movie.WeekendEnding && movie.WeekendEnding <= endDate)
							.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			var gameMovies = _fmlMiner?.Movies;

			result = result.Where(movie => gameMovies == null || gameMovies.Contains(movie)).ToList();

			CompressMovies(result);

			// Assign the cost so the view has this.

			foreach (var movie in result)
			{
				var found = gameMovies.FirstOrDefault(item => item.Equals(movie));

				if (found != null)
				{
					movie.Cost = found.Cost;
					movie.ImageUrl = found.ImageUrl;
					movie.WeekendEnding = now;
				}
			}

			return result;
		}

		private IMovieListModel MakePick(bool enableBonus)
		{
			var clonedList = CloneList(Movies);

			_moviePicker.Clear();
			_moviePicker.AddMovies(clonedList);

			if (!enableBonus)
			{
				_moviePicker.EnableBestPerformer = enableBonus;
			}

			var picks = _moviePicker.ChooseBest();

			return new MovieListModel(enableBonus ? "Bonus ON" : "Bonus OFF", picks);
		}

		/// <summary>
		/// Remove a movie from a list matching on the MovieName (not using Equals)
		/// </summary>
		/// <param name="list"></param>
		/// <param name="toRemove"></param>
		private void RemoveSameName(List<IMovie> list, IMovie toRemove)
		{
			var indexToRemove = 0;

			foreach (var movie in list)
			{
				if (movie.MovieName == toRemove.MovieName)
				{
					break;
				}

				indexToRemove++;
			}

			if (indexToRemove < list.Count)
			{
				list.RemoveAt(indexToRemove);
			}
		}
	}
}