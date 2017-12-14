using MovieMiner;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IIndexViewModel
	{
		IEnumerable<IMiner> Miners { get; set; }

		decimal Weight1 { get; set; }

		decimal Weight2 { get; set; }

		decimal Weight3 { get; set; }

		decimal Weight4 { get; set; }

		decimal Weight5 { get; set; }

		decimal Weight6 { get; set; }

		decimal Weight7 { get; set; }

		string GetFMLNerdLink(IMiner miner);
	}
}