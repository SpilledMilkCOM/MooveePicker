using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviePicker.Common.Interfaces;
using MoviePicker.Msf;
using MoviePicker.Tests;
using MoviePicker.WebApp.Models;
using SM.Common.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Unity;

namespace MoviePicker.WebApp.Tests.Models
{
	[TestClass]
	public class MinerModelSimulationTests : MoviePickerTestBase
	{
		private const int TODD_INDEX = 2;
		private const int MAX_MINERS = 6;

		// Unity Reference: https://msdn.microsoft.com/en-us/library/ff648211.aspx
		private static IUnityContainer _unity;

		public override IUnityContainer UnityContainer => _unity;


		[ClassInitialize]
		public static void InitializeBeforeAllTests(TestContext context)
		{
			_unity = new UnityContainer();

			//_unity.RegisterType<IMovie, Movie>();
			//_unity.RegisterType<IMovieList, MovieList>();
			//_unity.RegisterType<IMoviePicker, MooveePicker.MoviePicker>();
		}

		[TestMethod, TestCategory("Simulation")]
		public void MinerModelSimulation_OneStepUpDown()
		{
			Dictionary<int, int> bestListCounts = new Dictionary<int, int>();				// Keyed using the hash code.
			Dictionary<int, IMovieList> bestLists = new Dictionary<int, IMovieList>();		// Keyed using the hash code.
			ElapsedTime elapsed = new ElapsedTime();

			var moviePicker = new MsfMovieSolver { DisplayDebugMessage = false };
			var builder = new StringBuilder();
			var test = new MinerModel(true);
			var defaultWeights = CreateDefaultWeights();

			var weights = GenerateWeightLists(new List<int>(), defaultWeights);

			//foreach (var list in weights)
			//{
			//	foreach (var weight in list)
			//	{
			//		builder.Append(weight);
			//	}
			//	builder.Append("\r");
			//}

			//Debug.Write(builder);
			foreach (var list in weights)
			{
				SetWeights(test, list);

				moviePicker.AddMovies(test.CreateWeightedList());

				var best = moviePicker.ChooseBest();
				var hashCode = best.GetHashCode();
				int value;

				// Increment (or add to) the best list counts

				if (bestListCounts.TryGetValue(hashCode, out value))
				{
					bestListCounts[hashCode] = value + 1;
				}
				else
				{
					bestListCounts.Add(hashCode, 1);
					bestLists.Add(hashCode, best);
				}
			}

			// Sort through the MOST times a list is counted.

			foreach (var item in bestListCounts.OrderByDescending(item => item.Value).Take(5))
			{
				Debug.WriteLine($"Number of votes: {bestListCounts[item.Key]}/{weights.Count}");
				WriteMovies(bestLists[item.Key]);
			}
		}

		//----==== PRIVATE ====---------------------------------------------------------------------------

		private List<int> CreateDefaultWeights()
		{
			return new List<int>
			{
				3,			// Todd Thatcher
				3,			// Box Office Pro
				4,			// Box Office Mojo
				1,			// Cultured Vultures
				1,			// Box Office Prophet
				6			// Box Office Report
			};
		}

		private List<List<int>> GenerateWeightLists(List<int> beginning, List<int> end)
		{
			var result = new List<List<int>>();

			if (beginning.Count == MAX_MINERS)
			{
				result.Add(beginning);
			}
			else
			{
				var newBeginning = new List<int>(beginning);
				var newEnd = new List<int>(end);
				var currentWeight = RemoveFirst(newEnd);

				newBeginning.Add(currentWeight);

				// Use the baseline current weight

				result.AddRange(GenerateWeightLists(newBeginning, newEnd));

				newBeginning = new List<int>(beginning);

				newBeginning.Add(currentWeight + 1);

				result.AddRange(GenerateWeightLists(newBeginning, newEnd));

				if (currentWeight - 1 >= 0)
				{
					newBeginning = new List<int>(beginning);

					newBeginning.Add(currentWeight - 1);

					result.AddRange(GenerateWeightLists(newBeginning, newEnd));
				}
			}

			return result;
		}

		private int RemoveFirst(List<int> list)
		{
			int result = list.First();

			list.RemoveAt(0);

			return result;
		}

		private void SetWeights(MinerModel model, List<int> weights)
		{
			if (weights.Count != MAX_MINERS)
			{
				throw new ArgumentException($"The weight list MUST contain {MAX_MINERS} weights.");
			}

			int index = TODD_INDEX;

			foreach (var weight in weights)
			{
				model.Miners[index++].Weight = weight;
			}
		}
	}
}