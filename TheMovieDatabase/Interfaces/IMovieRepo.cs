using System.Collections.Generic;

namespace TheMovieDatabase.Interfaces
{
	public interface IMovieRepo
	{
		IEnumerable<IMovie> Search(string title);
	}
}