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
				var weekend = new DateTime(2018, 12, 2);
				UrlSource = "https://twitter.com/VisRecVids/status/1067884398081687554";
				return new List<IMovie>
						{
								new Movie { MovieName = "Ralph Breaks the Internet", Earnings = 30.8m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Grinch", Earnings = 18.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Creed II", Earnings = 19.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Fantastic Beasts The Crimes of Grindelwald", Earnings = 12 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Bohemian Rhapsody", Earnings = 8 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Instant Family", Earnings = 6.6m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Robin Hood", Earnings = 4 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Widows", Earnings = 3.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Green Book", Earnings = 3.5m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Possession of Hannah Grace", Earnings = 0 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "A Star Is Born", Earnings = 1.7m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Nutcracker  the Four Realms", Earnings = 1.3m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "The Favourite", Earnings = 1.1m * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Boy Erased", Earnings = 1 * MBAR, WeekendEnding = weekend },
								new Movie { MovieName = "Nobodys Fool", Earnings = 0.4m * MBAR, WeekendEnding = weekend },
						};
			}
		}

		private List<IMovie> MineData()
		{
			return null;
		}
	}
}