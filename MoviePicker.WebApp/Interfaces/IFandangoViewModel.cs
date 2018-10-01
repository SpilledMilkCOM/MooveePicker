using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IFandangoViewModel
	{
		long Duration { get; set; }

		bool IsTracking { get; }

		int PastHours { get; set; }

		IMovieListModel MovieListPerfectPick { get; set; }

		IEnumerable<IMovie> Movies { get; }

		void Load();
	}
}