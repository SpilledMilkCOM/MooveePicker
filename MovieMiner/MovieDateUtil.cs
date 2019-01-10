using System;
using System.Collections.Generic;

namespace MovieMiner
{
	public static class MovieDateUtil
	{
		private const int TZ_OFFSET = -7;        // The mountain time zone is 7 hours behind UTC.

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
			DateTime reference = dateTime ?? Now;
			var diff = reference.Subtract(StartOfSeason);
			var index = diff.Days / (DAYS_IN_WEEK * WEEKS_IN_SEASON);
			string result = null;

			if (index < _seasons.Count)
			{
				result = _seasons[index];
			}

			return result;
		}

		public static int DateToWeek(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? NextSunday();

			var diff = reference.Subtract(StartOfSeason);
			return (diff.Days / DAYS_IN_WEEK) % WEEKS_IN_SEASON + 1;
		}

		/// <summary>
		/// The date of the Sunday's (end of game)
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="isEstimate"></param>
		/// <returns></returns>
		public static DateTime GameSunday(DateTime? dateTime = null, bool isEstimate = false)
		{
			DateTime result = dateTime ?? Now;

			if (result.DayOfWeek == DayOfWeek.Monday && isEstimate)
			{
				result = result.AddDays(-1);
			}
			else
			{
				result = ThisSunday(result);
			}

			return result;
		}

		/// <summary>
		/// Return the previous Sunday.
		/// </summary>
		/// <param name="dateTime">Reference date (or null for Now)</param>
		/// <returns>Previous Sunday</returns>
		public static DateTime LastSunday(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? Now;

			return reference.AddDays(DayOfWeek.Sunday - reference.DayOfWeek);
		}

		/// <summary>
		/// Find the upcoming Sunday.
		/// </summary>
		/// <param name="dateTime">Reference date (or null for Now)</param>
		/// <returns>Next Sunday</returns>
		public static DateTime NextSunday(DateTime? dateTime = null)
		{
			DateTime reference = dateTime ?? Now;

			return reference.AddDays(7 - (int)reference.DayOfWeek);
		}

		/// <summary>
		/// Find the upcoming Sunday unless the reference day (or today) is Sunday.
		/// </summary>
		/// <param name="dateTime">Reference date (or null for Now)</param>
		/// <returns>The reference Sunday or the following Sunday</returns>
		public static DateTime ThisSunday(DateTime? dateTime = null)
		{
			DateTime result = dateTime ?? Now;

			if (result.DayOfWeek != DayOfWeek.Sunday)
			{
				// Sunday = 0 --> Saturday = 6
				result = result.AddDays(7 - (int)result.DayOfWeek);
			}

			return result;
		}

		public static TimeSpan UntilGameTime()
		{
			// The mountain time zone is 7 hours behind UTC.

			var now = DateTime.Now;
			var start = GameStartTime();
			var end = GameEndTime();
			TimeSpan result = new TimeSpan(0);
		
			if (start < end)
			{
				result = start.Subtract(now);
			}

			return new TimeSpan();
		}

		/// <summary>
		/// Always moving forward from Now
		/// </summary>
		/// <returns>Game start time within the time zone</returns>
		public static DateTime GameEndTime()
		{
			var now = Now;
			var result = new DateTime(now.Year, now.Month, now.Day + (DayOfWeek.Tuesday - now.DayOfWeek), now.Hour, now.Minute, now.Second);

			if (DayOfWeek.Friday - now.DayOfWeek < 0)
			{
				// Went backwards to Tuesday so add a whole week.

				result.AddDays(7);
			}

			return result;
		}

		/// <summary>
		/// Always moving forward from Now
		/// </summary>
		/// <returns>Game start time within the time zone</returns>
		public static DateTime GameStartTime()
		{
			var now = Now;
			var result = new DateTime(now.Year, now.Month, now.Day + (DayOfWeek.Friday - now.DayOfWeek), now.Hour, now.Minute, now.Second);

			if (DayOfWeek.Friday - now.DayOfWeek < 0)
			{
				// Went backwards to Friday so add a whole week.

				result.AddDays(7);
			}

			return result;
		}

		private static DateTime Now => DateTime.Now.AddHours(TZ_OFFSET).Date;
	}
}