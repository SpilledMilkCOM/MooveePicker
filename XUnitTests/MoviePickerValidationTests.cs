using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Unity;

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
		public void MoviePicker_ChooseBest_WeekEnding_20170604_Week1()
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
		public void MoviePicker_ChooseBest_WeekEnding_20170611_Week2()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();
			int id = 1;

			movies.Add(ConstructMovie(id++, "Wonder Woman", 58.5m, 613));
			movies.Add(ConstructMovie(id++, "The Mummy", 31.7m, 526));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 12.2m, 198));
			movies.Add(ConstructMovie(id++, "It Comes at night", 5.9m, 150));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 10.7m, 143));
			movies.Add(ConstructMovie(id++, "Megan Leavey", 3.8m, 59));
			movies.Add(ConstructMovie(id++, "Baywatch", 4.6m, 69));
			movies.Add(ConstructMovie(id++, "Alien: Covenant", 1.8m, 26));
			movies.Add(ConstructMovie(id++, "My Cousin Rachel", 0.95m, 15));
			movies.Add(ConstructMovie(id++, "Diary of a wimpy Kid", 0.65m, 8));
			movies.Add(ConstructMovie(id++, "Everything, Everything", 1.6m, 28));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 6.3m, 70));
			movies.Add(ConstructMovie(id++, "King Arthur", 0.45m, 7));
			movies.Add(ConstructMovie(id++, "Snatched", 0.49m, 9));
			movies.Add(ConstructMovie(id++, "Best of the rest", 0.5m, 9));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Wonder Woman"));
			Assert.Equal(5, best.Movies.Count(movie => movie.Name == "Guardians of the Galaxy"));
			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Alien: Covenant"));
			Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Diary of a wimpy Kid"));
		}

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170618_Week3()
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
		public void MoviePicker_ChooseBest_WeekEnding_20170625_Week4()
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

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170702_Week5()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;

			/*movies.Add(ConstructMovie(id++, "Transformers", 44.7m, 560));
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
			movies.Add(ConstructMovie(id++, "The Book of Henry", 0.95m, 11));*/
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 72.4m, 840));
			movies.Add(ConstructMovie(id++, "The House", 8.7m, 198));
			movies.Add(ConstructMovie(id++, "Transformers", 17m, 175));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 15.7m, 131));
			movies.Add(ConstructMovie(id++, "Baby Driver", 21m, 110));
			movies.Add(ConstructMovie(id++, "Cars 3", 9.7m, 102));
			movies.Add(ConstructMovie(id++, "All Eyez on Me", 1.83m, 19));
			movies.Add(ConstructMovie(id++, "The Mummy", 2.9m, 26));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 2.5m, 26));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 4.5m, 36));
			movies.Add(ConstructMovie(id++, "Captain Underpants", 1.1m, 19));
			movies.Add(ConstructMovie(id++, "Rough Night", 0m, 23));
			movies.Add(ConstructMovie(id++, "The Beguiled", 3.2m, 32));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 1.53m, 15));
			movies.Add(ConstructMovie(id++, "Beatriz At Dinner", 1.19m, 12));



			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			Assert.Equal(8, best.Movies.Count(movie => movie.Name == "Baby Driver"));
		}

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170702_Week6()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;

			movies.Add(ConstructMovie(id++, "FRI: Spider-Man: Homecoming", 47.3m, 501));
			movies.Add(ConstructMovie(id++, "SAT: Spider-Man: Homecoming", 41.4m, 448));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 33.85m, 404));
			movies.Add(ConstructMovie(id++, "SUN: Spider-Man: Homecoming", 31.1m, 339));
			movies.Add(ConstructMovie(id++, "Baby Driver", 13.8m, 143));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 9.35m, 104));
			movies.Add(ConstructMovie(id++, "Transformers", 6.897m, 70));
			movies.Add(ConstructMovie(id++, "Cars 3", 4.75m, 47));
			movies.Add(ConstructMovie(id++, "The House", 4.407m, 46));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 2.7m, 30));
			movies.Add(ConstructMovie(id++, "The Big Sick", 2.8585m, 28));
			movies.Add(ConstructMovie(id++, "The Beguiled", 2m, 21));
			movies.Add(ConstructMovie(id++, "The Mummy", 1.2795m, 14));
			movies.Add(ConstructMovie(id++, "Pirates of the caribbean", 1.225m, 13));
			movies.Add(ConstructMovie(id++, "Guardians of the Galaxy", 0.7705m, 8));

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			//Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Transformers"));
			//Assert.Equal(7, best.Movies.Count(movie => movie.Name == "47 Meters Down"));
		}

		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_20170716_Week7()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;

			movies.Add(ConstructMovie(id++, "Wish Upon", 6.2m, 92));
			movies.Add(ConstructMovie(id++, "The House", 2.15m, 26));
			movies.Add(ConstructMovie(id++, "Spider-Man", 53.1m, 586));
			movies.Add(ConstructMovie(id++, "Despicable Me 3", 17.7m, 201));
			movies.Add(ConstructMovie(id++, "The Big Sick", 9.25m, 143));
			movies.Add(ConstructMovie(id++, "Wonder Woman", 5.9m, 68));
			movies.Add(ConstructMovie(id++, "47 Meters Down", 1.4m, 17));
			movies.Add(ConstructMovie(id++, "Baby Driver", 8.25m, 93));
			movies.Add(ConstructMovie(id++, "Cars 3", 2.9m, 33));
			movies.Add(ConstructMovie(id++, "Transformers", 2.65m, 30));
			movies.Add(ConstructMovie(id++, "Best of the Rest", 0.4m, 5));
			movies.Add(ConstructMovie(id++, "The Mummy", 0.4m, 5));
			movies.Add(ConstructMovie(id++, "The Beguiled", 1m, 12));
			movies.Add(ConstructMovie(id++, "Pirates of the Caribbean", 0.5m, 6));
			movies.Add(ConstructMovie(id++, "War for the Planet of the Apes", 59.2m, 705));


			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			//Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Transformers"));
			//Assert.Equal(7, best.Movies.Count(movie => movie.Name == "47 Meters Down"));
		}

		/*
		[Fact]
		public void MoviePicker_ChooseBest_WeekEnding_2017MMDD_Week_N_Template()
		{
			var test = ConstructTestObject();
			var movies = new List<IMovie>();

			int id = 1;

			

			test.AddMovies(movies);

			var best = test.ChooseBest();

			WritePicker(test);
			WriteMovies(best);

			//Assert.Equal(1, best.Movies.Count(movie => movie.Name == "Transformers"));
			//Assert.Equal(7, best.Movies.Count(movie => movie.Name == "47 Meters Down"));
		}
		 */

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
