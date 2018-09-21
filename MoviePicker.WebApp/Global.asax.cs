using System;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MoviePicker.WebApp
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		protected void Application_Error(Object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();

			if (ex is ThreadAbortException)
			{
				return;
			}

			var helper = new UrlHelper(Request.RequestContext, RouteTable.Routes);

			var trimIndex = ex.StackTrace.IndexOf('\r');

			if (trimIndex < 0)
			{
				trimIndex = 100;
			}

			var abbreviatedStackTrace = ex.StackTrace.Substring(0, trimIndex);

			string url = helper.Action("Error", "Home", routeValues: new { message = $"{ex.Message} -- {abbreviatedStackTrace}" });

			Response.Redirect(url);
		}
	}
}