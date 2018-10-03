using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.ViewModels
{
	public class CalculateViewModel
	{
		public CalculateViewModel(PicksViewModel picks)
		{
			MovieList = picks.MovieList;
			MovieListBonusOff = picks.MovieListBonusOff;
			Movies = picks.Movies;
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