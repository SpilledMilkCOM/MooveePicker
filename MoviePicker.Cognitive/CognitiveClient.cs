using SM.Common.REST;
using SM.Common.REST.Interfaces;
using System.Collections.Specialized;
using System.Net;

namespace MoviePicker.Cognitive
{
	public class CognitiveClient : IRestClient
	{
		private IRestClient _restClient;

		public CognitiveClient()
		{
			var restClient = new RestClient();

			restClient.AddHeaders = AddHeaders;

			_restClient = restClient;
		}

		public string APIKey { get => _restClient.APIKey; set => _restClient.APIKey = value; }

		public string BaseAddress { get => _restClient.BaseAddress; set => _restClient.BaseAddress = value; }

		public string ContentType { get => _restClient.ContentType; set => _restClient.ContentType = value; }

		public string EndPointMethod { get => _restClient.EndPointMethod; set => _restClient.EndPointMethod = value; }

		public NameValueCollection Headers => _restClient.Headers;

		//----==== PUBLIC ====--------------------------------------------------------------------

		public void AddParamter(string key, string value)
		{
			_restClient.AddParamter(key, value);
		}

		public string Get()
		{
			return _restClient.Get();
		}

		public string Post(string dataToPost)
		{
			return _restClient.Post(dataToPost);
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void AddHeaders(WebClient webClient)
		{
			if (ContentType != null)
			{
				webClient.Headers.Add("Content-Type", ContentType);
			}

			if (APIKey != null)
			{
				webClient.Headers.Add("Ocp-Apim-Subscription-Key", APIKey);
			}
		}
	}
}
