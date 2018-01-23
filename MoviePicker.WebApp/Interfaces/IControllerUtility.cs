using System.Collections.Generic;
using System.Web;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IControllerUtility
	{
		List<int> GetRequestIntList(HttpRequestBase request, string key);
	}
}