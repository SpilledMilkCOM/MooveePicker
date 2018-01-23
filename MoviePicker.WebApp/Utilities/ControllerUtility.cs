using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;
using System.Web;

namespace MoviePicker.WebApp.Utilities
{
	public class ControllerUtility : IControllerUtility
	{
		public List<int> GetRequestIntList(HttpRequestBase request, string key)
		{
			char[] listDelimiter = { ',' };
			var result = new List<int>();
			var parms = request.Params;

			if (parms[key] != null)
			{
				var paramList = parms[key].Split(listDelimiter);

				foreach (var paramValue in paramList)
				{
					int value = 0;

					if (int.TryParse(paramValue, out value))
					{
						result.Add(value);
					}
				}
			}

			return result;
		}
	}
}