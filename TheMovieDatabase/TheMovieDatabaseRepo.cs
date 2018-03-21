using RestSharp;
using System.Collections.Generic;
using TheMovieDatabase.Interfaces;

namespace TheMovieDatabase
{
	public class TheMovieDatabaseRepo : IMovieRepo
	{
		private string _apiKey;

		public TheMovieDatabaseRepo(string apiKey)
		{
			_apiKey = apiKey;
		}

		public IEnumerable<IMovie> Search(string title)
		{
			var client = new RestClient($"https://api.themoviedb.org/3/search/movie?primary_release_year=2018&include_adult=false&page=1&query=Black%20Panther&language=en-US&api_key={_apiKey}");
			var request = new RestRequest(Method.GET);

			request.AddParameter("undefined", "{}", ParameterType.RequestBody);

			IRestResponse response = client.Execute(request);

			return null;
		}
	}
}