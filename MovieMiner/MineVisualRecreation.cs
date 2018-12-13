using System;
using System.Collections.Generic;

using MoviePicker.Common.Interfaces;
using MoviePicker.Common;

namespace MovieMiner
{
	public class MineVisualRecreation : MinerBase
	{
		private const string DEFAULT_URL = "https://www.youtube.com/channel/UCy3oMGqYJiPWQ030mYbaZig";
		private const decimal MBAR = 1000000;		// The Roman numeral M with a bar over it is a million.

		private bool _mineData = false;
		//private bool _mineData = true;

		public MineVisualRecreation()
			: base("Visual Recreation", "Vis Rec", DEFAULT_URL)
		{
			TwitterID = "VisRecVids";
		}

		public override IMiner Clone()
		{
			var result = new MineVisualRecreation();

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
				var weekend = new DateTime(2018, 12, 16);
				UrlSource = "https://twitter.com/VisRecVids/status/1065291372851314689";
				return new List<IMovie>
						{
								new Movie { MovieName = "SpiderMan Into the SpiderVerse", Earnings = 41 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Mule", Earnings = 18 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Mortal Engines", Earnings = 10 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Grinch", Earnings = 11.3m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Ralph Breaks the Internet", Earnings = 9.2m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Once Upon a Deadpool", Earnings = 5.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Creed II", Earnings = 5 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 4 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Favourite", Earnings = 3.3m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Instant Family", Earnings = 2.8m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Fantastic Beasts The Crimes of Grindelwald", Earnings = 2.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Green Book", Earnings = 3.1m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Robin Hood", Earnings = 1.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Widows", Earnings = 1.2m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "A Star Is Born", Earnings = 1.2m * MBAR, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			return null;
		}
	}
}