using System;

namespace MoviePicker.Common.Interfaces
{
    public interface IMovie
    {
        /// <summary>
        /// Allow simulations to adjust this movie's earnings.
        /// </summary>
        bool AdjustEarnings { get; set; }

        decimal Cost { get; set; }

        decimal Efficiency { get; }

        DateTime WeekendEnding { get; set; }

        decimal Earnings { get; set; }

        int Id { get; set; }

        bool IsBestPerformer { get; set; }

        string Name { get; set; }

        IMovie Clone();

        int GetHashCode();
    }
}