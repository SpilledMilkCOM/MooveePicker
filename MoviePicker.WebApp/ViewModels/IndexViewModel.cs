﻿using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoviePicker.WebApp.ViewModels
{
	/// <summary>
	/// The ViewModel for the Index page.  Flattening out the data seems to help with the Post back to the controller.
	/// </summary>
	public class IndexViewModel : IIndexViewModel
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
		public decimal BoxOffice11{ get; set; }
		public decimal BoxOffice12 { get; set; }
		public decimal BoxOffice13 { get; set; }
		public decimal BoxOffice14 { get; set; }
		public decimal BoxOffice15 { get; set; }

		public long Duration { get; set; }

		public Guid Id { get; set; }

		public string ImageType { get; set; }


		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		public bool IsTracking { get; set; }

		/// <summary>
		/// Display the Miners on the page.
		/// </summary>
		public IEnumerable<IMiner> Miners { get; set; }

		public bool ViewGridOpen { get; set; }

		public bool ViewMobileOpen { get; set; }

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

		public string WeightListForMiner(IMiner minerToCheck)
		{
			var builder = new StringBuilder();
			bool first = true;

			builder.Append("wl=");

			foreach (var miner in Miners.Skip(1))
			{
				if (!first)
				{
					builder.Append(",");
				}

				first = false;

				builder.Append(minerToCheck == miner ? "1" : "0");
			}

			return builder.ToString();
		}
	}
}