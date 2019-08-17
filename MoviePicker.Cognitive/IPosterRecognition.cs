namespace MoviePicker.Cognitive
{
	public interface IPosterRecognition
	{
		string AnalyzePoster(string fileName);

		string DescribePoster(string fileName);
	}
}