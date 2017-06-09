using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.Unity;
using MooveePicker;

namespace MoviePicker.Tests
{
	public abstract class MoviePickerTestBase
	{
		protected abstract IUnityContainer UnityContainer { get; }

		protected IMoviePicker ConstructTestObject()
		{
			return UnityContainer.Resolve<IMoviePicker>();
		}

		protected IMovie ConstructMovie(int id, string name, decimal millions, decimal cost)
		{
			var result = UnityContainer.Resolve<IMovie>();

			result.Id = id;
			result.Name = name;
			result.Earnings = millions * 1000000m;
			result.Cost = cost;

			return result;
		}

		protected void WriteMovies(IMovieList movies)
		{
			int screen = 1;

			Debug.WriteLine($"Total Cost (Bux): {movies.TotalCost}");
			Debug.WriteLine($"Total Earnings  : ${movies.TotalEarnings:N0}");

			foreach (var movie in movies.Movies.OrderByDescending(item => item.Earnings))
			{
				Debug.WriteLine($"{screen} - {movie.Name} ${movie.Earnings:N2} - [{movie.Efficiency}]");
			}
		}

		protected void WritePicker(IMoviePicker moviePicker)
		{
			Debug.WriteLine($"Total Comparisons: {((MooveePicker.MoviePicker)moviePicker).TotalComparisons:N0}");
			Debug.WriteLine($"Total Sub-problems: {((MooveePicker.MoviePicker)moviePicker).TotalSubProblems:N0}");
		}
	}
}