using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private IMinerModel _minerModel;
		private IIndexViewModel _viewModel;
		private IMoviePicker _moviePicker;

		public HomeController(IIndexViewModel viewModel, IMinerModel minerModel, IMoviePicker moviePicker)
		{
			_minerModel = minerModel;
			_moviePicker = moviePicker;
			_viewModel = viewModel;
			_viewModel.Miners = minerModel.Miners;

			_viewModel.Weight1 = minerModel.Miners[0].Weight;
			_viewModel.Weight2 = minerModel.Miners[1].Weight;
			_viewModel.Weight3 = minerModel.Miners[2].Weight;
			_viewModel.Weight4 = minerModel.Miners[3].Weight;
			_viewModel.Weight5 = minerModel.Miners[4].Weight;
			_viewModel.Weight6 = minerModel.Miners[5].Weight;
			_viewModel.Weight7 = minerModel.Miners[6].Weight;
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

		public ActionResult Simuation()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		[HttpGet]
		public FileStreamResult ExtractToCSV()
		{
			StringBuilder builder = new StringBuilder();

			// Column headers are FIRST!

			builder.Append("BUX,Movie");

			foreach (var miner in _minerModel.Miners)
			{
				builder.Append(",");
				builder.Append(miner.Abbreviation);
			}

			builder.AppendLine();

			foreach (var movie in _minerModel.Miners.First().Movies)
			{
				builder.Append(movie.Cost);
				builder.Append(",");
				builder.Append(movie.Name);

				foreach (var miner in _minerModel.Miners)
				{
					var minerMovie = miner.Movies.FirstOrDefault(item => item.Name == movie.Name);

					builder.Append(",");
					builder.Append(minerMovie?.Earnings);
				}

				builder.AppendLine();
			}

			var byteArray = Encoding.ASCII.GetBytes(builder.ToString());
			var stream = new MemoryStream(byteArray);

			return File(stream, "text/plain", "MyStuffCSV.txt");
		}

		[HttpGet]
		public ActionResult Picks()
		{
			return View(ConstructPicksViewModel());
		}

		[HttpPost]
		public ActionResult Picks(IndexViewModel viewModel)
		{
			// Transfer the posted data to the actual ViewModel

			_minerModel.Miners[0].Weight = viewModel.Weight1;
			_minerModel.Miners[1].Weight = viewModel.Weight2;
			_minerModel.Miners[2].Weight = viewModel.Weight3;
			_minerModel.Miners[3].Weight = viewModel.Weight4;
			_minerModel.Miners[4].Weight = viewModel.Weight5;
			_minerModel.Miners[5].Weight = viewModel.Weight6;
			_minerModel.Miners[6].Weight = viewModel.Weight7;

			//return RedirectToAction("Picks");
			return View(ConstructPicksViewModel());
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private PicksViewModel ConstructPicksViewModel()
		{
			var result = new PicksViewModel();

			result.Miners = _minerModel.Miners;
			result.Movies = _minerModel.CreateWeightedList();

			_moviePicker.AddMovies(result.Movies);

			result.MovieList = _moviePicker.ChooseBest();

			// Need to clone the list otherwise the above MovieList will lose its BestPerformer.

			var clonedList = new List<IMovie>();

			foreach (var movie in result.Movies)
			{
				clonedList.Add(movie.Clone());
			}

			_moviePicker.AddMovies(clonedList);

			_moviePicker.EnableBestPerformer = false;

			result.MovieListBonusOff = _moviePicker.ChooseBest();

			return result;
		}
	}
}