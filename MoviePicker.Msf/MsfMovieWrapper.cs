using System;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Msf
{
	/// <summary>
	/// needed as MSF solves with doubles not decimals
	/// </summary>
	public class MsfMovieWrapper : IMovie
	{
		private readonly IMovie _movie;

		public MsfMovieWrapper(IMovie movie)
		{
			_movie = movie;
		}

		public decimal Cost
		{
			get { return _movie.Cost; }
			set { _movie.Cost = value; }
		}

		public double CostAsDouble
		{
			get { return (double)_movie.Cost; }
			set { _movie.Cost = (decimal)value; }
		}

		public decimal Efficiency => _movie.Efficiency;

		public DateTime WeekendEnding
		{
			get { return _movie.WeekendEnding; }
			set { _movie.WeekendEnding = value; }
		}

		public decimal Earnings
		{
			get { return _movie.Earnings; }
			set { _movie.Earnings = value; }
		}

		public double EarningsAsDouble
		{
			get { return (double)_movie.Earnings; }
			set { _movie.Earnings = (decimal)value; }
		}

		public int Id
		{
			get { return _movie.Id; }
			set { _movie.Id = value; }
		}

		public string Name
		{
			get { return _movie.Name; }
			set { _movie.Name = value; }
		}

		public IMovie Clone()
		{
			throw new NotImplementedException();
		}
	}
}