using MoviePicker.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviePicker.Repository.Models
{
	[Table("BoxOfficeSource")]
	public class BoxOfficeSource : IBoxOfficeSource
	{
		[Key]
		[Required]          // NOT NULL
		public int Id { get; set; }

		[Required]          // NOT NULL
		public string Name { get; set; }

		/// <summary>
		/// A calculated value based on estimates and actuals
		/// </summary>
		[NotMapped]
		public int Weight { get; set; }
	}
}