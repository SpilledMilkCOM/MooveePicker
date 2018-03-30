using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IMovieListModel
	{
		/// <summary>
		/// The list of raw movie numbers for comparison.  If the Picks are YOUR picks then these are ESTIMATED values (for comparison).
		/// If the Picks are FML Perfect Picks then these are YOUR values (for comparison).
		/// </summary>
		IEnumerable<IMovie> ComparisonMovies { get; set; }

		string ComparisonHeader { get; set; }

		/// <summary>
		/// These can be either YOUR picks with the bonus on or off,
		/// or this can be the FML Perfect Picks based on the estimated actuals.
		/// </summary>
		List<IMovieList> Picks { get; set; }

		/// <summary>
		/// A list of the rest of the picks
		/// If you're looking at Top 3, then this list will contain 2 movie lists.
		/// </summary>
		List<IMovieList> PicksTheRest { get; set; }

		/// <summary>
		/// The total value of the picks using the Box Office of the comparison.
		/// (Will calculate the total Box Offce of a list of picks using FML Estimated values.)
		/// </summary>
		decimal TotalPicksFromComparison { get; }
	}
}