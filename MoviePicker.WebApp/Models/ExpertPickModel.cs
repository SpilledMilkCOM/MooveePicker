using MovieMiner;
using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.Models
{
	public class ExpertPickModel
	{
		public IMiner Miner { get; set; }

		public IMovieListModel MovieList { get; set; }

		public IMovieListModel MovieListBonusOff { get; set; }
	}
}