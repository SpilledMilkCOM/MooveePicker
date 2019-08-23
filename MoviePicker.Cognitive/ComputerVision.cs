using MoviePicker.Cognitive.Parameters;
using SM.Common.REST;
using SM.Common.REST.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoviePicker.Cognitive
{
	/// <summary>
	/// A class that wraps some of the Computer Vision API in the Microsoft Cognitive Services
	/// https://westus.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fa
	/// </summary>
	public class ComputerVision : IComputerVision
	{
		private const string BASE_METHOD = "vision";
		private const string API_VERSION = "V2.0";
		private const string HEADER_KEY = "Ocp-Apim-Subscription-Key";

		private ICognitiveConfiguration _configuration;
		private IRestClient _restClient;

		public ComputerVision(ICognitiveConfiguration configuration, IRestClient restClient)
		{
			_configuration = configuration;
			_restClient = restClient;
			_restClient.AddHeaders = AddHeaders;
			_restClient.BaseAddress = "https://southcentralus.api.cognitive.microsoft.com";
		}

		public string Analyze(string posterUrl, List<VisualFeature> visualFeatures = null, List<Detail> details = null, Language language = Language.undefined)
		{
			// https://[location].api.cognitive.microsoft.com/vision/v1.0/analyze[?visualFeatures][&details][&language] 

			_restClient.EndpointMethod = $"/{BASE_METHOD}/{API_VERSION}/analyze";

			_restClient.AddParameters("visualFeatures", visualFeatures?.Select(item => item.ToString()));
			_restClient.AddParameters("details", details?.Select(item => item.ToString()));

			if (language != Language.undefined)
			{
				_restClient.AddParameter("language", language.ToString());
			}

			return _restClient.Post($"{{\"url\":\"{posterUrl}\"}}");
		}

		public string Describe(string posterUrl, int maxCandidates = 1, Language language = Language.undefined)
		{
			// https://[location].api.cognitive.microsoft.com/vision/v2.0/describe[?maxCandidates][&language]

			_restClient.EndpointMethod = $"/{BASE_METHOD}/{API_VERSION}/describe";

			if (maxCandidates != 1)		// Since 1 is the default.
			{
				_restClient.AddParameter("maxCandidates", maxCandidates.ToString());
			}

			if (language != Language.undefined)
			{
				_restClient.AddParameter("language", language.ToString());
			}

			return _restClient.Post($"{{\"url\":\"{posterUrl}\"}}");
		}

		public object GetThumbnail(string posterUrl, int width, int height, bool smartCropping)
		{
			_restClient.EndpointMethod = $"/{BASE_METHOD}/{API_VERSION}/getthumbnail";

			_restClient.AddParameter("width", width.ToString());
			_restClient.AddParameter("height", height.ToString());
			_restClient.AddParameter("smartCropping", smartCropping.ToString());

			return _restClient.Post($"{{\"url\":\"{posterUrl}\"}}");
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AddHeaders()
		{
			// TODO: Might put this in a common RestClient method.
			if (_restClient.ContentType != null)
			{
				_restClient.Headers.Add(RestClient.HEADER_CONTENT_TYPE, _restClient.ContentType);
			}

			if (_restClient.APIKey != null)
			{
				_restClient.Headers.Add(HEADER_KEY, _restClient.APIKey);
			}
		}
	}
}