using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using Unity;

namespace XUnitTests
{
	public class ParkerMoviePickerValidationTestsContext : MoviePickerValidationTestsContext
	{
		public ParkerMoviePickerValidationTestsContext()
		{
			SetupContainer();
		}
		protected sealed override void SetupContainer()
		{
			UnityContainer.RegisterType<IMovie, Movie>();
			UnityContainer.RegisterType<IMovieList, MovieList>();
			UnityContainer.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();
		}
	}
}