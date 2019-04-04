using MovieMiner;
using MoviePicker.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace MoviePicker.WebApp.Utilities
{
	public static class ViewUtility
	{
		private const long KB = 1024;
		private const int SHIFT_KB = 8;

		public static string BytesToAbbreviation(long bytes)
		{
			var result = $"{bytes:N0} bytes";

			if (bytes < KB)
			{
				return result;
			}
			else if (bytes < KB * KB)
			{
				result = $"{bytes / KB:N1} KB";
			}
			else if (bytes < KB * KB * KB)
			{
				result = $"{bytes / (KB * KB):N1} MB";
			}
			else if (bytes < KB * KB * KB * KB)
			{
				result = $"{bytes / (KB * KB * KB):N1} GB";
			}
			else // if (bytes < KB << SHIFT_KB << SHIFT_KB << SHIFT_KB << SHIFT_KB)
			{
				result = $"{bytes / (KB * KB * KB * KB):N1} TB";
			}

			return result;
		}

		/// <summary>
		/// Return google graph data.
		/// </summary>
		/// <param name="history"></param>
		/// <returns></returns>
		public static string GraphData(IEnumerable<IBoxOffice> history)
		{
			var builder = new StringBuilder();
			var count = 0;

			builder.Append("[");

			foreach (var boxOffice in history)
			{
				if (count != 0)
				{
					builder.Append(", ");
				}

				builder.Append($"[{count}, {boxOffice.Earnings}]");

				count++;
			}

			builder.Append("]");

			return builder.ToString();
		}

		/// <summary>
		/// Return a background-color based on a percentage (positive or negative)
		/// </summary>
		/// <param name="percent">A positive or negative percentage.</param>
		/// <returns>A background-color</returns>
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

		/// <summary>
		/// Return a background-color based on a percentage (positive or negative)
		/// </summary>
		/// <param name="percent">A positive or negative percentage.</param>
		/// <returns>A background-color</returns>
		public static string PercentColorFromLastWeek(decimal percent)
		{
			string color = null;
			percent /= 100;

			if (percent > 1.0m)
			{
				// This is uber green.
				color = "#00ff00";
			}
			else if (percent > 0.75m)
			{
				color = "#33ff33";
			}
			else if (percent > 0.50m)
			{
				color = "#66ff66";
			}
			else if (percent > 0.25m)
			{
				color = "#99ff99";
			}
			else if (percent > 0.1m)
			{
				color = "#bbffbb";
			}
			else if (Math.Abs(percent) < 0.1m)
			{
				color = "#ffffff";
			}
			else if (Math.Abs(percent) < 0.20m)
			{
				color = "#ffeeee";
			}
			else if (Math.Abs(percent) < 0.30m)
			{
				color = "#ffdddd";
			}
			else if (Math.Abs(percent) < 0.40m)
			{
				color = "#ffcccc";
			}
			else if (Math.Abs(percent) < 0.50m)
			{
				color = "#ffbbbb";
			}
			else if (Math.Abs(percent) < 0.60m)
			{
				color = "#ffaaaa";
			}
			else if (Math.Abs(percent) < 0.70m)
			{
				color = "#ff9999";
			}
			else if (Math.Abs(percent) < 0.80m)
			{
				color = "#ff8888";
			}
			else if (Math.Abs(percent) < 1.0m)
			{
				color = "#ff6666";
			}
			else
			{
				color = "#ff0000";
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

		public static object RequestParamsToDynamic(HttpRequestBase request)
		{
			return new { bo = request.Params["bo"], wl = request.Params["wl"] };
		}

		/// <summary>
		/// Converts a web request's parameters to a dynamic object to be used in an Html.ActionLink() method.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>A dynamic object to be used in an ActionLink</returns>
		public static dynamic RequestParamsToDynamic(HttpRequestBase request, IList<string> userKeys)
		{
			var result = new ExpandoObject();
			var dictionary = (IDictionary<string, object>)result;

			foreach (string key in request.Params.Keys)
			{
				if (userKeys.Contains(key))
				{
					dictionary.Add(key, request.Params[key]);
				}
			}

			return result;
		}
	}
}