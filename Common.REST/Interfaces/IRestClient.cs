using System.Collections.Generic;
using System.Collections.Specialized;

namespace SM.Common.REST.Interfaces
{
	public interface IRestClient
	{
		RestClient.AddHeadersMethod AddHeaders { get; set; }

		string APIKey { get; set; }

		/// <summary>
		/// The base URL (domain)
		/// </summary>
		string BaseAddress { get; set; }

		string ContentType { get; set; }

		/// <summary>
		/// The API Endpoint method
		/// </summary>
		string EndpointMethod { get; set; }

		NameValueCollection Headers { get; }

		/// <summary>
		/// Adds a request parameter
		/// </summary>
		/// <param name="key">Parameter name</param>
		/// <param name="value">Parameter value</param>
		void AddParameter(string key, string value);

		void AddParameters(string key, IEnumerable<string> parameters);

		string Get();

		string Post(string dataToPost);
	}
}