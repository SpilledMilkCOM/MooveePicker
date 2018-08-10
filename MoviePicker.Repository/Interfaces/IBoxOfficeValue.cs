using System;

namespace MoviePicker.Repository.Interfaces
{
	public interface IBoxOfficeValue
	{
		DateTime Created { get; set; }

		/// <summary>
		/// The End date of the box office value.
		/// </summary>
		DateTime End { get; set; }

		int Id { get; set; }

		/// <summary>
		/// Whether or not the value is an estimate or actual value.
		/// </summary>
		bool IsActual { get; set; }

		/// <summary>
		/// The source of the box office value.
		/// </summary>
		IBoxOfficeSource Source { get; set; }

		/// <summary>
		/// The Start date of the box office value.
		/// </summary>
		DateTime Start { get; set; }

		/// <summary>
		/// The value of the box office estimate or actual.
		/// </summary>
		decimal Value { get; set; }
    }
}