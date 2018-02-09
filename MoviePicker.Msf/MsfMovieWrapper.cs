﻿using System;
using System.Diagnostics;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Msf
{
	/// <summary>
	/// needed as MSF solves with doubles not decimals
	/// </summary>
	[DebuggerDisplay("Id = {Id} - Name = {Name}")]
	public class MsfMovieWrapper : IMovie
	{
		private IMovie _movie;

		public MsfMovieWrapper(IMovie movie)
		{
			_movie = movie ?? throw new ArgumentNullException(nameof(movie));
		}

		public string Abbreviation => _movie.Abbreviation;

		public bool AdjustEarnings
		{
			get { return _movie.AdjustEarnings; }
			set { _movie.AdjustEarnings = value; }
		}

		public IMovie Clone()
		{
			return _movie = _movie.Clone();
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

		public DayOfWeek? Day
		{
			get { return _movie.Day; }
			set { _movie.Day = value; }
		}

		public decimal Efficiency => _movie.Efficiency;

		public decimal Earnings
		{
			get { return _movie.Earnings; }
			set { _movie.Earnings = value; }
		}

		public decimal EarningsBase
		{
			get { return _movie.EarningsBase; }
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

		public bool IsBestPerformer
		{
			get { return _movie.IsBestPerformer; }
			set { _movie.IsBestPerformer = value; }
		}

		public string ImageUrl
		{
			get { return _movie.ImageUrl; }
			set { _movie.ImageUrl = value; }
		}

		public string MovieName
		{
			get { return _movie.MovieName; }
			set { _movie.MovieName = value; }
		}

		public string Name
		{
			get { return _movie.Name; }
			set { _movie.Name = value; }
		}

		public DateTime WeekendEnding
		{
			get { return _movie.WeekendEnding; }
			set { _movie.WeekendEnding = value; }
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}