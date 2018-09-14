using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System.Collections.Generic;

namespace MoviePicker.WebApp.ViewModels
{
	public class ExpertPicksViewModel
	{
		public ExpertPicksViewModel()
		{
			ExpertPicks = new List<ExpertPickModel>();
		}

		public long Duration { get; set; }

		public List<ExpertPickModel> ExpertPicks { get; private set; }
	}
}