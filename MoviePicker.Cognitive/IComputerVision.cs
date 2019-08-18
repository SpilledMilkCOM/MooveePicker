namespace MoviePicker.Cognitive
{
	public interface IComputerVision
	{
		string Analyze(string fileName);

		string Describe(string fileName);
	}
}