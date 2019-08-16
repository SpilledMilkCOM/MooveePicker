namespace SM.Common.REST.Interfaces
{
	public interface IRestClient
	{
		string APIKey { get; set; }

		string BaseAddress { get; set; }

		string EndPointMethod { get; set; }

		void AddParamter(string key, string value);

		string Get();

		string Post(string dataToPost);
	}
}