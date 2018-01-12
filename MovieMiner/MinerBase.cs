using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class MinerBase : IMiner, ICache
	{
		/// <summary>
		/// To make this thread safe so the miners can become singletons and shared across threads/requests.
		/// </summary>
		private readonly object _loadLock;
		private volatile bool _isLoading;

		protected MinerBase(string name, string abbr, string url)
		{
			_isLoading = false;
			_loadLock = new object();

			Abbreviation = abbr;
			IsHidden = false;
			OkToMine = true;
			Name = name;
			Url = url;
			UrlSource = url;
			Weight = 1;
		}

		public string Abbreviation { get; private set; }

		public ICacheConfiguration CacheConfiguration { get; private set; }

		/// <summary>
		/// A thread safe version of 
		/// </summary>
		public string Error { get; set; }

		public DateTime? Expiration { get; private set; }

		public bool IsHidden { get; set; }

		public DateTime? LastLoaded { get; private set; }

		public List<IMovie> Movies { get; protected set; }

		public string Name { get; private set; }

		public bool OkToMine { get; set; }

		public string Url { get; private set; }

		public string UrlSource { get; protected set; }

		public decimal Weight { get; set; }

		//----==== PUBLIC METHODS ====-------------------------------------------------------------

		public void Clear()
		{
			Movies = new List<IMovie>();
		}

		public abstract IMiner Clone();

		/// <summary>
		/// Only load the data if it has expired.
		/// </summary>
		public void Load()
		{
			if (DateTime.Now > Expiration)
			{
				lock (_loadLock)
				{
					if (DateTime.Now > Expiration)
					{
						_isLoading = true;

						try
						{
							Mine();
						}
						finally
						{
							_isLoading = false;
						}

						LastLoaded = DateTime.Now;
						Expiration = LastLoaded.Value.Add(CacheConfiguration.Duration);
					}
				}
			}
		}

		public abstract List<IMovie> Mine();

		//----==== PROTECTED METHODS ====----------------------------------------------------------

		/// <summary>
		/// Copy all of the base stuff into the concrete clone passed in.
		/// </summary>
		/// <param name="clone"></param>
		/// <returns></returns>
		protected IMiner Clone(MinerBase clone)
		{
			// Copy and fill in all of the base goodness.

			// If the object is loading then you need to wait before you clone it.
			// TODO: Return stale data if loading.

			lock (_loadLock)
			{
				clone.Error = Error;

				clone.Movies = Movies;
			}

			return clone;
		}

		protected string MapName(string name)
		{
			var result = name;

			if (string.IsNullOrEmpty(name))
			{
				// @nerdguru didn't include the NAME! (only the day abbr.).  Yes, a hack, but I want the damn data.

				result = "Star Wars";
			}

			return result;
		}

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
			else if (earnings.Contains("k"))
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