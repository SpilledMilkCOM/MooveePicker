using System.Collections.Generic;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.ViewModels
{
	public class HistoryViewModel : IHistoryViewModel
	{
		public long Duration { get; set; }

		public IEnumerable<IMovie> Movies { get; set; }
	}
}