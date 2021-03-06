﻿using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.Models
{
	public class MovieListModel : IMovieListModel
	{
		public MovieListModel() { }

		public MovieListModel(string name, IMovieList movieList)
		{
			ComparisonHeader = name;

			Picks = new List<IMovieList> { movieList };
		}

		public string ComparisonHeader { get; set; }

		public IEnumerable<IMovie> ComparisonMovies { get; set; }

		public string Id { get; set; }

		public List<IMovieList> Picks { get; set; }

		public List<IMovieList> PicksTheRest { get; set; }

		public string ShareQueryString { get; set; }

		public decimal TotalPicksFromComparison
		{
			get
			{
				decimal result = 0;

				if (Picks != null && ComparisonMovies != null)
				{
					foreach (var pick in Picks[0].Movies)
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