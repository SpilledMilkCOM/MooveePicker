using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class MorePicksViewModel
	{
		public long Duration { get; set; }

		public IEnumerable<IMovieListModel> MorePicks { get; set; }
	}
}