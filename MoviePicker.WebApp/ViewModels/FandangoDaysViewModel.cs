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
	public class FandangoDaysViewModel : IFandangoViewModel
	{
		private const int AVERAGE_TICKET_PRICE = 10;

		private readonly IMiner _customMiner = null;			// My/Custom estimates.
		private readonly IMiner _fmlMiner = null;
		private readonly IMiner _mojoMiner = null;
		private readonly IMoviePicker _moviePicker = null;

		private List<IMovie> _movies = null;

		public FandangoDaysViewModel()
		{
			Miner = new MineFandangoTicketSalesByDay();

			PastHours = 24;
		}

		public FandangoDaysViewModel(IMinerModel minerModel, IMoviePicker moviePicker)
			: this()
		{
			_customMiner = minerModel.Miners[MinerModel.MY_INDEX];
			_fmlMiner = minerModel.Miners[MinerModel.FML_INDEX];
			_mojoMiner = minerModel.Miners[MinerModel.MOJO_LAST_INDEX];

			_moviePicker = moviePicker;
		}

		public long Duration { get; set; }

		public bool IsScaled { get; private set; }

		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		public bool IsTracking => _fmlMiner != null && _fmlMiner.ContainsEstimates;

		public IMiner Miner { get; private set; }

		public IMovieListModel MovieList { get; private set; }

		public IMovieListModel MovieListBonusOff { get; private set; }

		public IMovieListModel MovieListPerfectPick { get; set; }

		public IEnumerable<IMovie> Movies => _movies;

		public IEnumerable<IMovie> MoviesByDay => _movies;

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

		public int PastHours { get; set; }

		public DateTime LastUpdated { get; private set; }

		public void Load()
		{
			((ICache)Miner).Load();

			LastUpdated = Miner.LastUpdated.Value;

			_movies = FilterMovies();

			var movieList = _movies.OrderByDescending(movie => movie.EarningsBase).ToList();
			var totalBoxOffice = movieList.Sum(movie => movie.EarningsBase);

			// Scale the estimates (if they exist) to match the percentages of the sales.
			// o Use the sales scale outright and multiply the totalEstimate to get BO
			// o Combine the scales (somehow) and multiply the totalEstimate to get BO

			if (totalBoxOffice > 0)
			{
				var myMovieList = _customMiner.Movies;
				var totalEstimates = myMovieList?.Sum(item => item.EarningsBase) ?? 0;

				if (totalEstimates > 0)
				{
					IsScaled = true;

					foreach (var movie in movieList)
					{
						var myMovie = myMovieList.FirstOrDefault(item => item.Equals(movie));

						if (myMovie != null)
						{
							// For now this is option 1.
							movie.Earnings = totalEstimates * movie.EarningsBase / totalBoxOffice;
						}
					}
				}
			}

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
			var fmlMovie = _fmlMiner.Movies.FirstOrDefault();
			var endDate = fmlMovie?.WeekendEnding;						// Could be a Monday.
			var startDate = MovieDateUtil.GameSunday().AddDays(-2);     // Starts Friday
			var compoundMovie = _fmlMiner.CompoundMovie;

			if (compoundMovie != null)
			{
				// A compound movie exists (typically FRI, SAT, SUN)

				foreach (var movie in Miner.Movies)
				{
					if (movie.Equals(compoundMovie))
					{
						movie.Day = movie.WeekendEnding.DayOfWeek;
					}
				}
			}

			// Filter the list (Friday <- Sunday or Monday)
			// Group the movies by name.

			var result = Miner.Movies.Where(movie => startDate <= movie.WeekendEnding && movie.WeekendEnding <= endDate)
							.GroupBy(movie => movie.Name)		// Will split out the day too for compound movie.
							.Select(group => new Movie { MovieName = group.FirstOrDefault()?.MovieName, Day = group.FirstOrDefault()?.Day, Earnings = group.Sum(item => item.Earnings) })
							.Cast<IMovie>()
							.OrderByDescending(movie => movie.Earnings)
							.ToList();

			var gameMovies = _fmlMiner?.Movies;

			result = result.Where(movie => gameMovies == null || gameMovies.Contains(movie)).ToList();

			CompressMovies(result);

			// Assign the cost, image, and date so the view has this.

			foreach (var movie in result)
			{
				var found = gameMovies.FirstOrDefault(item => item.Equals(movie));
				var lastWeek = _mojoMiner?.Movies?.FirstOrDefault(item => item.Equals(movie));

				if (found != null)
				{
					movie.Cost = found.Cost;
					movie.ImageUrl = found.ImageUrl;
					movie.WeekendEnding = endDate ?? DateTime.Now;
					movie.IsNew = lastWeek == null;

					if (movie.IsNew)
					{
						if (compoundMovie == null)
						{
							movie.Earnings = Miner.Movies.Where(movie2 => startDate.AddDays(-1) <= movie2.WeekendEnding
																		&& movie2.WeekendEnding <= endDate
																		&& movie2.Equals(movie))
														.Sum(movie3 => movie3.EarningsBase);
						}
					}
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