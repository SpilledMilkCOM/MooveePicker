using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IHistoryViewModel
	{
		long Duration { get; set; }

		IEnumerable<IMovie> Movies { get; set; }
	}
}