﻿using MoviePicker.Cognitive.Parameters;
using System.Collections.Generic;

namespace MoviePicker.Cognitive
{
	public interface ICognitiveConfiguration
	{
		string APIKey { get; set; }

		List<Detail> Details { get; set; }

		List<VisualFeature> VisualFeatures { get; set; }
	}
}