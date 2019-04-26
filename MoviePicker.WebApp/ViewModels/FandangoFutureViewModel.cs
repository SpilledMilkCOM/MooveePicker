using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoviePicker.WebApp.ViewModels
{
	public class FandangoFutureViewModel
	{
		private readonly IMiner _fmlMiner = null;

		private List<IMovie> _movies = null;

		public FandangoFutureViewModel()
		{
			Miner = new MineFandangoTicketSales();
		}

		public FandangoFutureViewModel(IMinerModel minerModel, IMiner futureMiner)
			: this()
		{
			_fmlMiner = minerModel.Miners[MinerModel.FML_INDEX];
			Miner = futureMiner;
		}

		public long Duration { get; set; }

		public IMiner Miner { get; private set; }

		public IEnumerable<IMovie> Movies => _movies;

		public string ColumnGraphData
		{
			get
			{
				var builder = new StringBuilder();
				var isFirst = true;

				foreach (var movie in Movies.OrderByDescending(item => item.Earnings))
				{
					if (!isFirst)
					{
						builder.Append(", ");
					}

					builder.Append($"['{movie.Name}', {movie.Earnings}]");

					isFirst = false;
				}

				return builder.ToString();
			}
		}

		public DateTime LastUpdated { get; private set; }

		public void Load()
		{
			((ICache)Miner).Load();

			LastUpdated = Miner.LastUpdated ?? DateTime.Now;

			var movieList = new List<IMovie>();
			var viewModel = new HistoryViewModel { Movies = FilterMovies() };

			// When a miner is cloned.  Only the list is cloned, but you can still affect the movie inside the base list.
			// Which is why EACH movie needs to be cloned here, because its box office list can get updated with the estimates if they are part of the Query string.

			_fmlMiner.Movies.ForEach(item => movieList.Add(item.Clone()));

			// Remove any of the compound movies and turn it into a single movie since the data is for the single movie.

			var toRemove = viewModel.Movies.Where(item => item.Day.HasValue && item.Day != DayOfWeek.Friday).ToList();

			if (toRemove != null && toRemove.Any())
			{
				var movies = viewModel.Movies.ToList();

				toRemove.ForEach(item => movies.Remove(item));

				viewModel.Movies = movies;

				var fridayMovie = movies.FirstOrDefault(item => item.Day.HasValue);

				if (fridayMovie != null)
				{
					fridayMovie.Day = null;
				}
			}

			// Loop through the movies and attach the historical data.

			foreach (var movie in viewModel.Movies)
			{
				var futureMovieBoxOffice = Miner.Movies.Where(item => item.Equals(movie));

				if (futureMovieBoxOffice != null && futureMovieBoxOffice.Any())
				{
					var boList = new List<IBoxOffice>();

					// In this case the weekend ending is just a date (not necessarily a weekend).

					foreach (var boxOffice in futureMovieBoxOffice)
					{
						boList.Add(new BoxOffice { Earnings = boxOffice.EarningsBase, WeekendEnding = boxOffice.WeekendEnding });
					}

					movie.SetBoxOfficeHistory(boList);
				}
			}

			_movies = FilterMovies();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

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

				// Remove The IMAX Experience blah, blah, blah.

				foreach (var movie in list)
				{
					var index = movie.MovieName.IndexOf(" The IMAX ");

					if (index > 0)
					{
						movie.MovieName = movie.MovieName.Substring(0, index);
					}
				}

				var copy = new List<IMovie>(list);
				var toRemove = new List<IMovie>();

				foreach (var movie in list)
				{
					// Make sure the movie is not already being removed.

					if (toRemove.FirstOrDefault(item => item.Id == movie.Id) == null)
					{
						// Find simlarly named movie

						var likeMovies = copy.Where(item => item.Equals(movie) && item.Id != movie.Id).ToList();

						foreach (var likeMovie in likeMovies)
						{
							if (toRemove.FirstOrDefault(item => item.Id == likeMovie.Id) == null)
							{
								// Add the totals.

								movie.Earnings += likeMovie.Earnings;

								// Remove the movie so it can't be found again.

								RemoveSameId(copy, likeMovie);
								toRemove.Add(likeMovie);
							}
						}
					}
				}

				foreach (var movie in toRemove)
				{
					var found = list.FirstOrDefault(item => item.Id == movie.Id);

					if (found != null)
					{
						RemoveSameId(list, found);
					}
				}
			}
		}

		private List<IMovie> FilterMovies()
		{
			var now = DateTime.Now;

			// Filter the list (last 24 hours)
			// Group the movies by name.

			var result = Miner.Movies;

			var gameMovies = _fmlMiner?.Movies;

			result = result.Where(movie => gameMovies == null || gameMovies.Contains(movie)).ToList();

			var id = 1;

			result.ForEach(item => item.Id = id++);			// Choose some arbitrary IDs.

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

		/// <summary>
		/// Remove a movie from a list matching on the MovieName (not using Equals)
		/// </summary>
		/// <param name="list"></param>
		/// <param name="toRemove"></param>
		private void RemoveSameId(List<IMovie> list, IMovie toRemove)
		{
			var indexToRemove = 0;

			foreach (var movie in list)
			{
				if (movie.Id == toRemove.Id)
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