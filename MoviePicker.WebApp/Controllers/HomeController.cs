using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MoviePicker.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private const int MY_MINER_IDX = 1;

		private IMinerModel _minerModel;
		private IIndexViewModel _viewModel;
		private IMoviePicker _moviePicker;
		private ISimulationModel _simulationModel;

		public HomeController(IIndexViewModel viewModel, IMinerModel minerModel, IMoviePicker moviePicker, ISimulationModel simulationModel)
		{
			_minerModel = minerModel;
			_moviePicker = moviePicker;
			_simulationModel = simulationModel;
			_viewModel = viewModel;
			_viewModel.Miners = minerModel.Miners;

			// TODO: Use reflection...

			int index = 1;

			_viewModel.Weight1 = minerModel.Miners[index++].Weight;
			_viewModel.Weight2 = minerModel.Miners[index++].Weight;
			_viewModel.Weight3 = minerModel.Miners[index++].Weight;
			_viewModel.Weight4 = minerModel.Miners[index++].Weight;
			_viewModel.Weight5 = minerModel.Miners[index++].Weight;
			_viewModel.Weight6 = minerModel.Miners[index++].Weight;
			_viewModel.Weight7 = minerModel.Miners[index++].Weight;

			var myBoxOffice = minerModel.Miners[1].Movies;

			index = 0;

			_viewModel.BoxOffice1 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice2 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice3 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice4 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice5 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice6 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice7 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice8 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice9 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice10 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice11 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice12 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice13 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice14 = myBoxOffice[index++].EarningsBase;
			_viewModel.BoxOffice15 = myBoxOffice[index++].EarningsBase;
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
			int index = 1;

			// Transfer the posted data to the actual ViewModel
			// TODO: Use reflection...

			_minerModel.Miners[index++].Weight = viewModel.Weight1;
			_minerModel.Miners[index++].Weight = viewModel.Weight2;
			_minerModel.Miners[index++].Weight = viewModel.Weight3;
			_minerModel.Miners[index++].Weight = viewModel.Weight4;
			_minerModel.Miners[index++].Weight = viewModel.Weight5;
			_minerModel.Miners[index++].Weight = viewModel.Weight6;
			_minerModel.Miners[index++].Weight = viewModel.Weight7;

			index = 0;

			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice1;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice2;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice3;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice4;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice5;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice6;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice7;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice8;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice9;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice10;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice11;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice12;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice13;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice14;
			_minerModel.Miners[MY_MINER_IDX].Movies[index++].Earnings = viewModel.BoxOffice15;

			//return RedirectToAction("Picks");
			return View(ConstructPicksViewModel());
		}

		public ActionResult Simulation()
		{
			var picksViewModel = ConstructPicksViewModel();

			RunSimulation(picksViewModel);

			return View(picksViewModel);
		}

		public ActionResult WeekBack()
		{
			return View();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private PicksViewModel ConstructPicksViewModel()
		{
			var result = new PicksViewModel();
			var stopWatch = new Stopwatch();

			stopWatch.Start();

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

			stopWatch.Stop();

			result.Duration = stopWatch.ElapsedMilliseconds;

			return result;
		}

		private void RunSimulation(PicksViewModel picksViewModel)
		{
			var stopwatch = new Stopwatch();

			// Add picked movies.
			var movies = picksViewModel.MovieList.Movies.Distinct().ToList();

			// Add most efficient.

			foreach (var movie in picksViewModel.Movies.OrderByDescending(item => item.Efficiency))
			{
				// Can't use Contains
				if (movies.FirstOrDefault(item => item.Name == movie.Name) == null)
				{
					movies.Add(movie);

					if (movies.Count >= 8)
					{
						break;
					}
				}
			}

			_simulationModel.AddMovies(movies);

			picksViewModel.MovieList = _simulationModel.ChooseBest();

			picksViewModel.Duration += stopwatch.ElapsedMilliseconds;
		}
	}
}