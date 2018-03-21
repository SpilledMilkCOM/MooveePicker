using System.Collections.Generic;

namespace TheMovieDatabase.Interfaces
{
	public interface ISearchResults
	{
		int Page { get; set; }

		int TotalResults { get; set; }

		int TotalPages { get; set; }

		List<IMovie> Results { get; set; }
	}
}