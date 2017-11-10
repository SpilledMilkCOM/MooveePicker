using MovieMiner;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IMinerModel
	{
		List<IMiner> CreateMinersWithData();
	}
}