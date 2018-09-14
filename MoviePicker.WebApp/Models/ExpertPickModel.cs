using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class ExpertPickModel
	{
		public IMiner Miner { get; set; }

		public IMovieListModel MovieList { get; set; }

		public IMovieListModel MovieListBonusOff { get; set; }
	}
}