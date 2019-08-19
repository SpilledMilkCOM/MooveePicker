using MoviePicker.Cognitive.Parameters;
using System.Collections.Generic;

namespace MoviePicker.Cognitive
{
	public interface IComputerVision
	{
		string Analyze(string fileName
					, List<VisualFeature> visualFeatures = null
					, List<Detail> details = null
					, Language language = Language.undefined);

		string Describe(string fileName);
	}
}