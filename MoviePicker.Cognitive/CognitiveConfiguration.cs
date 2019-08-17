using System.Collections.Generic;
using MoviePicker.Cognitive.Parameters;

namespace MoviePicker.Cognitive
{
	public class CognitiveConfiguration : ICognitiveConfiguration
	{
		public string APIKey { get; set; }

		public List<Details> Details { get; set; }

		public List<VisualFeatures> VisualFeatures { get; set; }
	}
}