using MoviePicker.Cognitive.Parameters;
using System.Collections.Generic;

namespace MoviePicker.Cognitive
{
	public interface IComputerVision
	{
		string Analyze(string posterUrl
					, List<VisualFeature> visualFeatures = null
					, List<Detail> details = null
					, Language language = Language.undefined);

		string Describe(string posterUrl
					, int maxCandidates = 1
					, Language language = Language.undefined);

		object GetThumbnail(string posterUrl, int width, int height, bool smartCropping);
	}
}