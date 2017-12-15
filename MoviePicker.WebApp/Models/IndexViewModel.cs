using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.Models
{
	/// <summary>
	/// The ViewModel for the Index page.  Flattening out the data seems to help with the Post back to the controller.
	/// </summary>
	public class IndexViewModel : IIndexViewModel
	{
		/// <summary>
		/// Display the Miners on the page.
		/// </summary>
		public IEnumerable<IMiner> Miners { get; set; }

		/// <summary>
		/// Post the Weight back to the controller.
		/// </summary>
		public decimal Weight1 { get; set; }

		public decimal Weight2 { get; set; }

		public decimal Weight3 { get; set; }

		public decimal Weight4 { get; set; }

		public decimal Weight5 { get; set; }

		public decimal Weight6 { get; set; }

		public decimal Weight7 { get; set; }

		public string GetFMLNerdLink(IMiner miner)
		{
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