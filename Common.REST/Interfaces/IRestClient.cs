using System.Collections.Specialized;
using System.Net;

namespace SM.Common.REST.Interfaces
{
	public interface IRestClient
	{
		string APIKey { get; set; }

		string BaseAddress { get; set; }

		string ContentType { get; set; }

		string EndPointMethod { get; set; }

		NameValueCollection Headers { get; }

		void AddParamter(string key, string value);

		string Get();

		string Post(string dataToPost);
	}
}