using MoviePicker.WebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.WebApp.ViewModels
{
	public class ImagesViewModel
	{
		public ImagesViewModel()
		{
			Images = new List<FileModel>();
		}

		public List<FileModel> Images { get; set; }

		public long TotalSize => Images.Sum(image => image.SizeInBytes);
	}
}