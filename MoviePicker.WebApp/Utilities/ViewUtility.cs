using MovieMiner;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

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