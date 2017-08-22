using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using MoviePicker.Common.Interfaces;

namespace MovieMiner.Tests
{
	public abstract class MineTestBase
	{
		protected const string PRIMARY_TEST_CATEGORY = "Mining";

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

		public void AddDefaultLogger()
		{
			if (UnityContainer.Registrations.FirstOrDefault(registration => registration.RegisteredType == typeof(ILogger)) == null)
			{
				// Register the DebugLogger if the interface is not defined.
				UnityContainer.RegisterType<ILogger, DebugLogger>();
			}
		}

		protected virtual IMoviePicker ConstructTestObject()
		{
			AddDefaultLogger();

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

			Logger.WriteLine($"Total Cost (Bux): {movies.TotalCost}");
			Logger.WriteLine($"Total Earnings  : ${movies.TotalEarnings:N0}");

			foreach (var movie in movies.Movies.OrderByDescending(item => item.Earnings))
			{
			    var isBestBonus = movie.IsBestPerformer ? " *$2,000,000*" : string.Empty;

				Logger.WriteLine($"{screen++} - {movie.Name,-30} ${movie.Earnings:N2} - [${movie.Efficiency:N2}]{isBestBonus}");
			}
		}

		protected void WriteMovies(IEnumerable<IMovie> movies)
		{
			foreach (var movie in movies.OrderByDescending(item => item.Earnings))
			{
				var isBestBonus = movie.IsBestPerformer ? " *$2,000,000*" : string.Empty;

				if (movie.Cost > 0)
				{
					Logger.WriteLine($"{movie.WeekendEnding.ToString("d")} {movie.Name,-30} {movie.Cost} Bx   ${movie.Earnings:N2} - [${movie.Efficiency:N2}]{isBestBonus}");
				}
				else
				{
					Logger.WriteLine($"{movie.WeekendEnding.ToString("d")} {movie.Name,-30} ${movie.Earnings:N2}");
				}
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