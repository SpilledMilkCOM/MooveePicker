using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MooveePicker;

namespace MooviePicker.Tests
{
	[TestClass]
	public class MoviePickerTests
	{
		private static UnityContainer _unity;

		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			_unity.RegisterType<IMovie, Movie>();
			_unity.RegisterType<IMovieList, MovieList>();
			_unity.RegisterType<IMoviePicker, MoviePicker>();
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf01()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(1).ToList());

			var best = test.ChooseBest();

			Assert.AreEqual(1, best.Movies.Count());

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf02()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(2).ToList());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf03()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(3).ToList());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf04()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(4).ToList());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf06()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(6).ToList());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBestOutOf10()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies().Take(10).ToList());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		[TestMethod]
		public void MoviePicker_ChooseBest_ThisWeeksPicks()
		{
			var test = ConstructTestObject();

			test.AddMovies(ThisWeeksMovies());

			var best = test.ChooseBest();

			WriteMovies(best);
		}

		//----==== PRIVATE ====---------------------------------------------------------

		private IMoviePicker ConstructTestObject()
		{
			return _unity.Resolve<IMoviePicker>();
		}

		private IMovie ConstructMovie(int id, string name, decimal millions, decimal cost)
		{
			var result = _unity.Resolve<IMovie>();

			result.Id = id;
			result.Name = name;
			result.Earnings = millions * 1000000m;
			result.Cost = cost;

			return result;
		}

		private List<IMovie> ThisWeeksMovies()
		{
			var movies = new List<IMovie>();

			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 55, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 38, 526));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12, 198));
			movies.Add(ConstructMovie(id++, "It Comes at Night", 20, 150));
			movies.Add(ConstructMovie(id++, "Pirates", 12, 143));
			movies.Add(ConstructMovie(id++, "Guardians", 5, 70));
			movies.Add(ConstructMovie(id++, "Baywatch", 5, 60));
			movies.Add(ConstructMovie(id++, "Meagan Leavey", 3.3m, 59));
			movies.Add(ConstructMovie(id++, "Everything", 1.5m, 28));
			movies.Add(ConstructMovie(id++, "Alien", 2.1m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 1, 15));
			movies.Add(ConstructMovie(id++, "Snatched", 0.6m, 9));
			movies.Add(ConstructMovie(id++, "Best of the Rest", 1.1m, 9));
			movies.Add(ConstructMovie(id++, "Diary of a Wimpy Kid", 0.6m, 8));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.5m, 7));

			return movies;
		}

		private void WriteMovies(IMovieList movies)
		{
			int screen = 1;

			Debug.WriteLine($"Total Cost: {movies.TotalCost}");
			Debug.WriteLine($"Total Earnings: {movies.TotalEarnings:N0}");

			foreach (var movie in movies.Movies.OrderByDescending(item => item.Earnings))
			{
				Debug.WriteLine($"{screen} - {movie.Name}");
			}
		}
	}
}