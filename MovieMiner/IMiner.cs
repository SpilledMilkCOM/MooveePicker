﻿using System.Collections.Generic;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	// TODO: Might want to change the interface to send in a list of movies to look for to help out the scan.

	public interface IMiner
	{
		/// <summary>
		/// Abbreviation of the Miner name
		/// </summary>
		string Abbreviation { get; }

		bool IsHidden { get; set; }

		List<IMovie> Movies { get; }

		string Error { get; set; }

		/// <summary>
		/// Name of the Miner
		/// </summary>
		string Name { get; }

		/// <summary>
		/// By default this is set to true.
		/// </summary>
		bool OkToMine { get; set; }

		/// <summary>
		/// Typically the root Url for the root page.
		/// </summary>
		string Url { get; }

		/// <summary>
		/// Typically the root Url for the initial page.
		/// </summary>
		string UrlSource { get; }

		/// <summary>
		/// Used for weighted average.
		/// </summary>
		decimal Weight { get; set; }

		void Clear();

		List<IMovie> Mine();

		//Task<List<IMovie>> MineAsync();			// Not just yet.
	}
}