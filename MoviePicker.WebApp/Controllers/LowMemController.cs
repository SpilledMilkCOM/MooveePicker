using MoviePicker.WebApp.Interfaces;
using System;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	/// <summary>
	/// This controller serves up pages that don't need the data mining.
	/// </summary>
	public class LowMemController : Controller
	{
		private readonly IClientInfoModel _clientInfoModel;
		private readonly IInfoViewModel _infoViewModel;
		private readonly IServerInfoModel _serverInfoModel;

		public LowMemController(IClientInfoModel clientInfo, IInfoViewModel infoViewModel, IServerInfoModel serverInfo)
		{
			// Just some injection and some assignments.

			_clientInfoModel = clientInfo;
			_infoViewModel = infoViewModel;
			_serverInfoModel = serverInfo;

			_infoViewModel.ClientInfo = _clientInfoModel;
			_infoViewModel.ServerInfo = _serverInfoModel;
		}

		public ActionResult About()
		{
			ViewBag.IsGoogleAdValid = true;

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.IsGoogleAdValid = false;

			return View();
		}

		public ActionResult Info()
		{
			ViewBag.IsGoogleAdValid = false;

			_serverInfoModel.Now = DateTime.Now;
			_serverInfoModel.NowUtc = DateTime.UtcNow;

			_clientInfoModel.Device = Request.Browser.IsMobileDevice ? "Mobile" : "Desktop";
			_clientInfoModel.Name = Request.Browser.Type;

			return View(_infoViewModel);
		}
	}
}