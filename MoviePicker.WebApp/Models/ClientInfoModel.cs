using MoviePicker.WebApp.Interfaces;
using System;

namespace MoviePicker.WebApp.Models
{
	public class ClientInfoModel : IClientInfoModel
	{
		public string Device { get; set; }

		public string Name { get; set; }
	}
}