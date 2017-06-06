using System.Collections.Generic;

namespace MooveePicker
{
	public interface IMovieList
	{
		IEnumerable<IMovie> Movies { get; }

		decimal TotalCost { get; }

		decimal TotalEarnings { get; }

		void Add(IMovie movie);

		void Add(IEnumerable<IMovie> movies);

		bool CanAdd(IMovie movie);

		void Clear();

		IMovieList Clone();

		void Remove(IMovie movie);
	}
}