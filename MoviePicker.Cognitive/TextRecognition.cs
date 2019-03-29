namespace MoviePicker.Cognitive
{
	public class TextRecognition : ITextRecognition
	{
		private ICognitiveConfiguration _configuration;
		private IRestClient _restClient;

		public TextRecognition(ICognitiveConfiguration configuration, IRestClient restClient)
		{
			_configuration = configuration;
			_restClient = restClient;

			// https://[location].api.cognitive.microsoft.com/vision/v2.0/ocr[?language][&detectOrientation] 

			_restClient.BaseAddress = "https://southcentralus.api.cognitive.microsoft.com";
			_restClient.EndPointMethod = "/vision/v2.0/ocr?language=en";
		}

		public string AnalyzeText(string posterUrl)
		{
			string result = null;

			result = _restClient.Post($"{{\"url\":\"{posterUrl}\"}}");

			return result;
		}
	}
}