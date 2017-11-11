using MoviePicker.WebApp.Interfaces;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private IMinerModel _minerModel;
		private IIndexViewModel _viewModel;

		public HomeController(IIndexViewModel viewModel, IMinerModel minerModel)
		{
			_minerModel = minerModel;
			_viewModel = viewModel;
			_viewModel.Miners = minerModel.Miners;
		}

		public ActionResult Index()
		{
			return View(_viewModel);
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