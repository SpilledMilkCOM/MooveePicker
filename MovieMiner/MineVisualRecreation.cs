using System;
using System.Collections.Generic;

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineVisualRecreation : MinerBase
	{
		private const string DEFAULT_URL = "https://www.youtube.com/channel/UCy3oMGqYJiPWQ030mYbaZig";
		private const decimal MBAR = 1000000;

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
				var weekend = new DateTime(2018, 11, 25);
				UrlSource = "https://twitter.com/VisRecVids/status/1065292217969979392";
				return new List<IMovie>
						{
								new Movie { MovieName = "Ralph Breaks the Internet", Earnings = 53 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Creed II", Earnings = 32 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Fantastic Beasts The Crimes of Grindelwald", Earnings = 28 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Grinch", Earnings = 29 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 12 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Instant Family", Earnings = 10 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Widows", Earnings = 7.4m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Robin Hood", Earnings = 8.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Green Book", Earnings = 4 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "A Star Is Born", Earnings = 3.1m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Nutcracker  the Four Realms", Earnings = 3 * MBAR, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			return null;
		}
	}
}