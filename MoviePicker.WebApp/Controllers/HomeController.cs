using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using MoviePicker.WebApp.Utilities;
using MoviePicker.WebApp.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;

using TopMoviePicker = MooveePicker.MoviePicker;

namespace MoviePicker.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private const int FML_INDEX = 0;
		private const int MY_MINER_IDX = FML_INDEX + 1;

		private IControllerUtility _controllerUtility;
		private IMinerModel _minerModel;
		private IMinerModel _minerModelCache;       // Access to the singleton so it can be cleared/expired
		private IIndexViewModel _viewModel;
		private IMoviePicker _moviePicker;
		private ISimulationModel _simulationModel;

		/// <summary>
		/// </summary>
		/// <param name="viewModel">A new view model per thread</param>
		/// <param name="minerModel">A singleton minerModel</param>
		/// <param name="moviePicker"></param>
		/// <param name="simulationModel"></param>
		public HomeController(IIndexViewModel viewModel
							, IMinerModel minerModel
							, IMoviePicker moviePicker
							, ISimulationModel simulationModel
							, IControllerUtility controllerUtility)
		{
			// expecting a singleton as the miner model.
			_minerModelCache = minerModel;
			_minerModel = minerModel.Clone();

			// TODO: Possibly use LazyLoad<> on some of these injected objects.

			_controllerUtility = controllerUtility;
			_moviePicker = moviePicker;
			_simulationModel = simulationModel;
			_viewModel = viewModel;
			_viewModel.Miners = _minerModel.Miners;

			UpdateViewModel();
		}

		public ActionResult Error(string message)
		{
			return View(new ErrorViewModel { MainMessage = message ?? "An unknown error occurred." });
		}

		[HttpGet]
		public ActionResult Expire()
		{
			_minerModelCache.Expire();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Index()
		{
			var stopWatch = new Stopwatch();

			stopWatch.Start();

			_viewModel.ViewGridOpen = !Request.Browser.IsMobileDevice;
			_viewModel.ViewMobileOpen = Request.Browser.IsMobileDevice;

			ViewBag.IsGoogleAdValid = true;

			ParseBoxOfficeWeightRequest();
			ParseViewRequest();

			_viewModel.IsTracking = _minerModel.Miners[FML_INDEX].ContainsEstimates;
			//_viewModel.IsTracking = true;

			ControllerUtility.SetTwitterCard(ViewBag);

			DownloadMoviePosters();

			stopWatch.Stop();

			_viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(_viewModel);
		}

		[HttpGet]
		public ActionResult Index2()
		{
			// TODO: Collapse this down to a method call.

			var stopWatch = new Stopwatch();

			stopWatch.Start();

			_viewModel.ViewGridOpen = !Request.Browser.IsMobileDevice;
			_viewModel.ViewMobileOpen = Request.Browser.IsMobileDevice;

			ViewBag.IsGoogleAdValid = true;

			ParseBoxOfficeWeightRequest();
			ParseViewRequest();

			_viewModel.IsTracking = _minerModel.Miners[FML_INDEX].ContainsEstimates;
			//_viewModel.IsTracking = true;

			ControllerUtility.SetTwitterCard(ViewBag);

			DownloadMoviePosters();

			stopWatch.Stop();

			_viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(_viewModel);
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

			return File(stream, "text/plain", "MooveePickerData.txt");
		}

		[HttpGet]
		public ActionResult MorePicks()
		{
			ViewBag.IsGoogleAdValid = true;

			ParseBoxOfficeWeightRequest();

			ControllerUtility.SetTwitterCard(ViewBag);

			DownloadMoviePosters();

			return View(ConstructMorePicksViewModel());
		}

		[HttpGet]
		public ActionResult Picks()
		{
			ViewBag.IsGoogleAdValid = true;

			ParseBoxOfficeWeightRequest();

			// Hide the last miner (BO Mojo for previous week).

			_minerModel.Miners.Last().IsHidden = true;

			ControllerUtility.SetTwitterCard(ViewBag);

			return View(ConstructPicksViewModel());
		}

		[HttpGet]
		public ActionResult ShareBonusOffPicks()
		{
			return View("SharePicks", ConstructSharePicksViewModel(false));
		}

		[HttpGet]
		public ActionResult ShareBonusOnPicks()
		{
			return View("SharePicks", ConstructSharePicksViewModel(true));
		}

		public ActionResult Simulation()
		{
			var picksViewModel = ConstructPicksViewModel();

			RunSimulation(picksViewModel);

			return View(picksViewModel);
		}

		[HttpGet]
		public ActionResult Tracking()
		{
			var stopWatch = new Stopwatch();

			stopWatch.Start();

			ViewBag.IsGoogleAdValid = true;

			ParseBoxOfficeWeightRequest();

			var viewModel = ConstructPicksViewModel();

			// Assign perfect pick to movie list so it is shared.
			viewModel.MovieList = viewModel.MovieListPerfectPick;

			var sharedViewModel = ConstructSharePicksViewModel(true, viewModel);

			// Show the values for the FML Estimate data.

			_minerModel.Miners.First().IsHidden = false;

			// Hide the last miner (BO Mojo for previous week).

			_minerModel.Miners.Last().IsHidden = true;

			//ControllerUtility.SetTwitterCard(ViewBag);

			ControllerUtility.SetTwitterCard(ViewBag, "summary_large_image", null
								, "Tracking my picks against the perfect pick to see where I went right or horribly wrong."
								, $"{Constants.WEBSITE_URL}/images/{sharedViewModel.TwitterImageFileName}"
								, "Collage of the perfect pick lineup."
								, "Check out my picks against the perfect pick.");

			//DownloadMoviePosters();

			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		public ActionResult WeekBack()
		{
			return RedirectToAction("Index");
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private MorePicksViewModel ConstructMorePicksViewModel()
		{
			var result = new MorePicksViewModel { MorePicks = new List<IMovieListModel>(), MorePicksBonusOff = new List<IMovieListModel>() };
			var stopWatch = new Stopwatch();

			stopWatch.Start();

			var movies = _minerModel.CreateWeightedList();
			var moviePicker = new TopMoviePicker(new MovieList());
			var moviePickerBonusOff = new TopMoviePicker(new MovieList());

			moviePicker.AddMovies(movies);

			var bonusThread = new Thread(new ThreadStart(() =>
			{
				foreach (var movieList in moviePicker.ChooseBest(10))
				{
					result.MorePicks.Add(new MovieListModel { Picks = new List<IMovieList> { movieList } });
				}
			}))
			{ Name = "bonusThread" };

			var bonusOffThread = new Thread(new ThreadStart(() =>
			{
				foreach (var movieList in moviePickerBonusOff.ChooseBest(10))
				{
					result.MorePicksBonusOff.Add(new MovieListModel { Picks = new List<IMovieList> { movieList } });
				}
			}))
			{ Name = "bonusOffThread" };

			// Need to clone the list otherwise the above MovieList will lose its BestPerformer.

			var clonedList = new List<IMovie>();

			foreach (var movie in movies)
			{
				clonedList.Add(movie.Clone());
			}

			moviePickerBonusOff.AddMovies(clonedList);
			moviePickerBonusOff.EnableBestPerformer = false;

			// Start both.

			bonusThread.Start();
			bonusOffThread.Start();

			// Wait for both to finish...

			bonusThread.Join();
			bonusOffThread.Join();

			stopWatch.Stop();

			result.Duration = stopWatch.ElapsedMilliseconds;

			return result;
		}

		private PicksViewModel ConstructPicksViewModel(bool onlyBestPerformer = false)
		{
			var result = new PicksViewModel();
			var stopWatch = new Stopwatch();

			stopWatch.Start();

			result.IsTracking = _minerModel.Miners[FML_INDEX].ContainsEstimates;

			result.Miners = _minerModel.Miners;
			result.Movies = _minerModel.CreateWeightedList();

			_moviePicker.AddMovies(result.Movies);

			var pickList = _moviePicker.ChooseBest(3);

			result.MovieList = new MovieListModel()
			{
				ComparisonHeader = result.IsTracking ? "Estimated" : null,
				ComparisonMovies = result.IsTracking ? _minerModel.Miners[FML_INDEX].Movies : null,
				Picks = pickList
			};

			if (!onlyBestPerformer)
			{
				// Need to clone the list otherwise the above MovieList will lose its BestPerformer.

				var clonedList = new List<IMovie>();

				foreach (var movie in result.Movies)
				{
					clonedList.Add(movie.Clone());
				}

				_moviePicker.AddMovies(clonedList);
				_moviePicker.EnableBestPerformer = false;

				pickList = _moviePicker.ChooseBest(3);

				result.MovieListBonusOff = new MovieListModel()
				{
					ComparisonHeader = result.IsTracking ? "Estimated" : null,
					ComparisonMovies = result.IsTracking ? _minerModel.Miners[FML_INDEX].Movies : null,
					Picks = pickList
				};
			}

			if (result.IsTracking)
			{
				// Don't need to use clones, because the BONUS is always used for best possible values.

				_moviePicker.EnableBestPerformer = true;
				_moviePicker.AddMovies(_minerModel.Miners[FML_INDEX].Movies);

				result.MovieListPerfectPick = new MovieListModel()
				{
					ComparisonHeader = "Custom",
					ComparisonMovies = result.Movies,
					Picks = new List<IMovieList> { _moviePicker.ChooseBest() }
				};
			}

			result.SharedPicksUrl = SharedPicksFromModels();

			var leadMovie = result.MovieList.Picks?.First().Movies.OrderByDescending(movie => movie.Cost).FirstOrDefault()?.Name;

			// Needs to be put in the ViewBag since this value is on the footer.
			ViewBag.TwitterTweetUrl = $"https://twitter.com/intent/tweet?text={leadMovie} leads my lineup @fml_movies {result.SharedPicksUrl.Replace("?", "%3F")}";

			DownloadMoviePosters();

			stopWatch.Stop();

			result.Duration = stopWatch.ElapsedMilliseconds;

			return result;
		}

		private SharePicksViewModel ConstructSharePicksViewModel(bool bonusOn, PicksViewModel picksViewModel = null, int index = 0)
		{
			ViewBag.Title = bonusOn ? "Share Bonus ON Picks" : "Share Bonus OFF Picks";
			var subTitle = bonusOn ? "Bonus ON" : "Bonus OFF";

			ParseBoxOfficeWeightRequest();

			if (picksViewModel == null)
			{
				picksViewModel = ConstructPicksViewModel();
			}

			var webRootPath = Server.MapPath("~");
			var localFilePrefix = $"{webRootPath}images{Path.DirectorySeparatorChar}";
			var picks = bonusOn ? picksViewModel.MovieList.Picks[index] : picksViewModel.MovieListBonusOff.Picks[index];
			var movieImages = picks.MovieImages.Select(movie => Path.GetFileName(movie.Replace("MoviePoster_", string.Empty)));

			// Files should already be there now.
			//FileUtility.DownloadFiles(movieImages, localFilePrefix);

			var localFiles = FileUtility.LocalFiles(movieImages, $"{localFilePrefix}MoviePoster_");
			string bonusFile = null;

			if (bonusOn)
			{
				var bonusMovie = picks.Movies.FirstOrDefault(movie => movie.IsBestPerformer);

				if (bonusMovie != null && bonusMovie.ImageUrl != null)
				{
					bonusFile = FileUtility.LocalFile(bonusMovie.ImageUrl.Replace("MoviePoster_", string.Empty), $"{localFilePrefix}MoviePoster_");
				}
			}

			// Attempt to use the temp posters first since that is more restrictive, otherwise you might get BOTH sets and you'll see duplicates in the randomization.

			var filmCellFileNames = FileUtility.FilterImagesInPath(localFilePrefix, "MoviePoster_*.temp.*");

			if (filmCellFileNames == null || filmCellFileNames.Count == 0)
			{
				// Just in case there are no "temp" names.

				filmCellFileNames = FileUtility.FilterImagesInPath(localFilePrefix, "MoviePoster_*.*");
			}

			var filmCellFiles = FileUtility.LocalFiles(filmCellFileNames, localFilePrefix);

			var viewModel = new SharePicksViewModel()
			{
				//ImageFileName = picksViewModel.GenerateSharedImage(webRootPath, localFiles, bonusFile, null)
				ImageFileName = picksViewModel.GenerateSharedImage(webRootPath, localFiles, bonusFile, filmCellFiles)
			};

			// Ordering by Cost is the same sort as the file names.
			var leadingMovieName = picks.Movies.OrderByDescending(movie => movie.Cost).FirstOrDefault()?.Name;
			var bonusMovieName = bonusOn ? picks.Movies.FirstOrDefault(movie => movie.IsBestPerformer)?.Name : null;
			var spentBux = picks.Movies.Sum(movie => movie.Cost);
			var spentBuxText = spentBux == 1000 ? " and spent all my BUX" : $" and spent {spentBux} BUX";
			var minerPick = MinerWeightToMiner();
			var lineupArticle = minerPick == null ? "my" : "the";

			if (bonusMovieName == null)
			{
				bonusMovieName = picks.Movies.GroupBy(movie => movie.Name)
											.OrderByDescending(group => group.Count())
											.FirstOrDefault()?.Key;
			}

			if (bonusMovieName == leadingMovieName)
			{
				bonusMovieName = "it";
			}

			bonusMovieName = (bonusOn) ? $", counting on {bonusMovieName} as the bonus movie" : $", hoping for {bonusMovieName} as the bonus movie";

			viewModel.TwitterDescription = $"{leadingMovieName} leads {lineupArticle} lineup{bonusMovieName}{spentBuxText}.";
			viewModel.TwitterImageFileName = viewModel.ImageFileName.Replace("Shared_", "Twitter_");
			viewModel.TwitterTitle = $"{Constants.APPLICATION_NAME}: {subTitle} (Est ${picks.TotalEarnings:N0})";

			var defaultTwitterText = minerPick == null ? "Check out my @fml_movies picks." : $"If you're {minerPick.Name} @{minerPick.TwitterID} your @fml_movies picks are:";

			defaultTwitterText += picks.ToString();
			defaultTwitterText += minerPick == null ? " only cost me " : " cost ";
			defaultTwitterText += $"{spentBux.ToString("N0")} BUX #ShowYourScreens @SpilledMilkCOM";

			ControllerUtility.SetTwitterCard(ViewBag, "summary_large_image"
											, viewModel.TwitterTitle
											, viewModel.TwitterDescription
											, $"{Constants.WEBSITE_URL}/images/{viewModel.TwitterImageFileName}"
											, "Collage of my movie lineups."
											, defaultTwitterText);

			DownloadMoviePosters();

			return viewModel;
		}

		private void DownloadMoviePosters()
		{
			var localFilePrefix = $"{Server.MapPath("~")}images{Path.DirectorySeparatorChar}MoviePoster_";

			if (_minerModel.DownloadMoviePosters(localFilePrefix))
			{
				// If the posters were downloaded then update the cached image urls to the local files.

				//lock (_minerModelCache)
				//{
				//	_minerModelCache.Miners[FML_INDEX].SetMovies(new List<IMovie>(_minerModel.Miners[FML_INDEX].Movies));
				//}
			}
		}

		private void ParseBoxOfficeWeightRequest()
		{
			char[] listDelimiter = { ',' };
			bool hasParams = false;

			// Attempt to parse the request.

			//var parms = Request.Params;
			var decimalList = _controllerUtility.GetRequestDecimalList(Request, "bo");
			int idx = 0;

			hasParams = decimalList.Any();

			foreach (var movie in _minerModel.Miners[MY_MINER_IDX].Movies)
			{
				if (idx < decimalList.Count)
				{
					movie.Earnings = decimalList[idx++];
				}
				else
				{
					movie.Earnings = 0;
				}
			}

			var intList = _controllerUtility.GetRequestIntList(Request, "wl");
			idx = 0;
			hasParams |= intList.Any();

			bool isFirst = true;

			foreach (var miner in _minerModel.Miners)
			{
				if (!isFirst && idx < intList.Count)
				{
					miner.Weight = intList[idx++];
				}

				isFirst = false;
			}

			if (hasParams)
			{
				// Refresh my picks based on the weights.

				((ICache)_minerModel.Miners[MY_MINER_IDX]).Load();
			}

			UpdateViewModel();
		}

		private void ParseViewRequest()
		{
		}

		private void RunSimulation(PicksViewModel picksViewModel)
		{
			var stopwatch = new Stopwatch();

			// Add picked movies.

			var movies = picksViewModel.MovieList.Picks[0].Movies.Distinct().ToList();

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

			picksViewModel.MovieList = new MovieListModel()
			{
				Picks = new List<IMovieList> { _moviePicker.ChooseBest() }
			};

			picksViewModel.Duration += stopwatch.ElapsedMilliseconds;
		}

		/// <summary>
		/// Coverts the weight list in the request to a miner.
		/// </summary>
		/// <param name="weightList">The weight list from the request</param>
		/// <returns></returns>
		private IMiner MinerWeightToMiner()
		{
			IMiner result = null;
			int index = 0;
			char[] delimiters = new char[] { ',' };
			var weightList = ControllerUtility.GetRequestString(Request, "wl");

			if (weightList != null)
			{
				var weights = weightList.Split(delimiters);
				bool minerWeightsOnly = ControllerUtility.GetRequestString(Request, "bo") == null && weightList != null;

				if (minerWeightsOnly)
				{
					foreach (var miner in _minerModel.Miners.Skip(1))
					{
						if (index < weights.Length && weights[index] == "1")
						{
							result = miner;
						}

						index++;
					}
				}
			}

			return result;
		}

		private string SharedPicksFromModels()
		{
			var stringBuilder = new StringBuilder();
			bool first = true;

			stringBuilder.Append(TrimParameters(Request.Url.ToString()));
			stringBuilder.Append("?bo=");

			foreach (var movie in _minerModel.Miners[MY_MINER_IDX].Movies)
			{
				if (!first)
				{
					stringBuilder.Append(",");
				}
				else
				{
					first = false;
				}

				stringBuilder.Append(movie.EarningsBase.ToString("F0"));
			}

			first = true;
			stringBuilder.Append("&wl=");

			for (int idx = MY_MINER_IDX; idx < _minerModel.Miners.Count; idx++)
			{
				if (!first)
				{
					stringBuilder.Append(",");
				}
				else
				{
					first = false;
				}

				stringBuilder.Append(_minerModel.Miners[idx].Weight.ToString("F0"));
			}

			return stringBuilder.ToString();
		}

		private string TrimParameters(string request)
		{
			var paramsIdx = request.IndexOf("?");

			if (paramsIdx > 0)
			{
				request = request.Substring(0, paramsIdx);
			}

			return request;
		}

		private void UpdateViewModel()
		{
			// TODO: Use reflection...

			int index = 1;

			_viewModel.Weight1 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight2 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight3 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight4 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight5 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight6 = _minerModel.Miners[index++].Weight;
			_viewModel.Weight7 = _minerModel.Miners[index++].Weight;

			var myBoxOffice = _minerModel.Miners[1].Movies;

			index = 0;

			if (myBoxOffice != null && myBoxOffice.Count >= 15)
			{
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
		}
	}
}