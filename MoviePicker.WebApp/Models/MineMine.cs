using System.Collections.Generic;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Interfaces;

namespace MovieMiner
{
	/// <summary>
	/// This holds the weighted numbers
	/// </summary>
	public class MineMine : MinerBase
	{
		public MineMine(IMinerModel model)
			: base("My Predictions", "Mine", null)
		{
			Model = model;
			CacheConfiguration = null;		// Always load this (it's ALWAYS expired)
		}

		/// <summary>
		/// A collection of ALL of the miners including this one.
		/// Need to be able to inject the model into the clone.
		/// </summary>
		public IMinerModel Model { get; set; }

		public override IMiner Clone()
		{
			var result = new MineMine(Model);

			Clone(result);

			// Since the movies are the same, but in a new list these movies need to be cloned so they can be overwritten and NOT overwrite the static miner.

			var movies = new List<IMovie>();

			// Build a new list of cloned movies and then reset the movies to the clones.

			result.Movies.ForEach(movie => movies.Add(movie.Clone()));
			result.Movies = movies;

			return result;
		}

		public override void Expire()
		{
			base.Expire();

			// Don't keep the old data.

			Movies = null;
		}

		public override List<IMovie> Mine()
		{
			return Model.CreateWeightedList();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}