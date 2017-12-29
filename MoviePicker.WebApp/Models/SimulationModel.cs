using System.Collections.Generic;
using MooveePicker.Simulations;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.WebApp.Models
{
	public class SimulationModel : ISimulationModel
	{
		private IMoviePicker _moviePicker;

		public SimulationModel(IMovieList movieList)
		{
			_moviePicker = new MoviePickerVariantsAll(movieList, null);
		}

		public void AddMovies(IEnumerable<IMovie> movies)
		{
			_moviePicker.AddMovies(movies);
		}

		public IMovieList ChooseBest()
		{
			return _moviePicker.ChooseBest();
		}
	}
}