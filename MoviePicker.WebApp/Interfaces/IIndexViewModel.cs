using MovieMiner;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IIndexViewModel
	{
		IEnumerable<IMiner> Miners { get; set; }

		int Weight1 { get; set; }

		int Weight2 { get; set; }

		int Weight3 { get; set; }

		int Weight4 { get; set; }

		int Weight5 { get; set; }

		int Weight6 { get; set; }

		string GetFMLNerdLink(IMiner miner);
	}
}