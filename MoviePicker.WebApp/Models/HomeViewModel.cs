using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class HomeViewModel : IHomeViewModel
	{
		public IEnumerable<IMiner> Miners { get; set; }
	}
}