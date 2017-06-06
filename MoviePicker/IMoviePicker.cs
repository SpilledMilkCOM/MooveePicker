using System.Collections.Generic;

namespace MooveePicker
{
	public interface IMoviePicker
	{
		IEnumerable<IMovie> Movies { get; }

		void AddMovies(IEnumerable<IMovie> movies);

		IMovieList ChooseBest();
	}
}