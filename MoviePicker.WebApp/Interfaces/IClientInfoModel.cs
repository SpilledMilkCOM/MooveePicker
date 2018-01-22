using System;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IClientInfoModel
	{
		string Device { get; set; }

		string Name { get; set; }
	}
}