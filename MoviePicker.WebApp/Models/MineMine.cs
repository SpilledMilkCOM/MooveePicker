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
		private readonly MinerModel _model;

		public MineMine(MinerModel model)
			: base("My Predictions", "Mine", null)
		{
			_model = model;
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();

			Movies = _model.CreateWeightedList();

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}