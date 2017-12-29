using MoviePicker.Common.Interfaces;
using System.Collections.Generic;

namespace MoviePicker.WebApp.Models
{
	public interface ISimulationModel
	{

		void AddMovies(IEnumerable<IMovie> movies);

		IMovieList ChooseBest();
	}
}