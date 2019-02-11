using MovieMiner;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using MoviePicker.WebApp.Utilities;
using MoviePicker.WebApp.ViewModels;
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
			return View();
		}

		public ActionResult ClearFiles()
		{
			var webRootPath = Server.MapPath("~");
			var localFilePrefix = $"{webRootPath}{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}MoviePoster_";

			FileUtility.CleanupFiles(localFilePrefix);

			return RedirectToAction("Info");
		}

		public ActionResult Contact()
		{
			return View();
		}

		public ActionResult Images()
		{
			var viewModel = new ImagesViewModel();
			var webRootPath = Server.MapPath("~");
			var localFilePrefix = $"{webRootPath}images";
			var filter = Request.Params["filter"];
			var sortBy = Request.Params["sortBy"];
			var files = string.IsNullOrEmpty(filter) ? Directory.GetFiles(localFilePrefix) : Directory.GetFiles(localFilePrefix, filter);

			foreach (var filePath in files)
			{
				var fileModel = new FileModel { Name = Path.GetFileName(filePath) };
				var fileInfo = new FileInfo(filePath);

				fileModel.CreationDateUTC = fileInfo.CreationTimeUtc;
				fileModel.CreationDateUTC = fileInfo.CreationTimeUtc.AddHours(-6);		// Daylight savings
				fileModel.ImageUrl = $"/images/{fileModel.Name}";
				fileModel.SizeInBytes = fileInfo.Length;

				viewModel.Images.Add(fileModel);
			} 

			if (sortBy == "size")
			{
				viewModel.Images = viewModel.Images.OrderByDescending(item => item.SizeInBytes).ToList();
			}
			else if (sortBy == "date")
			{
				viewModel.Images = viewModel.Images.OrderByDescending(item => item.CreationDateUTC).ToList();
			}

			viewModel.NextCleanup = (int)FileUtility.NextCleanupDuration;

			return View(viewModel);
		}

		public ActionResult Info()
		{
			_infoViewModel.ServerInfo.ProcessBytes = System.Diagnostics.Process.GetCurrentProcess()?.WorkingSet64 ?? 0;

			_infoViewModel.ServerInfo.Now = DateTime.Now;
			_infoViewModel.ServerInfo.NowUtc = DateTime.UtcNow;

			var imagePath = $"{Server.MapPath("~")}{Path.DirectorySeparatorChar}images";

			_infoViewModel.ServerInfo.MoviePosterFiles = FileUtility.FilterImagesInPath(imagePath, "MoviePoster_*");
			_infoViewModel.ServerInfo.SharedFiles = FileUtility.FilterImagesInPath(imagePath, "Shared_*");
			_infoViewModel.ServerInfo.TwitterFiles = FileUtility.FilterImagesInPath(imagePath, "Twitter_*");

			_infoViewModel.ClientInfo.Device = Request.Browser.IsMobileDevice ? "Mobile" : "Desktop";
			_infoViewModel.ClientInfo.Name = Request.Browser.Type;


			_infoViewModel.ServerInfo.NextCleanup = (int)FileUtility.NextCleanupDuration;

			return View(_infoViewModel);
		}

		public ActionResult Miner(string name)
		{
			var found = _infoViewModel.MinerModel.Miners.FirstOrDefault(miner => miner.Name == name);

			return View(found);
		}

		public ActionResult ExpireMiner(string minerName)
		{
			var foundMiner = _infoViewModel.MinerModel.Miners.FirstOrDefault(miner => miner.Name == minerName) as ICache;

			foundMiner?.Expire();

			return RedirectToAction("Miner", new { name = minerName });
		}

		public ActionResult ExpireMiners()
		{
			_infoViewModel.MinerModel.Expire();

			return RedirectToAction("Info");
		}
	}
}