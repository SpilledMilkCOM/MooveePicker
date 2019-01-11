﻿namespace MoviePicker.Cognitive
{
	public class PosterRecognition : IPosterRecognition
	{
		private ICognitiveConfiguration _configuration;
		private IRestClient _restClient;

		public PosterRecognition(ICognitiveConfiguration configuration, IRestClient restClient)
		{
			_configuration = configuration;
			_restClient = restClient;

			// https://[location].api.cognitive.microsoft.com/vision/v1.0/analyze[?visualFeatures][&details][&language] 

			_restClient.BaseAddress = "https://southcentralus.api.cognitive.microsoft.com";
			_restClient.EndPointMethod = "/vision/v1.0/analyze";
		}

		public string AnalyzePoster(string posterUrl)
		{
			string result = null;

			result = _restClient.Post($"{{\"url\":\"{posterUrl}\"}}");

			return result;
		}
	}
}