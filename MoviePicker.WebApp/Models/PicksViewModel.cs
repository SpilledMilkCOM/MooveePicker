using MovieMiner;
using MoviePicker.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.Models
{
	public class PicksViewModel
	{
		public IEnumerable<IMiner> Miners { get; set; }

		public IMovieList MovieList { get; set; }

		public IMovieList MovieListBonusOff { get; set; }

		public IEnumerable<IMovie> Movies { get; set; }

		/// <summary>
		/// TODO: Consolidate
		/// </summary>
		/// <param name="miner"></param>
		/// <returns></returns>
		public string GetFMLNerdLink(IMiner miner)
		{
			// TODO: Consolidate

			string url = "http://analyzer.fmlnerd.com/lineups/?ests=";
			string movieList = null;
			var nerdList = Miners.First();

			foreach (var movie in nerdList.Movies)
			{
				var minerMovie = miner.Movies.FirstOrDefault(item => item.Name == movie.Name);

				if (movieList != null)
				{
					movieList += ",";
				}

				movieList += minerMovie == null ? "0" : minerMovie.Earnings.ToString();
			}

			return url + movieList;
		}
	}
}