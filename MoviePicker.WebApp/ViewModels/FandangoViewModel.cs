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
	public class FandangoViewModel : IFandangoViewModel
	{
		private const int AVERAGE_TICKET_PRICE = 10;

		private readonly IMiner _fmlMiner = null;
		private readonly IMoviePicker _moviePicker = null;

		private List<IMovie> _movies = null;

		public FandangoViewModel()
		{
			Miner = new MineFandangoTicketSales();

			PastHours = 24;
		}

		public FandangoViewModel(IMiner fmlMiner, IMoviePicker moviePicker)
			: this()
		{
			_fmlMiner = fmlMiner;
			_moviePicker = moviePicker;
		}

		public long Duration { get; set; }

		public DateTime FilteredTo => LastUpdated.AddHours(-PastHours);

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


		public string PieGraphData
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

					builder.Append($"['{movie.Name}', {movie.Earnings / AVERAGE_TICKET_PRICE}]");

					isFirst = false;
				}

				return builder.ToString();
			}
		}

		public DateTime LastUpdated { get; private set; }

		public void Load()
		{
			((ICache)Miner).Load();

			LastUpdated = Miner.Movies.Max(movie => movie.WeekendEnding);

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

			var result = Miner.Movies.Where(movie => movie.WeekendEnding > FilteredTo)
							.GroupBy(movie => movie.Name)
							.Select(group => new Movie { Name = group.Key, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

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