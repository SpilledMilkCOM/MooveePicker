using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Services;
using MoviePicker.Common;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Msf
{
    public class MsfMovieSolver:IMoviePicker
    {
        private readonly List<MsfMovieWrapper> _movies;
        private decimal PenaltyForUnusedScreens { get; set; }
        private bool DisplayDebugMessage { get; set; }

        public MsfMovieSolver()
        {
            _movies = new List<MsfMovieWrapper>();
            
            AvailableScreens = 8;
            AvailableFmlBux = 1000;
            PenaltyForUnusedScreens = 2;
            DisplayDebugMessage = false;


        }

        public IEnumerable<IMovie> Movies => _movies;

        public int AvailableScreens { get; set; }

        public int AvailableFmlBux { get; set; }

        public int TotalComparisons { get; set; }

        public int TotalSubProblems { get; set; }

        public void AddMovies(IEnumerable<IMovie> movies)
        {
            _movies.Clear();

            foreach (var movie in movies)
            {
                _movies.Add(new MsfMovieWrapper(movie));
            }
        }

        public IMovieList ChooseBest()
        {
            var context = CreateSolver();

            Solution solution = context.Solve(new SimplexDirective());

            var decision = solution.Decisions.First();
            var fmlBuxUsed = 0;
            decimal estimatedBoxOfficeTotal = 0;
            var screensUsed = 0;
            Dictionary<string, MsfMovieWrapper> movies = _movies.ToDictionary(p => p.Name);

            MovieList cinePlexMovies = new MovieList();
            foreach (var v in decision.GetValues())
            {
                int count = Convert.ToInt32(v.GetValue(0));
                var title = Convert.ToString(v.GetValue(1));
                var movie = movies[title];
                if (count > 0)
                {
                    screensUsed += count;
                    fmlBuxUsed += (int)(count * movie.Cost);
                    estimatedBoxOfficeTotal += (count * movie.Earnings);
                    for (int i = 0; i < count; i++)
                    {
                        cinePlexMovies.Add(movie);
                    }
                }
            }
            decimal penalty = (AvailableScreens - screensUsed) * PenaltyForUnusedScreens;
            if (DisplayDebugMessage)
            {
                Console.WriteLine(
                    $"Total Estimated BoxOffice:\t${estimatedBoxOfficeTotal:n} M Total FmlBux spent: ${fmlBuxUsed:n} Screens utilized: {screensUsed} Total Penalty: ${penalty:n} M");
                Console.WriteLine($"Cineplex Total: \t\t\t\t\t\t\t\t\t${estimatedBoxOfficeTotal - penalty:n} M");

                Console.WriteLine();
                Console.WriteLine();

                Report report = solution.GetReport();
                Console.WriteLine(report.ToString());
            }
            return cinePlexMovies;
        }

        private SolverContext CreateSolver()
        {
            SolverContext context = new SolverContext();
            Model model = context.CreateModel();

            Set movieSet = new Set(Domain.Any, "movies");
            Decision numberOfScreensToPlayMovieOn = new Decision(Domain.IntegerNonnegative, "numberOfScreensToPlayMovieOn",
                movieSet);
            model.AddDecision(numberOfScreensToPlayMovieOn);
            Parameter estimatedEarnings = new Parameter(Domain.RealNonnegative, "EstimatedBoxOfficeRevenue", movieSet);
            estimatedEarnings.SetBinding(_movies, "EarningsAsDouble", "Name");

            Parameter fmlBux = new Parameter(Domain.IntegerNonnegative, "FmlBux", movieSet);
            fmlBux.SetBinding(_movies, "CostAsDouble", "Name");
            model.AddParameters(estimatedEarnings, fmlBux);

            //constraints: 2
            //1: number of screens <= AvailableScreens
            //2: available money to spend <= AvailableFmlBux
            model.AddConstraint("cinePlexFmlBuxConstraint",
                Model.Sum(Model.ForEach(movieSet, t => numberOfScreensToPlayMovieOn[t]*fmlBux[t])) <= AvailableFmlBux);
            model.AddConstraint("cinePlexScreensConstraint",
                Model.Sum(Model.ForEach(movieSet, t => numberOfScreensToPlayMovieOn[t])) <= AvailableScreens);

            //goal:
            //maximize earnings. earnings = selectedMovies.Sum(Estimated Earnings) - ((AvailableScreens - selectedMovies.Count) * Penalty)
            var revenueTerm = Model.Sum(Model.ForEach(movieSet, t => numberOfScreensToPlayMovieOn[t]*estimatedEarnings[t]));
            var penaltyTerm = Model.Product(-(double)PenaltyForUnusedScreens,
                Model.Difference(8, Model.Sum(Model.ForEach(movieSet, t => numberOfScreensToPlayMovieOn[t]))));
            model.AddGoal("cinePlexMaximizeRevenueMinimizeUnusedScreens", GoalKind.Maximize, Model.Sum(revenueTerm, penaltyTerm));
            return context;
        }
    }
}
