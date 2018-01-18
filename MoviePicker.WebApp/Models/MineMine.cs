using System.Collections.Generic;
using MoviePicker.Common.Interfaces;
using MoviePicker.WebApp.Models;

namespace MovieMiner
{
	/// <summary>
	/// This holds the weighted numbers
	/// </summary>
	public class MineMine : MinerBase
	{
		// A collection of ALL of the miners including this one.
		private readonly MinerModel _model;

		public MineMine(MinerModel model)
			: base("My Predictions", "Mine", null)
		{
			_model = model;
			CacheConfiguration = null;		// Always load this (it's ALWAYS expired)
		}

		public override IMiner Clone()
		{
			var result = new MineMine(_model);

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			return _model.CreateWeightedList();
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}