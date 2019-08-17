using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

using SM.Common.REST.Interfaces;

namespace SM.Common.REST
{
	public class RestClient : IRestClient
	{
		private string _parameters;
		private AddHeadersMethod _addHeadersMethod;

		public delegate void AddHeadersMethod(WebClient webClient);

		public RestClient()
		{
			ContentType = "application/json";

			Headers = new NameValueCollection();
		}

		// Inject the method versus overriding.
		public AddHeadersMethod AddHeaders {
			get => _addHeadersMethod;
			set => _addHeadersMethod = (value == null) ? throw new ArgumentNullException("The AddHeaders method cannot be null.") : value;
		}

		public string APIKey { get; set; }

		public string BaseAddress { get; set; }

		public string ContentType { get; set; }

		public string EndPointMethod { get; set; }

		public NameValueCollection Headers { get; private set; }

		//----==== PUBLIC ====--------------------------------------------------------------------

		public void AddParamter(string key, string value)
		{
			if (_parameters == null)
			{
				_parameters = "?";
			}
			else
			{
				_parameters += "&";
			}

			_parameters += $"{key}={value}";
		}

		public string Get()
		{
			string result = null;

			using (var webClient = new WebClient())
			{
				// The content type may not be needed for this, but it's provided in the default headers.
				// TODO: May need to clean this up too.

				AddHeaders(webClient);

				result = webClient.DownloadString($"{EndPointMethod}{_parameters}");
			}

			return result;
		}

		public string Post(string dataToPost)
		{
			string result = null;

			using (var webClient = new WebClient())
			{
				webClient.BaseAddress = BaseAddress;

				AddHeaders(webClient);

				var response = webClient.UploadData($"{EndPointMethod}{_parameters}", "POST", Encoding.Default.GetBytes(dataToPost));

				if (response != null)
				{
					result = Encoding.Default.GetString(response);
				}
			}

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}