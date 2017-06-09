using System.Collections.Generic;

namespace MooveePicker
{
	public interface IMoviePicker
	{
		IEnumerable<IMovie> Movies { get; }

		int TotalComparisons { get; set; }

		int TotalSubProblems { get; }

		void AddMovies(IEnumerable<IMovie> movies);

		IMovieList ChooseBest();
	}
}