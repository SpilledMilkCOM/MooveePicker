using MovieMiner;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using MoviePicker.WebApp.Utilities;
using MoviePicker.WebApp.ViewModels;
using SM.Common.Emoji;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
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
		private const int DATA_MINER_COUNT = 6;
		private const int MORE_PICKS_COUNT = 6;
		private const string NEW_LINE_HTML = "%0a";
		private const string PERCENT_HTML = "%25";

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

		[HttpGet]
		public ActionResult Calculate()
		{
			ParseViewRequest();

			//Thread.Sleep(1000);			// Impose a load for testing.

			var result = new CalculateViewModel(ConstructPicksViewModel());

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
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
		public ActionResult ExpertPicks()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var result = new ExpertPicksViewModel();
			var fmlMiner = _minerModel.Miners[MinerModel.FML_INDEX];
			var fmlMovies = fmlMiner.Movies;
			var lastMiner = _minerModel.Miners[MinerModel.MOJO_LAST_INDEX];
			var minerCount = 0;
			var weekendEnding = _minerModel.WeekendEnding;

			if (fmlMiner.ContainsEstimates && fmlMiner.Movies.Count > 0)
			{
				result.MovieListPerfectPick = PerfectPick(null);
			}

			// Create each ExpertPickModel and attach their bonus ON and OFF MovieListModels

			foreach (var miner in _minerModel.Miners.Skip(MinerModel.MY_INDEX + 1))
			{
				if (miner.Picks != null && miner.Movies.Count > 0 && !miner.IsHidden && miner != lastMiner)
				{
					var expert = new ExpertPickModel { Miner = miner };
					var shareQueryString = $"{WeightListFromCounter(minerCount)}&id={Guid.NewGuid()}&we={weekendEnding}";      // Need to add the unique ID for Twitter to regenerate the page/image.

					// Make sure the images are synchronized.

					foreach (var movie in miner.Picks.Movies)
					{
						var fmlMovie = fmlMovies.FirstOrDefault(item => item.Equals(movie));

						if (fmlMovie != null)
						{
							movie.ImageUrl = fmlMovie.ImageUrl;
						}
					}

					foreach (var movie in miner.PicksBonusOff.Movies)
					{
						var fmlMovie = fmlMovies.FirstOrDefault(item => item.Equals(movie));

						if (fmlMovie != null)
						{
							movie.ImageUrl = fmlMovie.ImageUrl;
						}
					}

					expert.MovieList = new MovieListModel()
					{
						ComparisonHeader = "Bonus ON",
						ComparisonMovies = fmlMiner.Movies,
						Picks = new List<IMovieList> { miner.Picks },
						ShareQueryString = $"/Home/ShareBonusOnPicks?{shareQueryString}"
					};

					expert.MovieListBonusOff = new MovieListModel()
					{
						ComparisonHeader = "Bonus OFF",
						ComparisonMovies = fmlMiner.Movies,
						Picks = new List<IMovieList> { miner.PicksBonusOff },
						ShareQueryString = $"/Home/ShareBonusOffPicks?{shareQueryString}"
					};

					if (result.MovieListPerfectPick != null)
					{
						expert.TotalPicksFromComparison = expert.MovieList.TotalPicksFromComparison;
					}

					result.ExpertPicks.Add(expert);
				}

				minerCount++;
			}

			// Set default tweet

			var nl = NEW_LINE_HTML;
			var weighingIn = result.ExpertPicks.Count == 6 ? "All" : $"So far, {result.ExpertPicks.Count}";
			var plural = result.ExpertPicks.Count != 1 ? "s" : string.Empty;
			var tweetText = $"{weighingIn} expert{plural} weighing in the box office predictions this weekend";

			if (result.MovieListPerfectPick != null)
			{
				var topExpert = result.ExpertPicks.OrderByDescending(expert => expert.TotalPicksFromComparison).FirstOrDefault();

				if (topExpert != null)
				{
					tweetText = $"So far, {topExpert.Miner.Name} @{topExpert.Miner.TwitterID} leads with a total box office of ${ViewUtility.SmallDollars(topExpert.TotalPicksFromComparison)} and ";

					if (topExpert.TotalPicksFromComparison < result.MovieListPerfectPick.Picks[0].TotalEarnings)
					{
						tweetText += $"is trailing the perfect pick by ${ViewUtility.SmallDollars(result.MovieListPerfectPick.Picks[0].TotalEarnings - topExpert.TotalPicksFromComparison)} ({(result.MovieListPerfectPick.Picks[0].TotalEarnings - topExpert.TotalPicksFromComparison) / result.MovieListPerfectPick.Picks[0].TotalEarnings * 100:N0}{PERCENT_HTML})";
					}
					else
					{
						tweetText += "has nailed that perfect pick!";
					}

					tweetText += $"{nl}";

					foreach (var expert in result.ExpertPicks.OrderByDescending(item => item.TotalPicksFromComparison).Skip(1).Take(2))
					{
						tweetText += $"{nl}@{expert.Miner.TwitterID} ${ViewUtility.SmallDollars(expert.TotalPicksFromComparison)} [$-{ViewUtility.SmallDollars(result.MovieListPerfectPick.Picks[0].TotalEarnings - expert.TotalPicksFromComparison)} (-{(result.MovieListPerfectPick.Picks[0].TotalEarnings - expert.TotalPicksFromComparison) / result.MovieListPerfectPick.Picks[0].TotalEarnings * 100:N0}{PERCENT_HTML})]";
					}
				}
			}

			tweetText += $"{nl}{nl}#ShowYourScreens #BoxOffice @SpilledMilkCOM";

			ControllerUtility.SetTwitterCard(ViewBag, null, $"{Constants.APPLICATION_NAME} - Expert League", "See what the experts pick AND their ranking when the estimates are in.", null, null, tweetText);

			stopWatch.Stop();

			result.Duration = stopWatch.ElapsedMilliseconds;

			return View(result);
		}

		[HttpGet]
		public FileStreamResult ExtractToCSV()
		{
			var builder = new StringBuilder();
			var theaterCounts = _minerModel.Miners[MinerModel.NUMBERS_THEATER_INDEX];
			var lastWeekMojo = _minerModel.Miners[MinerModel.MOJO_LAST_INDEX];

			// Column headers are FIRST!

			builder.Append("BUX,Movie");

			foreach (var miner in _minerModel.Miners)
			{
				builder.Append(",");
				builder.Append(miner.Abbreviation);

				if (miner == lastWeekMojo)
				{
					// Basically doubles up the last week's Mojo (BO and Theater Count)

					builder.Append(",");
					builder.Append(miner.Abbreviation + " TC");
				}
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

					if (miner == theaterCounts)
					{
						builder.Append(minerMovie?.TheaterCount);
					}
					else
					{
						builder.Append(minerMovie?.Earnings.ToString(CultureInfo.InvariantCulture));
					}

					if (miner == lastWeekMojo)
					{
						// Basically doubles up the last week's Mojo (BO and Theater Count)

						builder.Append(",");
						builder.Append(minerMovie?.TheaterCount);
					}
				}

				builder.AppendLine();
			}

			var byteArray = Encoding.ASCII.GetBytes(builder.ToString());
			var stream = new MemoryStream(byteArray);

			return File(stream, "text/plain", "MooveePickerData.txt");
		}

		[HttpGet]
		public ActionResult Fandango()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			IFandangoViewModel viewModel = new FandangoViewModel(_minerModel, _moviePicker);

			viewModel.PastHours = _controllerUtility.GetRequestInt(Request, "past") ?? 24;

			DownloadMoviePosters();

			viewModel.Load();

			if (viewModel.IsTracking && _minerModel.Miners[MinerModel.FML_INDEX].Movies.Count > 0)
			{
				viewModel.MovieListPerfectPick = PerfectPick(viewModel.Movies);
			}

			var movieList = viewModel.Movies.OrderByDescending(movie => movie.EarningsBase).ToList();
			var totalBoxOffice = movieList.Sum(movie => movie.EarningsBase);

			var nl = NEW_LINE_HTML;
			var tweetText = $"The top @Fandango tickets sales for the past {viewModel.PastHours} hours:{nl}";

			foreach (var movie in movieList.Take(3))
			{
				tweetText += $"{nl}{movie.Hashtag} {(movie.EarningsBase / totalBoxOffice * 100).ToString("N1")}{PERCENT_HTML}";
			}

			tweetText += $"{nl}{nl}#ShowYourScreens @SpilledMilkCOM";

			ControllerUtility.SetTwitterCard(ViewBag, null, $"{Constants.APPLICATION_NAME} - Fandango Hourly Sales", "A breakdown of the hourly ticket sales by percentages. (click-through for up-to-date numbers)", null, null, tweetText);


			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		[HttpGet]
		public ActionResult FandangoDays()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			IFandangoViewModel viewModel = new FandangoDaysViewModel(_minerModel, _moviePicker);

			DownloadMoviePosters();

			ClearMinerModel(0);

			ParseViewRequest();

			viewModel.Load();

			var movieList = viewModel.Movies.OrderByDescending(movie => movie.EarningsBase).ToList();
			var totalBoxOffice = movieList.Sum(movie => movie.EarningsBase);

			// Scale the estimates (if they exist) to match the percentages of the sales.
			// o Use the sales scale outright and multiply the totalEstimate to get BO
			// o Combine the scales (somehow) and multiply the totalEstimate to get BO

			if (totalBoxOffice > 0)
			{
				var myMovieList = _minerModel.Miners[MinerModel.MY_INDEX].Movies;
				var totalEstimates = myMovieList?.Sum(item => item.EarningsBase) ?? 0;

				if (totalEstimates > 0)
				{
					foreach (var movie in movieList)
					{
						var myMovie = myMovieList.FirstOrDefault(item => item.Equals(movie));

						if (myMovie != null)
						{
							// For now this is option 1.
							myMovie.Earnings = totalEstimates * movie.EarningsBase / totalBoxOffice;
						}
					}
				}
			}

			if (viewModel.IsTracking && _minerModel.Miners[MinerModel.FML_INDEX].Movies.Count > 0)
			{
				viewModel.MovieListPerfectPick = PerfectPick(viewModel.Movies);
			}

			var nl = NEW_LINE_HTML;
			var tweetText = $"The top @Fandango tickets sales for the weekend ending {_minerModel.WeekendEnding.Value.ToShortDateString()}:{nl}";

			foreach (var movie in movieList.Take(3))
			{
				tweetText += $"{nl}{movie.Hashtag} {(movie.EarningsBase / totalBoxOffice * 100).ToString("N1")}{PERCENT_HTML}";
			}

			tweetText += $"{nl}{nl}#ShowYourScreens @SpilledMilkCOM";

			ControllerUtility.SetTwitterCard(ViewBag, null, $"{Constants.APPLICATION_NAME} - Fandango Weekend Sales", "A breakdown of the weekend ticket sales by percentages. (click-through for up-to-date numbers)", null, null, tweetText);

			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		[HttpGet]
		public ActionResult FandangoFutures()
		{
			const string FUTURE_URL_KEY = "fandango:future";

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var futureMiner = new MineFandangoTicketSalesFuture(ConfigurationManager.AppSettings[FUTURE_URL_KEY]);
			var viewModel = new FandangoFutureViewModel(_minerModel, futureMiner);

			viewModel.Load();

			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		[HttpGet]
		public ActionResult History()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var loaded = false;
			var movieList = new List<IMovie>();

			// When a miner is cloned.  Only the list is cloned, but you can still affect the movie inside the base list.
			// Which is why EACH movie needs to be cloned here, because its box office list can get updated with the estimates if they are part of the Query string.

			_minerModel.Miners[MinerModel.FML_INDEX].Movies.ForEach(item => movieList.Add(item.Clone()));

			var viewModel = new HistoryViewModel { Movies = movieList };

			ClearMinerModel(0);

			ParseViewRequest();

			foreach (var movie in viewModel.Movies)
			{
				// Only load the history if needed.

				if (movie.BoxOfficeHistory == null)
				{
					var mojoMovie = _minerModel.Miners[MinerModel.MOJO_LAST_INDEX].Movies.FirstOrDefault(item => item.Equals(movie));

					if (mojoMovie != null && mojoMovie.Identifier != null)
					{
						var history = new MineBoxOfficeMojoHistory(mojoMovie.Identifier);
						var movies = history.Mine();

						movie.Identifier = mojoMovie.Identifier;

						if (movies.Any())
						{
							movie.SetBoxOfficeHistory(movies.First().BoxOfficeHistory);
						}

						loaded = true;
					}
				}
			}


			if (loaded)
			{
				// Update the FML cache if history was loaded.

				lock (_minerModelCache)
				{
					var cachedFmlMovies = _minerModelCache.Miners[MinerModel.MOJO_LAST_INDEX].Movies;

					foreach (var movie in viewModel.Movies)
					{
						if (movie.BoxOfficeHistory == null || movie.BoxOfficeHistory.Any())
						{
							var fmlMovie = cachedFmlMovies.FirstOrDefault(item => item.Equals(movie));

							if (fmlMovie != null)
							{
								fmlMovie.Identifier = movie.Identifier;                 // This can be used to link to the Box Office Mojo website.
								fmlMovie.SetBoxOfficeHistory(movie.BoxOfficeHistory);
							}
						}
					}
				}
			}

			// Add the user estimates to the ViewModel values.

			var userMovies = _minerModel.Miners[MinerModel.MY_INDEX].Movies;
			var theaterCountMovies = _minerModel.Miners[MinerModel.NUMBERS_THEATER_INDEX].Movies;

			foreach (var movie in viewModel.Movies)
			{
				var userMovie = userMovies.FirstOrDefault(item => item.Equals(movie));
				var theaterCountMovie = theaterCountMovies.FirstOrDefault(item => item.Equals(movie));

				if (userMovie != null && userMovie.Earnings > 0)
				{
					var boxOfficeHistory = movie.BoxOfficeHistory?.ToList() ?? new List<IBoxOffice>();

					IBoxOffice boxOffice = new BoxOffice
					{
						Earnings = userMovie.Earnings,
						TheaterCount = theaterCountMovie?.TheaterCount ?? 0,
						WeekendEnding = userMovie.WeekendEnding
					};

					boxOfficeHistory.Add(boxOffice);
					movie.SetBoxOfficeHistory(boxOfficeHistory);
				}
			}

			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		[HttpGet]
		public ActionResult Index()
		{
			ClearMinerModel();

			// Adjust the weights (possibly adjust the defaults above).

			ParseViewRequest();

			// Hide the last miner (BO Mojo for previous week).

			_minerModel.Miners.Last().IsHidden = true;

			ControllerUtility.SetTwitterCard(ViewBag);

			return View(UpdatePicksViewModel(ConstructPicksViewModel()));
		}

		[HttpGet]
		public ActionResult IndexOld()
		{
			var stopWatch = new Stopwatch();

			stopWatch.Start();

			_viewModel.ViewGridOpen = !Request.Browser.IsMobileDevice;
			_viewModel.ViewMobileOpen = Request.Browser.IsMobileDevice;

			ParseViewRequest();

			_viewModel.IsTracking = _minerModel.Miners[MinerModel.FML_INDEX].ContainsEstimates;

			ControllerUtility.SetTwitterCard(ViewBag);

			DownloadMoviePosters();

			stopWatch.Stop();

			_viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(_viewModel);
		}

		[HttpGet]
		public ActionResult IndexWide()
		{
			// TODO: Collapse this down to a method call.

			// Set the weights to 1 across the board. (treat all sources equal)

			for (int minerIndex = MinerModel.MY_INDEX + 1; minerIndex < MinerModel.NUMBERS_THEATER_INDEX; minerIndex++)
			{
				_minerModel.Miners[minerIndex].Weight = 1;
			}

			// Do NOT include MY numbers when pre-populating the box office values;

			_minerModel.Miners[MinerModel.MY_INDEX].Weight = 0;
			((ICache)_minerModel.Miners[MinerModel.MY_INDEX]).Load();

			// Adjust the weights (possibly adjust the defaults above).

			ParseViewRequest();

			// Hide the last miner (BO Mojo for previous week).

			_minerModel.Miners.Last().IsHidden = true;

			ControllerUtility.SetTwitterCard(ViewBag);

			return View(UpdatePicksViewModel(ConstructPicksViewModel()));
		}

		//[HttpGet]
		//public ActionResult MorePicks()
		//{
		//	ParseViewRequest();

		//	ControllerUtility.SetTwitterCard(ViewBag);

		//	DownloadMoviePosters();

		//	return View(ConstructMorePicksViewModel());
		//}

		[HttpGet]
		public ActionResult ShareBonusOffPicks()
		{
			return View("SharePicks", ConstructSharePicksViewModel(false, false));
		}

		[HttpGet]
		public ActionResult ShareBonusOnPicks()
		{
			return View("SharePicks", ConstructSharePicksViewModel(true, false));
		}

		[HttpGet]
		public ActionResult SharePerfectPick()
		{
			return View("SharePicks", ConstructSharePicksViewModel(true, true));
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

			ParseViewRequest();

			var viewModel = ConstructPicksViewModel();

			var sharedViewModel = ConstructSharePicksViewModel(true, true, viewModel);

			viewModel.MovieList.ComparisonHeader = "Your Custom Picks";

			// Show the values for the FML Estimate data.

			_minerModel.Miners.First().IsHidden = false;

			// Hide the last miner (BO Mojo for previous week).

			_minerModel.Miners.Last().IsHidden = true;

			// TODO: Adjust text based on the miner's weight.

			ControllerUtility.SetTwitterCard(ViewBag, "summary_large_image", null
								, "Tracking my picks against the perfect pick to see where I went right or horribly wrong."
								, $"{Constants.WEBSITE_URL}/images/{sharedViewModel.TwitterImageFileName}"
								, "Collage of the perfect pick lineup."
								, "Check out my picks against the perfect pick. #ShowYourScreens");

			ControllerUtility.SetOpenGraph(ViewBag, Request);

			stopWatch.Stop();

			viewModel.Duration = stopWatch.ElapsedMilliseconds;

			return View(viewModel);
		}

		public ActionResult WeekBack()
		{
			return RedirectToAction("Index");
		}

		//----==== PRIVATE ====--------------------------------------------------------------------

		private void ClearMinerModel(decimal weight = 1)
		{
			// Set the weights to 1 across the board. (treat all 'expert' sources equal)

			for (int minerIndex = MinerModel.MY_INDEX + 1; minerIndex < MinerModel.NUMBERS_THEATER_INDEX; minerIndex++)
			{
				_minerModel.Miners[minerIndex].Weight = weight;
			}

			// Do NOT include MY numbers when pre-populating the box office values;

			_minerModel.Miners[MinerModel.MY_INDEX].Weight = 0;
			((ICache)_minerModel.Miners[MinerModel.MY_INDEX]).Load();
		}

		private List<IMovie> CloneList(IEnumerable<IMovie> list)
		{
			var result = new List<IMovie>();

			foreach (var movie in list.Where(item => item.Earnings > 0))
			{
				result.Add(movie.Clone());
			}

			return result;
		}

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
				var pickCount = 1;

				foreach (var movieList in moviePicker.ChooseBest(MORE_PICKS_COUNT))
				{
					result.MorePicks.Add(new MovieListModel { ComparisonHeader = $"Picks {pickCount++}", Picks = new List<IMovieList> { movieList } });
				}
			}))
			{ Name = "bonusThread" };

			var bonusOffThread = new Thread(new ThreadStart(() =>
			{
				var pickCount = 1;

				foreach (var movieList in moviePickerBonusOff.ChooseBest(MORE_PICKS_COUNT))
				{
					result.MorePicksBonusOff.Add(new MovieListModel { ComparisonHeader = $"Picks {pickCount++}", Picks = new List<IMovieList> { movieList } });
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
			var fmlMiner = _minerModel.Miners[MinerModel.FML_INDEX];

			stopWatch.Start();

			DownloadMoviePosters();

			result.IsTracking = fmlMiner.ContainsEstimates;

			result.Miners = _minerModel.Miners;
			result.Movies = _minerModel.CreateWeightedList();

			ParseBonusBarRequest(result.Movies);
			ParseBonusBarRequest(_minerModel.Miners[MinerModel.MY_INDEX].Movies);

			if (result.Movies.Count() > 0)
			{
				var clonedList = CloneList(result.Movies);
				var shareQueryString = $"{QueryStringFromModel()}&id={Guid.NewGuid()}";     // Need to add the unique ID for Twitter to regenerate the page/image.

				_moviePicker.AddMovies(clonedList);

				var pickList = _moviePicker.ChooseBest(3);

				result.MovieList = new MovieListModel()
				{
					ComparisonHeader = "Bonus ON",
					ComparisonMovies = result.IsTracking ? fmlMiner.Movies : null,
					Id = "bonusOnMovieList",
					Picks = pickList,
					ShareQueryString = $"/Home/ShareBonusOnPicks?{shareQueryString}"
				};

				if (!onlyBestPerformer)
				{
					// Need to clone the list otherwise the above MovieList will lose its BestPerformer.

					clonedList = CloneList(result.Movies);

					_moviePicker.AddMovies(clonedList);
					_moviePicker.EnableBestPerformer = false;

					pickList = _moviePicker.ChooseBest(3);

					result.MovieListBonusOff = new MovieListModel()
					{
						ComparisonHeader = "Bonus OFF",
						ComparisonMovies = result.IsTracking ? fmlMiner.Movies : null,
						Id = "bonusOffMovieList",
						Picks = pickList,
						ShareQueryString = $"/Home/ShareBonusOffPicks?{shareQueryString}"
					};
				}
			}

			if (result.IsTracking && fmlMiner.Movies.Count > 0)
			{
				result.MovieListPerfectPick = PerfectPick(result.Movies);
			}

			result.SharedPicksUrl = SharedPicksFromModels();

			var leadMovie = result.MovieList?.Picks?.First().Movies.OrderByDescending(movie => movie.Cost).FirstOrDefault()?.Name;

			// Needs to be put in the ViewBag since this value is on the footer.
			ViewBag.TwitterTweetText = $"{leadMovie} leads my lineup @fml_movies {result.SharedPicksUrl.Replace("?", "%3F")}";

			var viewModel = new SharePicksViewModel()
			{
				ImageFileName = _viewModel.ImageType == null ? null : GenerateSharedImage(_viewModel.ImageType == "on", result.IsTracking, result)
			};

			stopWatch.Stop();

			result.Duration = stopWatch.ElapsedMilliseconds;

			return result;
		}

		private SharePicksViewModel ConstructSharePicksViewModel(bool bonusOn, bool isTracking, PicksViewModel picksViewModel = null, int index = 0)
		{
			ViewBag.Title = bonusOn ? "Share Bonus ON Picks" : "Share Bonus OFF Picks";
			var subTitle = bonusOn ? "Bonus ON" : "Bonus OFF";

			ParseViewRequest();

			if (picksViewModel == null)
			{
				picksViewModel = ConstructPicksViewModel();
			}

			var picks = bonusOn ? picksViewModel.MovieList?.Picks[index] : picksViewModel.MovieListBonusOff?.Picks[index];

			var viewModel = new SharePicksViewModel()
			{
				ImageFileName = GenerateSharedImage(bonusOn, isTracking, picksViewModel)
			};

			SetupSharedTweet(bonusOn, viewModel, picks);

			DownloadMoviePosters();

			return viewModel;
		}

		private void DownloadMoviePosters()
		{
			var localFilePrefix = $"{Server.MapPath("~")}images{Path.DirectorySeparatorChar}MoviePoster_";

			if (_minerModel.DownloadMoviePosters(localFilePrefix))
			{
				// If the posters were downloaded then update the cached image urls to the local files.

				lock (_minerModelCache)
				{
					_minerModelCache.Miners[MinerModel.FML_INDEX].SetMovies(new List<IMovie>(_minerModel.Miners[MinerModel.FML_INDEX].Movies));
				}
			}
		}

		/// <summary>
		/// Generate a shared image for Twitter (if it doesn't already exist)
		/// </summary>
		/// <param name="bonusOn"></param>
		/// <param name="isTracking"></param>
		/// <param name="picksViewModel"></param>
		/// <param name="index"></param>
		/// <returns>The name of the image file.</returns>
		private string GenerateSharedImage(bool bonusOn, bool isTracking, PicksViewModel picksViewModel, int index = 0)
		{
			var webRootPath = Server.MapPath("~");
			var localFilePrefix = $"{webRootPath}images{Path.DirectorySeparatorChar}";
			var result = $"Twitter_{_viewModel.Id}";

			if (!System.IO.File.Exists($"{localFilePrefix}"))
			{
				var picks = bonusOn ? picksViewModel.MovieList?.Picks[index] : picksViewModel.MovieListBonusOff?.Picks[index];
				var movieImages = picks?.MovieImages?.Select(movie => Path.GetFileName(movie.Replace("MoviePoster_", string.Empty)));
				var localFiles = FileUtility.LocalFiles(movieImages, $"{localFilePrefix}MoviePoster_");
				string bonusFile = null;
				string perfectPickBonusFile = null;

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

				List<string> perfectPickFiles = null;

				if (isTracking && picksViewModel.IsTracking)
				{
					var perfectPick = picksViewModel?.MovieListPerfectPick?.Picks[0];
					var perfectPickImages = perfectPick?.MovieImages?.Select(movie => Path.GetFileName(movie.Replace("MoviePoster_", string.Empty)));

					perfectPickFiles = FileUtility.LocalFiles(perfectPickImages, $"{localFilePrefix}MoviePoster_");

					var bonusMovie = perfectPick.Movies.FirstOrDefault(movie => movie.IsBestPerformer);

					if (bonusMovie != null && bonusMovie.ImageUrl != null)
					{
						perfectPickBonusFile = FileUtility.LocalFile(bonusMovie.ImageUrl.Replace("MoviePoster_", string.Empty), $"{localFilePrefix}MoviePoster_");
					}
				}

				result = picksViewModel.GenerateSharedImage(webRootPath, localFiles, perfectPickFiles, bonusFile, perfectPickBonusFile, filmCellFiles);

				if (result != null)
				{
					_viewModel.Id = new Guid(result.Replace("Twitter_", string.Empty).Replace(".jpg", string.Empty));
				}
			}

			return result;
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
			var weightList = _controllerUtility.GetRequestString(Request, "wl");

			if (weightList != null)
			{
				var weights = weightList.Split(delimiters);

				if (weightList != null && weights[0] == "0")
				{
					foreach (var miner in _minerModel.Miners.Skip(1))
					{
						if (index < weights.Length && weights[index] == "1")
						{
							if (result != null)
							{
								// If there are MORE than one, then fail.

								result = null;
								break;
							}

							result = miner;
						}

						index++;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Update the movie list based on the Bonus Bar (Earnings = Cost * BB * 1000)
		/// </summary>
		/// <param name="movies"></param>
		private void ParseBonusBarRequest(IEnumerable<IMovie> movies)
		{
			var bonusBar = _controllerUtility.GetRequestDecimal(Request, "bb");

			if (bonusBar.HasValue)
			{
				foreach (var movie in movies)
				{
					movie.Earnings = movie.Cost * bonusBar.Value * 1000;
				}
			}
		}

		/// <summary>
		/// Parse the Box Office and Miner weights from the Request parameters into the miners and view model.
		/// </summary>
		private void ParseBoxOfficeWeightRequest()
		{
			char[] listDelimiter = { ',' };
			bool hasParams = false;

			// Attempt to parse the request.

			//var parms = Request.Params;
			var decimalList = _controllerUtility.GetRequestDecimalList(Request, "bo");
			int idx = 0;

			hasParams = decimalList.Any();

			if (hasParams)
			{
				foreach (var movie in _minerModel.Miners[MinerModel.MY_INDEX].Movies)
				{
					if (idx < decimalList.Count)
					{
						movie.Earnings = decimalList[idx++];
					}
				}
			}

			var intList = _controllerUtility.GetRequestIntList(Request, "wl");
			idx = 0;
			hasParams |= intList.Any();

			foreach (var miner in _minerModel.Miners.Skip(1))
			{
				if (idx < intList.Count && !miner.IsHidden)
				{
					miner.Weight = intList[idx];
				}

				idx++;
			}

			if (hasParams)
			{
				// Refresh my picks based on the weights.

				((ICache)_minerModel.Miners[MinerModel.MY_INDEX]).Load();
			}

			UpdateViewModel();
		}

		/// <summary>
		/// Parse ALL of the view Request parameters into the view model. (including the BO and weights)
		/// </summary>
		private void ParseViewRequest()
		{
			ParseBoxOfficeWeightRequest();

			_viewModel.ImageType = _controllerUtility.GetRequestString(Request, "it");
			_viewModel.Id = _controllerUtility.GetRequestGuid(Request, "id");
		}

		/// <summary>
		/// Return the perfect pick from a list of the current box office estimates.
		/// </summary>
		/// <param name="movies"></param>
		/// <returns></returns>
		private IMovieListModel PerfectPick(IEnumerable<IMovie> movies)
		{
			// Don't need to use clones, because the BONUS is always used for best possible values.

			_moviePicker.EnableBestPerformer = true;
			_moviePicker.AddMovies(_minerModel.Miners[MinerModel.FML_INDEX].Movies);

			return new MovieListModel()
			{
				ComparisonHeader = "Perfect Pick (estimated)",
				ComparisonMovies = movies,
				Picks = new List<IMovieList> { _moviePicker.ChooseBest() }
			};
		}

		/// <summary>
		/// Create a QueryString from the model.  (BoxOffice / bo - Weight List / wl)
		/// </summary>
		/// <returns>QueryString</returns>
		private string QueryStringFromModel()
		{
			var stringBuilder = new StringBuilder();
			bool first = true;

			stringBuilder.Append("bo=");

			foreach (var movie in _minerModel.Miners[MinerModel.MY_INDEX].Movies)
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

			// Skip the first AND last miner.

			first = true;
			stringBuilder.Append("&wl=");

			for (int idx = MinerModel.MY_INDEX; idx < _minerModel.Miners.Count - 1; idx++)
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
		/// Initialize the view model and ViewBag for the default tweet and twitter summary card (OG) information.
		/// </summary>
		/// <param name="bonusOn"></param>
		/// <param name="viewModel"></param>
		/// <param name="picks"></param>
		private void SetupSharedTweet(bool bonusOn, SharePicksViewModel viewModel, IMovieList picks)
		{
			// Ordering by Cost is the same sort as the file names.
			var leadingMovieName = picks.Movies.OrderByDescending(movie => movie.Cost).FirstOrDefault()?.Name;
			var bonusMovieName = bonusOn ? picks.Movies.FirstOrDefault(movie => movie.IsBestPerformer)?.Name : null;
			var spentBux = picks.Movies.Sum(movie => movie.Cost);
			var spentBuxText = spentBux == 1000 ? " and spent all my BUX" : $" and spent {spentBux} BUX";
			var minerPick = MinerWeightToMiner();
			var lineupArticle = minerPick == null ? "my" : "the";
			var subTitle = bonusOn ? "Bonus ON" : "Bonus OFF";
			decimal? tiebreaker = null;

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

			var miners = _viewModel.Miners.ToList();

			if (minerPick == null && miners[MinerModel.MY_INDEX].Movies.Any())
			{
				tiebreaker = miners[MinerModel.MY_INDEX].Movies[0].EarningsBase;
			}
			else if (minerPick.Movies.Any())
			{
				tiebreaker = minerPick.Movies[0].EarningsBase;
			}

			bonusMovieName = (bonusOn) ? $", counting on {bonusMovieName} as the bonus movie" : $", hoping for {bonusMovieName} as the bonus movie";

			viewModel.TwitterDescription = $"{leadingMovieName} leads {lineupArticle} lineup{bonusMovieName}{spentBuxText}.";
			viewModel.TwitterImageFileName = viewModel.ImageFileName?.Replace("Shared_", "Twitter_");
			viewModel.TwitterTitle = $"{Constants.APPLICATION_NAME}: {subTitle} (Est ${picks.TotalEarnings:N0})";

			var defaultTwitterText = minerPick == null ? "Check out my @fml_movies picks:" : $"If you're {minerPick.Name} @{minerPick.TwitterID} your @fml_movies picks are:";

			defaultTwitterText += NEW_LINE_HTML + picks.ToString();

			if (tiebreaker.HasValue)
			{
				defaultTwitterText += $"{NEW_LINE_HTML}(${tiebreaker:N0} {EmojiConstants.NECKTIE})";
			}

			defaultTwitterText += $"{NEW_LINE_HTML}[cost {spentBux.ToString("N0")} BUX]{NEW_LINE_HTML}{NEW_LINE_HTML}#ShowYourScreens @SpilledMilkCOM RT if you like this #PerfectPick";

			ControllerUtility.SetTwitterCard(ViewBag, "summary_large_image"
											, viewModel.TwitterTitle
											, viewModel.TwitterDescription
											, viewModel.TwitterImageFileName == null ? null : $"{Constants.WEBSITE_URL}/images/{viewModel.TwitterImageFileName}"
											, "Collage of my movie lineups."
											, defaultTwitterText);

			ControllerUtility.SetOpenGraph(ViewBag, Request);
		}

		private string SharedPicksFromModels()
		{
			return $"{TrimParameters(Request.Url.ToString())}?{QueryStringFromModel()}";
		}

		/// <summary>
		/// Effectively a CAR() of the request paramters (just the URL WITHOUT the parameters).
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private string TrimParameters(string request)
		{
			var paramsIdx = request.IndexOf("?");

			if (paramsIdx > 0)
			{
				request = request.Substring(0, paramsIdx);
			}

			return request;
		}

		private PicksViewModel UpdatePicksViewModel(PicksViewModel viewModel)
		{
			// TODO: Use reflection...

			int index = 1;

			viewModel.Weight1 = _minerModel.Miners[index++].Weight;
			viewModel.Weight2 = _minerModel.Miners[index++].Weight;
			viewModel.Weight3 = _minerModel.Miners[index++].Weight;
			viewModel.Weight4 = _minerModel.Miners[index++].Weight;
			viewModel.Weight5 = _minerModel.Miners[index++].Weight;
			viewModel.Weight6 = _minerModel.Miners[index++].Weight;
			viewModel.Weight7 = _minerModel.Miners[index++].Weight;

			var myBoxOffice = _minerModel.Miners[MinerModel.MY_INDEX].Movies;

			index = 0;

			if (myBoxOffice != null && myBoxOffice.Count >= 15)
			{
				viewModel.BoxOffice1 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice2 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice3 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice4 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice5 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice6 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice7 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice8 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice9 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice10 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice11 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice12 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice13 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice14 = myBoxOffice[index++].EarningsBase;
				viewModel.BoxOffice15 = myBoxOffice[index++].EarningsBase;
			}

			return viewModel;
		}

		/// <summary>
		/// Update the ViewModel from the MinerModel.
		/// </summary>
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

			var myBoxOffice = _minerModel.Miners[MinerModel.MY_INDEX].Movies;

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

		private string WeightListFromCounter(int minerIndex)
		{
			var result = new StringBuilder();

			result.Append("wl=0");

			for (int counter = 0; counter < DATA_MINER_COUNT; counter++)
			{
				if (counter == minerIndex)
				{
					result.Append(",1");
				}
				else
				{
					result.Append(",0");
				}
			}

			return result.ToString();
		}
	}
}