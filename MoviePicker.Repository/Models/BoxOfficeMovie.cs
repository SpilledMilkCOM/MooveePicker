using MoviePicker.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviePicker.Repository.Models
{
	[Table("BoxOfficeMovie")]
	public class BoxOfficeMovie : IBoxOfficeMovie
	{
		[Required]          // NOT NULL
		public int Cost { get; set; }

		[Key]
		[Required]          // NOT NULL
		public int Id { get; set; }

		[Required]          // NOT NULL
		public string Name { get; set; }


		[Required]          // NOT NULL
		public string ImageUrl { get; set; }
	}
}