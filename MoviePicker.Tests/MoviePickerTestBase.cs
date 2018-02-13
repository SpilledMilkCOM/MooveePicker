using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity;

namespace MoviePicker.Tests
{
	[ExcludeFromCodeCoverage]
	public abstract class MoviePickerTestBase
	{
		private ILogger _logger;

		/// <summary>
		/// This is public so it can be modified from the TestHarness.
		/// </summary>
		public abstract IUnityContainer UnityContainer { get; }

		protected ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					lock (this)
					{
						if (_logger == null)
						{
							AddDefaultLogger();

							_logger = UnityContainer.Resolve<ILogger>();
						}
					}
				}
				return _logger;
			}
		}	

		protected virtual IMoviePicker ConstructTestObject()
		{
			AddDefaultLogger();

			return UnityContainer.Resolve<IMoviePicker>();
		}

		private void AddDefaultLogger()
		{
			if (UnityContainer.Registrations.FirstOrDefault(registration => registration.RegisteredType == typeof(ILogger)) == null)
			{
				// Register the DebugLogger if the interface is not defined.
				UnityContainer.RegisterType<ILogger, DebugLogger>();
			}
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

		protected void WriteMovies(IEnumerable<IMovie> movies)
		{
			Logger.WriteLine($"Total Movies In List: {movies.Count()}");

			foreach (var movie in movies.OrderByDescending(item => item.Earnings))
			{
				var isBestBonus = movie.IsBestPerformer ? " *$2,000,000*" : string.Empty;

				if (movie.Cost > 0)
				{
					Logger.WriteLine($"{movie.WeekendEnding.ToString("d")} {movie.Name,-30} {movie.Cost,3} Bx   ${movie.Earnings,13:N2} - [${movie.Efficiency,10:N2}]{isBestBonus}");
				}
				else
				{
					Logger.WriteLine($"{movie.WeekendEnding.ToString("d")} {movie.Name,-30} ${movie.Earnings:N2}");
				}
			}
		}

		protected void WriteMovies(IMovieList movies)
		{
			int screen = 1;

			Logger.WriteLine($"Total Cost (Bux): {movies.TotalCost}");
			Logger.WriteLine($"Total Earnings  : ${movies.TotalEarnings:N0}");

			foreach (var movie in movies.Movies.OrderByDescending(item => item.Earnings))
			{
			    var isBestBonus = movie.IsBestPerformer ? " *$2,000,000*" : string.Empty;

				Logger.WriteLine($"{screen++} - {movie.Name,-30} ${movie.Earnings:N2} - [${movie.Efficiency:N2}]{isBestBonus}");
			}
		}

		protected void WritePicker(IMoviePicker moviePicker)
		{
			Logger.WriteLine($"Picker: {moviePicker.GetType().Name}");
			Logger.WriteLine($"Total Comparisons: {moviePicker.TotalComparisons:N0} [{moviePicker.TotalComparisons / Math.Pow(16, 8) * 100}% of {Math.Pow(16, 8):N0}]");
			Logger.WriteLine($"Total Sub-problems: {moviePicker.TotalSubProblems:N0}");
		}
	}
}