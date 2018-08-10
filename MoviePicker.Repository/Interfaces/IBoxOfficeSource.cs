namespace MoviePicker.Repository.Interfaces
{
	public interface IBoxOfficeSource
    {
		int Id { get; set; }

		string Name { get; set; }

		int Weight { get; set; }
	}
}