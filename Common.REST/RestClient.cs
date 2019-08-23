using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
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

		// MS recommends only having ONE instance per application.
		// Many of the methods are thread safe.
		private static HttpClient _client = new HttpClient();

		public RestClient()
		{
			ContentType = "application/json";

			Headers = new NameValueCollection();
		}

		// Inject the method versus overriding.
		public AddHeadersMethod AddHeaders
		{
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
			Headers.Clear();
			AddHeaders?.Invoke();

			_client.BaseAddress = new Uri(BaseAddress);

			foreach (var key in Headers.AllKeys)
			{
				_client.DefaultRequestHeaders.Add(key, Headers[key]);
			}

			var result = _client.GetStringAsync($"{EndpointMethod}{_parameters}");

			result.Wait();      // Basically turning this synchronous.

			return result.Result;
		}

		public string Post(string dataToPost)
		{
			Headers.Clear();
			AddHeaders?.Invoke();

			_client.BaseAddress = new Uri(BaseAddress);

			foreach (var key in Headers.AllKeys)
			{
				if (key != "Content-Type")
				{
					_client.DefaultRequestHeaders.Add(key, Headers[key]);
				}
			}

			//			var content = new ByteArrayContent(Encoding.Default.GetBytes(dataToPost));
			var content = new StringContent(dataToPost, Encoding.UTF8, ContentType);

			var responseResult = _client.PostAsync($"{EndpointMethod}{_parameters}", content).Result.Content.ReadAsStringAsync();

			responseResult.Wait();      // Basically turning this synchronous.

			return responseResult.Result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}