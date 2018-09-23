using MoviePicker.WebApp.Models;
using System.Collections.Generic;

namespace MoviePicker.WebApp.ViewModels
{
	public class ImagesViewModel
	{
		public ImagesViewModel()
		{
			Images = new List<FileModel>();
		}

		public List<FileModel> Images { get; set; }
	}
}