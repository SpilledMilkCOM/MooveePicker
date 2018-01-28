using MovieMiner;
using System;
using System.Linq;

namespace MoviePicker.WebApp.Utilities
{
	public static class ViewUtility
	{
		public static string PercentColor(decimal percent)
		{
			string color = null;

			if (Math.Abs(percent) > 1.0m)
			{
				color = "#00cccc";
			}
			else if (Math.Abs(percent) > 0.50m)
			{
				color = "#00ffff";
			}
			else if (Math.Abs(percent) > 0.20m)
			{
				color = "#4dffff";
			}
			else if (Math.Abs(percent) > 0.15m)
			{
				color = "#b3ffff";
			}
			else if (Math.Abs(percent) > 0.1m)
			{
				color = "#ccffff";
			}
			else if (Math.Abs(percent) > 0.05m)
			{
				color = "#e6ffff";
			}

			return (color != null) ? $"background-color: {color};" : string.Empty;
		}

		public static decimal? PercentAwayFromEstimates(IMiner estimate, IMiner miner)
		{
			int dataPointCount = 0;
			decimal sumOfDiffPercent = 0;
			decimal? result = null;

			foreach (var estimatedMovie in estimate.Movies)
			{
				var minerMovie = miner.Movies.FirstOrDefault(movie => movie.Name == estimatedMovie.Name);

				if (minerMovie != null && minerMovie.EarningsBase > 0)
				{
					sumOfDiffPercent += Math.Abs(estimatedMovie.EarningsBase - minerMovie.EarningsBase) / minerMovie.EarningsBase;

					dataPointCount++;
				}
			}

			if (dataPointCount > 0)
			{
				result = sumOfDiffPercent / dataPointCount;
			}

			return result;
		}
	}
}