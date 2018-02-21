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

			//Logger.Error(LoggerType.Global, ex, "Exception");

			var helper = new UrlHelper();
			string url = helper.Action("error", "home", new { message = ex.Message });

			Response.Redirect(url);
		}
	}
}