using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

using SM.Common.REST.Interfaces;

namespace SM.Common.REST
{
	/// <summary>
	/// A generic class that 
	/// </summary>
	public class RestClient : IRestClient
	{
		public const string HEADER_CONTENT_TYPE = "Content-Type";

		public delegate void AddHeadersMethod();

		private string _parameters;
		private AddHeadersMethod _addHeadersMethod;

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

		public string EndpointMethod { get; set; }

		public NameValueCollection Headers { get; private set; }

		//----==== PUBLIC ====--------------------------------------------------------------------

		public void AddParameter(string key, string value)
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

		/// <summary>
		/// Adds a comma separated list to the parameter.
		/// </summary>
		/// <param name="key">The parameter key</param>
		/// <param name="parameters">A list of objects whose ToString() value will be used.</param>
		public void AddParameters(string key, IEnumerable<string> parameters)
		{
			if (_parameters == null)
			{
				_parameters = "?";
			}
			else
			{
				_parameters += "&";
			}

			if (parameters != null)
			{
				if (parameters.Any())
				{
					_parameters += $"{key}={parameters.First()}";
				}

				foreach (var parameter in parameters.Skip(1))
				{
					_parameters += $",{parameter}";
				}
			}
		}

		public string Get()
		{
			string result = null;

			using (var webClient = new WebClient())
			{
				webClient.BaseAddress = BaseAddress;

				// The content type may not be needed for this, but it's provided in the default headers.
				// TODO: May need to clean this up too.

				AddHeaders?.Invoke();

				webClient.Headers.Add(Headers);

				result = webClient.DownloadString($"{EndpointMethod}{_parameters}");
			}

			return result;
		}

		public string Post(string dataToPost)
		{
			string result = null;

			using (var webClient = new WebClient())
			{
				webClient.BaseAddress = BaseAddress;

				AddHeaders?.Invoke();

				webClient.Headers.Add(Headers);

				var response = webClient.UploadData($"{EndpointMethod}{_parameters}", "POST", Encoding.Default.GetBytes(dataToPost));

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