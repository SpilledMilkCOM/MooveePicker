﻿using System.Net;
using System.Text;

namespace MoviePicker.Cognitive
{
	// TODO: Move this to a utility class so it can be reused.
	public class RestClient : IRestClient
	{
		private string _parameters;

		public RestClient()
		{
		}

		public string APIKey { get; set; }

		public string BaseAddress { get; set; }

		public string EndPointMethod { get; set; }

		public string Get()
		{
			string result = null;

			using (var webClient = new WebClient())
			{
				webClient.Headers.Add("X-Api-Key", APIKey);

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
				webClient.Headers.Add("Content-Type", "application/json");
				webClient.Headers.Add("Ocp-Apim-Subscription-Key", APIKey);

				var response = webClient.UploadData($"{EndPointMethod}{_parameters}", "POST", Encoding.Default.GetBytes(dataToPost));

				if (response != null)
				{
					result = Encoding.Default.GetString(response);
				}
			}

			return result;
		}

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
	}
}