using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	/// <summary>
	/// This controller serves up pages that don't need the data mining.
	/// </summary>
	public class AboutController : Controller
	{

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}
	}
}