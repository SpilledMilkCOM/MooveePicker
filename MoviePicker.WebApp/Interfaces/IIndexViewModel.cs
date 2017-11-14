﻿using MovieMiner;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IIndexViewModel
	{
		IEnumerable<IMiner> Miners { get; set; }

		string GetFMLNerdLink(IMiner miner);
	}
}