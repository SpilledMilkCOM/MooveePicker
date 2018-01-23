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

			return result;
		}

		public override List<IMovie> Mine()
		{
			return Model.CreateWeightedList();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}