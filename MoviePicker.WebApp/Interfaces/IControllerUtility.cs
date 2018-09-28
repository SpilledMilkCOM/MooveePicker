using System.Collections.Generic;
using System.Web;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IControllerUtility
	{
		int? GetRequestInt(HttpRequestBase request, string key);

		List<decimal> GetRequestDecimalList(HttpRequestBase request, string key);

		List<int> GetRequestIntList(HttpRequestBase request, string key);
	}
}