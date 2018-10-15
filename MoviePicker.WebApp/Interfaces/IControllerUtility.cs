using System;
using System.Collections.Generic;
using System.Web;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IControllerUtility
	{
		Guid GetRequestGuid(HttpRequestBase request, string key);

		List<decimal> GetRequestDecimalList(HttpRequestBase request, string key);

		int? GetRequestInt(HttpRequestBase request, string key);

		List<int> GetRequestIntList(HttpRequestBase request, string key);

		string GetRequestString(HttpRequestBase request, string key);
	}
}