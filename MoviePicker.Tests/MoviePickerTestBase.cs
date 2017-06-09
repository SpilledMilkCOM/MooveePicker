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
	}
}