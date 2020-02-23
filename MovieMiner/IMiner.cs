using System;
using System.Collections.Generic;
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

		string BoxOfficeListCSV { get; }

		bool CompoundLoaded { get; }

		IMovie CompoundMovie { get; }

		decimal CompoundTotal { get; }

		bool ContainsEstimates { get; set; }

		bool CloneCausedReload { get; }

		int GameDays { get; set; }

		bool IsHidden { get; set; }

		bool IsNewData { get; }

		DateTime? LastUpdated { get; set; }

		List<IMovie> Movies { get; }

		string Error { get; set; }

		string ErrorDetail { get; set; }

		/// <summary>
		/// Name of the Miner
		/// </summary>
		string Name { get; }

		/// <summary>
		/// By default this is set to true.
		/// </summary>
		bool OkToMine { get; set; }

		IMovieList Picks { get; set; }

		IMovieList PicksBonusOff { get; set; }

		/// <summary>
		/// The Twitter handle/ID of the miner source.  Do NOT include the @ sign.
		/// </summary>
		string TwitterID { get; }

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

		/// <summary>
		/// Since the main object is singleton then you need to be able to return copies to the views,
		/// because this contains the data and its state (Error/Status)
		/// </summary>
		/// <returns></returns>
		IMiner Clone();

		List<IMovie> Mine();

		void SetMovies(List<IMovie> movies);

		//Task<List<IMovie>> MineAsync();			// Not just yet.
	}
}