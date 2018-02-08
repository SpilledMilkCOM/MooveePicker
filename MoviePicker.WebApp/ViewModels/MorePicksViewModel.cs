using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class MorePicksViewModel
	{
		public long Duration { get; set; }

		public List<MovieListModel> MorePicks { get; set; }

		public List<MovieListModel> MorePicksBonusOff { get; set; }
	}
}