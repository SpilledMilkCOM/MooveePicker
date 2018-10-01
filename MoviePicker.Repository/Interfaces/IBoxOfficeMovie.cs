namespace MoviePicker.Repository.Interfaces
{
	public interface IBoxOfficeMovie
    {
		int Cost { get; set; }

		int Id { get; set; }

		string ImageUrl { get; set; }

		string Name { get; set; }
	}
}