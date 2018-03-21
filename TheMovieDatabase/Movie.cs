using System;
using TheMovieDatabase.Interfaces;

namespace TheMovieDatabase
{
	public class Movie : IMovie
	{
		public string BackdropPath { get; set; }

		public int Id { get; set; }

		public string Overview { get; set; }

		public string PosterPath { get; set; }

		public DateTime ReleaseDate { get; set; }

		public string Title { get; set; }
	}
}