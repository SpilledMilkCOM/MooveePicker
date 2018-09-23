using System;

namespace MoviePicker.WebApp.Models
{
	public class FileModel
	{
		public DateTime CreationDateUTC { get; set; }

		public DateTime CreationDateMountainTZ { get; set; }

		public string ImageUrl { get; set; }

		public string Name { get; set; }

		public long SizeInBytes { get; set; }
	}
}