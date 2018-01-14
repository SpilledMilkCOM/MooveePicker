﻿using MovieMiner;
using MoviePicker.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.Models
{
	public class PicksViewModel
	{
		public long Duration { get; set; }

		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		public bool IsTracking { get; set; }

		public int ListCount { get; set; }

		public IEnumerable<IMiner> Miners { get; set; }

		public IMovieList MovieList { get; set; }

		public IMovieList MovieListBonusOff { get; set; }

		public IMovieList MovieListPerfectPick { get; set; }

		public IEnumerable<IMovie> Movies { get; set; }

		public string SharedPicksUrl { get; set; }

		/// <summary>
		/// TODO: Consolidate
		/// </summary>
		/// <param name="miner"></param>
		/// <returns></returns>
		public string GetFMLNerdLink()
		{
			// TODO: Consolidate

			string url = "http://analyzer.fmlnerd.com/lineups/?ests=";
			string movieList = null;

			foreach (var movie in Movies)
			{
				if (movieList != null)
				{
					movieList += ",";
				}

				movieList += movie == null ? "0" : ((int)movie.EarningsBase).ToString("D");
			}

			return url + movieList;
		}

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

				movieList += minerMovie == null ? "0" : minerMovie.EarningsBase.ToString();
			}

			return url + movieList;
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