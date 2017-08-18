using System.Collections.Generic;

namespace MoviePicker.Common.Interfaces
{
	public interface IMoviePicker
	{
        /// <summary>
        /// THE best performer (if not null - if this is null then there is a tie)
        /// </summary>
        IMovie BestPerformer { get; }

        /// <summary>
        /// A list of best performing movies ($$/bux).
        /// NOTE: Return a list because there COULD be a tie.
        /// </summary>
        IEnumerable<IMovie> BestPerformers { get; }

		bool EnableBestPerformer { get; set; }

        IEnumerable<IMovie> Movies { get; }

		int TotalComparisons { get; set; }

		int TotalSubProblems { get; }

		void AddMovies(IEnumerable<IMovie> movies);

		IMovieList ChooseBest();
	}
}