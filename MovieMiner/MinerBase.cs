using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class MinerBase : IMiner
	{
		protected MinerBase(string name, string abbr, string url)
		{
			Abbreviation = abbr;
			Name = name;
			Url = url;
			Weight = 1;
		}

		public string Abbreviation { get; private set; }

		public string Name { get; private set; }

		public string Url { get; private set; }

		public int Weight { get; set; }

		public abstract List<IMovie> Mine();

		protected decimal ParseEarnings(string earnings)
		{
			decimal result = 0m;
			decimal multiplier = 1m;

			earnings = earnings.ToLower();

			if (earnings.Contains("m"))
			{
				multiplier = 1000000m;

				earnings = earnings.Replace("m", string.Empty);
			}
			else if(earnings.Contains("k"))
			{
				multiplier = 1000m;

				earnings = earnings.Replace("k", string.Empty);
			}

			earnings = earnings.Replace("$", string.Empty);

			if (decimal.TryParse(earnings, out result))
			{
				result *= multiplier;
			}
			else
			{
				result = 0;
			}

			return result;
		}

		protected string RemovePunctuation(string text)
		{
			// Map possible punctuation within words.
			text = text.Replace(" and ", " & ");

			return Regex.Replace(text, "[^\\w\\s]", string.Empty).Replace("-", string.Empty).Trim();
		}
	}
}