using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System.Linq;
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

			_viewModel.NerdWeight = minerModel.Miners[0].Weight;
			_viewModel.ToddWeight = minerModel.Miners[1].Weight;
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

		[HttpGet]
		public ActionResult Picks()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Picks(IndexViewModel viewModel)
		{
			return RedirectToAction("Picks");
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

	}
}