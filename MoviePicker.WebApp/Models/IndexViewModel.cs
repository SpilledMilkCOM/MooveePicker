using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public class IndexViewModel : IIndexViewModel
	{
		public IEnumerable<IMiner> Miners { get; set; }
	}
}