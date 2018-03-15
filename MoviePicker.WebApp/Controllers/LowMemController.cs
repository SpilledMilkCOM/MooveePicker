using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	/// <summary>
	/// This controller serves up pages that don't need the data mining.
	/// </summary>
	public class LowMemController : Controller
	{
		private readonly IInfoViewModel _infoViewModel;

		public LowMemController(IInfoViewModel infoViewModel)
		{
			// Just some injection and some assignments.

			_infoViewModel = infoViewModel;

			ControllerUtility.SetTwitterCard(ViewBag);
		}

		public ActionResult About()
		{
			ViewBag.IsGoogleAdValid = true;

			return View();
		}

		public ActionResult ClearFiles()
		{
			var webRootPath = Server.MapPath("~");
			var localFilePrefix = $"{webRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}MoviePoster_";

			ViewBag.IsGoogleAdValid = false;

			FileUtility.CleanupFiles(localFilePrefix);

			return RedirectToAction("Info");
		}

		public ActionResult Contact()
		{
			ViewBag.IsGoogleAdValid = false;

			return View();
		}

		public ActionResult Info()
		{
			ViewBag.IsGoogleAdValid = false;

			_infoViewModel.ServerInfo.Now = DateTime.Now;
			_infoViewModel.ServerInfo.NowUtc = DateTime.UtcNow;

			var imagePath = $"{Server.MapPath("~")}{Path.DirectorySeparatorChar}images";

			_infoViewModel.ServerInfo.MoviePosterFileCount = FileUtility.FilterImagesInPath(imagePath, "MoviePoster_*")?.Count ?? 0;
			_infoViewModel.ServerInfo.SharedFileCount = FileUtility.FilterImagesInPath(imagePath, "Shared_*")?.Count ?? 0;
			_infoViewModel.ServerInfo.TwitterFileCount = FileUtility.FilterImagesInPath(imagePath, "Twitter_*")?.Count ?? 0;

			_infoViewModel.ClientInfo.Device = Request.Browser.IsMobileDevice ? "Mobile" : "Desktop";
			_infoViewModel.ClientInfo.Name = Request.Browser.Type;

			return View(_infoViewModel);
		}

		public ActionResult Miner(string name)
		{
			var found = _infoViewModel.MinerModel.Miners.FirstOrDefault(miner => miner.Name == name);

			return View(found);
		}

		public ActionResult ExpireMiners()
		{
			_infoViewModel.MinerModel.Expire();

			return RedirectToAction("Info");
		}
	}
}