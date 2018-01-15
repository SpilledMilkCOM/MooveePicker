using MoviePicker.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.Models
{
	public class MovieListModel : IMovieListModel
	{
		public string ComparisonHeader { get; set; }

		public IEnumerable<IMovie> ComparisonMovies { get; set; }

		public IMovieList Picks { get; set; }

		public decimal TotalPicksFromComparison
		{
			get
			{
				decimal result = 0;

				if (Picks != null && ComparisonMovies != null)
				{
					foreach (var pick in Picks.Movies)
					{
						var foundMovie = ComparisonMovies.FirstOrDefault(movie => movie.Name == pick.Name);

						if (foundMovie != null)
						{
							result += foundMovie.Earnings;
						}
					}
				}

				return result;
			}
		}
	}
}