using System;

namespace TheMovieDatabase.Interfaces
{
	public interface IMovie
	{
		string BackdropPath { get; set; }

		int Id { get; set; }

		string Overview { get; set; }

		string PosterPath { get; set; }

		DateTime ReleaseDate { get; set; }

		string Title { get; set; }
	}
}