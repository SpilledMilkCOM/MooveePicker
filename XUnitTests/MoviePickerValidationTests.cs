using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using MoviePicker.Common.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTests
{
	public abstract class MoviePickerTestBase
	{
		protected ITestOutputHelper OutputHelper { get; set; }

		protected MoviePickerValidationTestsContext Context { get; set; }

		public MoviePickerTestBase(ITestOutputHelper outputHelper, MoviePickerValidationTestsContext context)
		{
			OutputHelper = outputHelper;
			Context = context;
		}

		

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170604()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;
			movies.Add(ConstructMovie(id++, "Wonder Woman", 103, 845));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 23.9m, 239));
			movies.Add(ConstructMovie(id++, "Pirates", 22.1m, 193));
			movies.Add(ConstructMovie(id++, "Guardians", 9.8m, 74));
			movies.Add(ConstructMovie(id++, "Baywatch", 8.7m, 62));
			movies.Add(ConstructMovie(id++, "Alien", 4.1m, 31));
			movies.Add(ConstructMovie(id++, "Everything Everything", 3.3m, 22));
			movies.Add(ConstructMovie(id++, "Diary of a Wimpy Kid", 1.3m, 17));
			movies.Add(ConstructMovie(id++, "Snatched", 1.3m, 14));
			movies.Add(ConstructMovie(id++, "King Arthur", 1.2m, 12));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.Equal(7, best.Movies.Count(movie => movie.Name == "Everything Everything"));
		}

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170618()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;
			movies.Add(ConstructMovie(id++, "Wonder Woman", 41.3m, 478));
			movies.Add(ConstructMovie(id++, "Cars 3", 53.7m, 719));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 26.4m, 327));
			movies.Add(ConstructMovie(id++, "The Mummy", 14.5m, 167));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 9m, 71));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 11.2m, 105));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 7.2m, 78));
			movies.Add(ConstructMovie(id++, "Rough Night", 8m, 243));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 5.1m, 60));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 2.5m, 25));
			movies.Add(ConstructMovie(id++, "It Comes at night", 2.6m, 34));
			movies.Add(ConstructMovie(id++, "The Book of Henry", 1.4m, 31));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 0.568m, 11));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 0.388m, 10));
			movies.Add(ConstructMovie(id++, "Baywatch", 1.6m, 29));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.Equal(7, best.Movies.Count(movie => movie.Name == "Pirates of the caribbean"));
		}

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170625()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;
			
			movies.Add(ConstructMovie(id++, "Transformers", 44.7m, 560));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 24.9m, 286));
			movies.Add(ConstructMovie(id++, "Cars 3", 24.1m, 278));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 5.8m, 104));
			movies.Add(ConstructMovie(id++, "The Mummy", 6.1m, 73));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 5.4m, 58));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 7.1m, 55));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 4.3m, 45));
			movies.Add(ConstructMovie(id++, "Rough Night", 4.7m, 39));
			movies.Add(ConstructMovie(id++, "Tubelight", 0.9m, 34));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 3.0m, 32));
			movies.Add(ConstructMovie(id++, "Beatriz At Dinner", 1.8m, 17));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 1.2m, 16));
			movies.Add(ConstructMovie(id++, "It Comes at night", 0.8m, 13));
			movies.Add(ConstructMovie(id++, "The Book of Henry", 0.95m, 11));


			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Transformers"));
			Assert.Equal(7, best.Movies.Count(movie => movie.Name == "47 Meters Down"));
		}

		protected IMoviePicker ConstructTestObject()
		{
			if (Context == null)
			{
				throw new NullReferenceException("Context must be set before construction the subject under test");
			}
			return Context.UnityContainer.Resolve<IMoviePicker>();
		}

		protected IMovie ConstructMovie(int id, string name, decimal millions, decimal cost)
		{
			var result = Context.UnityContainer.Resolve<IMovie>();

			result.Id = id;
			result.Name = name;
			result.Earnings = millions * 1000000m;
			result.Cost = cost;

			return result;
		}

		protected void WriteMovies(IMovieList movies)
		{
			int screen = 1;

			OutputHelper.WriteLine($"Total Cost (Bux): {movies.TotalCost}");
			OutputHelper.WriteLine($"Total Earnings  : ${movies.TotalEarnings:N0}");

			foreach (var movie in movies.Movies.OrderByDescending(item => item.Earnings))
			{
				OutputHelper.WriteLine($"{screen++} - {movie.Name,-30} ${movie.Earnings:N2} - [${movie.Efficiency:N2}]");
			}
		}

		protected void WritePicker(IMoviePicker moviePicker)
		{
			OutputHelper.WriteLine($"Picker: {moviePicker.GetType().Name}");
			OutputHelper.WriteLine($"Total Comparisons: {moviePicker.TotalComparisons:N0} [{moviePicker.TotalComparisons / Math.Pow(16, 8) * 100}% of {Math.Pow(16, 8):N0}]");
			OutputHelper.WriteLine($"Total Sub-problems: {moviePicker.TotalSubProblems:N0}");
		}
	}
}
