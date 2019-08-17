using MoviePicker.Cognitive.Parameters;
using System.Collections.Generic;

namespace MoviePicker.Cognitive
{
	public interface ICognitiveConfiguration
	{
		string APIKey { get; set; }

		List<Details> Details { get; set; }

		List<VisualFeatures> VisualFeatures { get; set; }
	}
}