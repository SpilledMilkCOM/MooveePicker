using MovieMiner;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IHomeViewModel
	{
		IEnumerable<IMiner> Miners { get; set; }
	}
}