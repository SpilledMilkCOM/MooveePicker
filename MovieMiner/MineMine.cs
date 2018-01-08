using System.Collections.Generic;
using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	/// <summary>
	/// This holds the weighted numbers
	/// </summary>
	public class MineMine : MinerBase
	{
		public MineMine()
			: base("My Predictions", "Mine", null)
		{
		}

		public override List<IMovie> Mine()
		{
			var result = new List<IMovie>();

			Movies = result;

			return result;
		}

		//----==== PRIVATE ====--------------------------------------------------------------------
	}
}