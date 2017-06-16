using Microsoft.Practices.Unity;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;

namespace XUnitTests
{
	public class MsfMoviePickerValidationTestsContext : MoviePickerValidationTestsContext
	{
		public MsfMoviePickerValidationTestsContext()
		{
			SetupContainer();
		}
		protected sealed override void SetupContainer() 
		{
			UnityContainer.RegisterType<IMovie, Movie>();
			UnityContainer.RegisterType<IMovieList, MovieList>();
			UnityContainer.RegisterType<IMoviePicker, MsfMovieSolver>();
		}
	}
}