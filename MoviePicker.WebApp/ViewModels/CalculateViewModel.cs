using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.ViewModels
{
	public class CalculateViewModel
	{
		public CalculateViewModel(PicksViewModel picks)
		{
			Duration = picks.Duration;
			MovieList = picks.MovieList;
			MovieListBonusOff = picks.MovieListBonusOff;
			Movies = picks.Movies.OrderByDescending(movie => movie.Efficiency);
			SharedPicksUrl = picks.SharedPicksUrl;
		}

		public long Duration { get; set; }

		public IMovieListModel MovieList { get; set; }

		public IMovieListModel MovieListBonusOff { get; set; }

		public IMovieListModel MovieListPerfectPick { get; set; }

		public IEnumerable<IMovie> Movies { get; set; }

		public string SharedPicksUrl { get; set; }
	}
}