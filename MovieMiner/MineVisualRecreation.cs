using System;
using System.Collections.Generic;

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineVisualRecreation : MinerBase
	{
		private const string DEFAULT_URL = "https://www.youtube.com/channel/UCy3oMGqYJiPWQ030mYbaZig";

		private bool _mineData = false;
		//private bool _mineData = true;

		public MineVisualRecreation()
			: base("Visual Recreation", "Vis Rec", DEFAULT_URL)
		{
			TwitterID = "BoxOffice";
		}

		public override IMiner Clone()
		{
			var result = new MineBoxOfficePro();

			Clone(result);

			return result;
		}

		public override List<IMovie> Mine()
		{
			if (_mineData)
			{
				return MineData();
			}
			else
			{
				var weekend = new DateTime(2018, 11, 11);
				UrlSource = "https://www.youtube.com/watch?v=Ahav1u-rDlo";
				return new List<IMovie>
					{
						new Movie { MovieName = "The Grinch", Earnings = 71000000, WeekendEnding = weekend },
						new Movie { MovieName = "Bohemian Rhapsody", Earnings = 32000000, WeekendEnding = weekend },
						new Movie { MovieName = "The Girl in the Spiders Web", Earnings = 10500000, WeekendEnding = weekend },
						new Movie { MovieName = "The Nutcracker  the Four Realms", Earnings = 10800000, WeekendEnding = weekend },
						new Movie { MovieName = "Overlord", Earnings = 8000000, WeekendEnding = weekend },
						new Movie { MovieName = "A Star Is Born", Earnings = 7400000, WeekendEnding = weekend },
						new Movie { MovieName = "Nobodys Fool", Earnings = 6300000, WeekendEnding = weekend },
						new Movie { MovieName = "Venom", Earnings = 5000000, WeekendEnding = weekend },
						new Movie { MovieName = "Halloween", Earnings = 4300000, WeekendEnding = weekend },
						new Movie { MovieName = "Smallfoot", Earnings = 2000000, WeekendEnding = weekend },
					};
			}
		}

		private List<IMovie> MineData()
		{
			return null;
		}
	}
}