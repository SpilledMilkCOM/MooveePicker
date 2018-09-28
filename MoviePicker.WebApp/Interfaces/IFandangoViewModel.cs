namespace MoviePicker.WebApp.Interfaces
{
	public interface IFandangoViewModel
	{
		long Duration { get; set; }

		int PastHours { get; set; }

		void Load();
	}
}