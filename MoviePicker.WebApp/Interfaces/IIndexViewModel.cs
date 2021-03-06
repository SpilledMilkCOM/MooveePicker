﻿using MovieMiner;
using System;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IIndexViewModel
	{
		decimal BoxOffice1 { get; set; }
		decimal BoxOffice2 { get; set; }
		decimal BoxOffice3 { get; set; }
		decimal BoxOffice4 { get; set; }
		decimal BoxOffice5 { get; set; }
		decimal BoxOffice6 { get; set; }
		decimal BoxOffice7 { get; set; }
		decimal BoxOffice8 { get; set; }
		decimal BoxOffice9 { get; set; }
		decimal BoxOffice10 { get; set; }
		decimal BoxOffice11 { get; set; }
		decimal BoxOffice12 { get; set; }
		decimal BoxOffice13 { get; set; }
		decimal BoxOffice14 { get; set; }
		decimal BoxOffice15 { get; set; }

		long Duration { get; set; }

		Guid Id { get; set; }

		/// <summary>
		/// The generated image type for the Twitter (Open Graph) summary card.
		/// Image Type (it)
		/// </summary>
		string ImageType { get; set; }

		/// <summary>
		/// The estimated values are in (typically on a Saturday)
		/// </summary>
		bool IsTracking { get; set; }

		IEnumerable<IMiner> Miners { get; set; }

		bool ViewGridOpen { get; set; }

		bool ViewMobileOpen { get; set; }

		decimal Weight1 { get; set; }

		decimal Weight2 { get; set; }

		decimal Weight3 { get; set; }

		decimal Weight4 { get; set; }

		decimal Weight5 { get; set; }

		decimal Weight6 { get; set; }

		decimal Weight7 { get; set; }

		string GetFMLNerdLink(IMiner miner);
	}
}