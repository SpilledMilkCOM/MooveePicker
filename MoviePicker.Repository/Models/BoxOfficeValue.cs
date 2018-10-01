using MoviePicker.Repository.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviePicker.Repository.Models
{
	[Table("BoxOfficeValue")]
	public class BoxOfficeValue : IBoxOfficeValue
	{
		[Required]          // NOT NULL
		public DateTime Created { get; set; }

		[Required]          // NOT NULL
		public DateTime End { get; set; }

		[Key]
		[Required]          // NOT NULL
		public int Id { get; set; }

		[Required]          // NOT NULL
		public bool IsActual { get; set; }

		[Required]          // NOT NULL
		public int MovieId { get; set; }

		[Required]          // NOT NULL
		public DateTime Start { get; set; }

		[Required]          // NOT NULL
		public decimal Value { get; set; }

		//----==== Not part of the DB schema ====--------------------------------------------------------------------------

		[NotMapped]
		public IBoxOfficeSource Source { get; set; }
	}
}