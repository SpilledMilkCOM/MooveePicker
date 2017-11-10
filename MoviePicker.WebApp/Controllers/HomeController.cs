using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private IMinerModel _minerModel;
		private IHomeViewModel _viewModel;

		public HomeController(IHomeViewModel viewModel, IMinerModel minerModel)
		{
			_minerModel = minerModel;
			_viewModel = viewModel;
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

	}
}