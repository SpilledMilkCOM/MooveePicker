using System;

namespace MoviePicker.Common.Interfaces
{
    public interface IMovie
    {
		string Abbreviation { get; }

        /// <summary>
        /// Allow simulations to adjust this movie's earnings.
        /// </summary>
        bool AdjustEarnings { get; set; }

		/// <summary>
		/// FML Bux
		/// </summary>
        decimal Cost { get; set; }

		/// <summary>
		/// The movie COULD be a multi-day pick (Friday, Saturday, Sunday)
		/// </summary>
		DayOfWeek? Day { get; set; }

		/// <summary>
		/// Box Office $$$ per FML Bux
		/// </summary>
		decimal Efficiency { get; }

		/// <summary>
		/// The Date based weekend ending (Sunday)
		/// </summary>
        DateTime WeekendEnding { get; set; }

		/// <summary>
		/// The Box Office earnings
		/// </summary>
        decimal Earnings { get; set; }

		/// <summary>
		/// The Box Office earnings without any bonuses
		/// </summary>
		decimal EarningsBase { get; }

		string Hashtag { get; }

		/// <summary>
		/// An arbitrary Id for the movie
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Local image Url (unless there isn't one then it's the same as the source)
		/// </summary>
		string ImageUrl { get; set; }

		/// <summary>
		/// The source image url that will not change.
		/// </summary>
		string ImageUrlSource { get; set; }

		/// <summary>
		/// Set to true if the Efficiency is the best out of all the other movies.
		/// </summary>
		bool IsBestPerformer { get; set; }

		/// <summary>
		/// The name of the movie (NOT including the day of the week, if applicable)
		/// </summary>
		string MovieName { get; set; }

		/// <summary>
		/// The name of the movie (including the day of the week, if applicable)
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Create a copy of this movie
		/// </summary>
		/// <returns></returns>
        IMovie Clone();

		/// <summary>
		/// A unique identifier typically based on the data contained within the object.
		/// </summary>
		/// <returns></returns>
        int GetHashCode();
    }
}