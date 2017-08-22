using System;
using System.Collections.Generic;

namespace MovieMiner
{
	public static class MovieDateUtil
	{
		// This offset may change depending on what year we're in (or the whims of FML)
		private const int SEASON_OFFSET_WEEKS = 8;
		private const int DAYS_IN_WEEK = 7;
		private const int WEEKS_IN_SEASON = 13;

		private static readonly List<string> _seasons = new List<string> { "Spring", "Summer", "Fall", "Winter" };

		/// <summary>
		/// The first Sunday (weekend ending) of the spring season.
		/// </summary>
		public static DateTime StartOfSeason => NextSunday(new DateTime(DateTime.Now.Year, 1, 1)).AddDays(7 * SEASON_OFFSET_WEEKS);

		public static string DateToSeason(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? DateTime.Now;
			var diff = reference.Subtract(StartOfSeason);
			var index = diff.Days / (DAYS_IN_WEEK * WEEKS_IN_SEASON);
			string result = null;

			if (index < _seasons.Count)
			{
				result = _seasons[index];
			}

			return result;
		}

		public static DateTime LastSunday(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? DateTime.Now;

			return reference.AddDays(DayOfWeek.Sunday - reference.DayOfWeek);
		}

		public static DateTime NextSunday(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? DateTime.Now;

			return reference.AddDays(7 - (int)reference.DayOfWeek);
		}
	}
}