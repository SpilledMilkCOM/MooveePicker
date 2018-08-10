using System.Collections.Generic;

namespace MoviePicker.Repository.Interfaces
{
	public interface IBoxOfficeRepository
	{
		void UpdateBoxOfficeValues(List<IBoxOfficeValue> values);
	}
}