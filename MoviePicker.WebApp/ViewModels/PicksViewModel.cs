using MovieMiner;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoviePicker.WebApp.Models
{
	public class PicksViewModel
	{
		/// <summary>
		/// Post the Box Office values back to the controller.
		/// </summary>
		public decimal BoxOffice1 { get; set; }
		public decimal BoxOffice2 { get; set; }
		public decimal BoxOffice3 { get; set; }
		public decimal BoxOffice4 { get; set; }
		public decimal BoxOffice5 { get; set; }
		public decimal BoxOffice6 { get; set; }
		public decimal BoxOffice7 { get; set; }
		public decimal BoxOffice8 { get; set; }
		public decimal BoxOffice9 { get; set; }
		public decimal BoxOffice10 { get; set; }
		public decimal BoxOffice11 { get; set; }
		public decimal BoxOffice12 { get; set; }
		public decimal BoxOffice13 { get; set; }
		public decimal BoxOffice14 { get; set; }
		public decimal BoxOffice15 { get; set; }

		public long Duration { get; set; }

		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		public bool IsTracking { get; set; }

		public int ListCount { get; set; }

		public IEnumerable<IMiner> Miners { get; set; }

		public IMovieListModel MovieList { get; set; }

		public IMovieListModel MovieListBonusOff { get; set; }

		public IMovieListModel MovieListPerfectPick { get; set; }

		public IEnumerable<IMovie> Movies { get; set; }

		public string SharedPicksImageUrl { get; set; }

		public string SharedPicksUrl { get; set; }

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

		public string GenerateSharedImage(string webRootPath, List<string> files, string bonusFile, List<string> cellFilmFiles)
		{
			var imageUtil = new ImageUtility();

			return imageUtil.CombineImages(webRootPath, files, bonusFile, cellFilmFiles);
		}

		public int Rank(IMovie movie)
		{
			int rank = 1;

			foreach (var ranked in Movies.OrderByDescending(item => item.Efficiency))
			{
				if (movie == ranked)
				{
					break;
				}

				rank++;
			}

			return rank;
		}
	}
}