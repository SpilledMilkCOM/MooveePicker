﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MoviePicker.Common.Interfaces;

namespace MovieMiner
{
	public abstract class MinerBase : IMiner, ICache
	{
		// To make this thread safe so the miners can become singletons and shared across threads/requests.
		private readonly object _isLoadingLock;
		private readonly object _errorLock;
		private readonly object _movieLock;

		// The list of movies could be loaded on another thread.
		private volatile bool _isLoading;
		private volatile List<IMovie> _movies;
		private volatile string _error;

		protected MinerBase(string name, string abbr, string url)
		{
			_errorLock = new object();
			_isLoadingLock = new object();
			_movieLock = new object();
			_movies = new List<IMovie>();

			Abbreviation = abbr?.Trim();
			CacheConfiguration = new CacheConfiguration();              // Just take the default for now.
			Expiration = DateTime.Now.Subtract(new TimeSpan(1));        // This will trigger the first load.
			IsHidden = false;
			OkToMine = true;
			Name = name;
			Url = url;
			UrlSource = url;
		}

		public string Abbreviation { get; private set; }

		public ICacheConfiguration CacheConfiguration { get; protected set; }

		public bool CloneCausedReload { get; private set; }

		public IMovie CompoundMovie { get { return Movies.FirstOrDefault(movie => movie.Day.HasValue); } }

		public decimal CompoundTotal { get { return Movies.Where(movie => movie.Day.HasValue).Sum(matchedMovie => matchedMovie.EarningsBase); } }

		/// <summary>
		/// A thread safe version of setting the Error (the Error can be set in the Loading thread or when filtering)
		/// </summary>
		public string Error
		{
			get
			{
				string result = null;

				lock (_errorLock)
				{
					// Return a copy of the error
					result = _error;
				}

				return result;
			}
			set
			{
				lock (_errorLock)
				{
					_error = value;
				}
			}
		}

		public DateTime? Expiration { get; private set; }

		public bool IsHidden { get; set; }

		public DateTime? LastLoaded { get; private set; }

		/// <summary>
		/// The list of movies is a critical path so there are locks around the get/set
		/// </summary>
		public List<IMovie> Movies
		{
			get
			{
				List<IMovie> result = null;

				lock (_movieLock)
				{
					// Return a copy of the movie list so that list cannot be messed with.

					if (_movies != null)
					{
						result = new List<IMovie>(_movies);
					}
				}

				return result;
			}
			private set
			{
				lock (_movieLock)
				{
					_movies = value;
				}
			}
		}

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

		public void Expire()
		{
			// Assign the exipration to the past versus Now so that it's guaranteed to load the next time.
			Expiration = DateTime.Now.Subtract(new TimeSpan(1));
		}

		/// <summary>
		/// Only load the data if it has expired.
		/// This is thread safe and WON'T mine any data if not needed or already in the process of being mined.
		/// </summary>
		public void Load()
		{
			if (ShouldLoad())
			{
				// _isLoading was set to true in the above thread safe method.

				try
				{
					Error = string.Empty;

					Movies = Mine();

					CloneCausedReload = true;
					LastLoaded = DateTime.Now;
				}
				finally
				{
					if (CacheConfiguration != null)
					{
						if (Movies.Count > 0)
						{
							Expiration = LastLoaded.Value.Add(CacheConfiguration.Duration);
						}
						else
						{
							Expiration = LastLoaded.Value.Add(CacheConfiguration.EmptyDuration);
						}
					}

					lock (_isLoadingLock)
					{
						_isLoading = false;
					}
				}
			}
		}

		public abstract List<IMovie> Mine();

		public void SetMovies(List<IMovie> movies)
		{
			Movies = movies;
		}

		//----==== PROTECTED METHODS ====----------------------------------------------------------

		/// <summary>
		/// Copy all of the base stuff into the concrete clone passed in.
		/// </summary>
		/// <param name="clone"></param>
		/// <returns></returns>
		protected IMiner Clone(MinerBase clone)
		{
			CloneCausedReload = false;

			// Make sure the source data is all up to date.
			// If this is the thread loading then you will wait before you clone it.
			// TODO: Possibly thread out the loading of the data.

			Load();

			if (CloneCausedReload)
			{
				clone.CloneCausedReload = true;
				CloneCausedReload = false;
			}

			// Copy and fill in all of the base goodness.

			clone.Abbreviation = Abbreviation;
			clone.CacheConfiguration = CacheConfiguration;
			clone.Expiration = Expiration;
			clone.IsHidden = IsHidden;
			clone.LastLoaded = LastLoaded;
			clone.Name = Name;
			clone.OkToMine = false;         // This is the clone of the singleton, this should prevent any reloading
			clone.Url = Url;
			clone.UrlSource = UrlSource;
			clone.Weight = Weight;          // You get the default.  :)

			// Set during the Mine() method.

			clone.Error = Error;

			// Create a NEW list of movies, the movie objects are still shared between this object and the cloned object.
			// (you can't just assign the list over otherwise the list will be shared too and you'll get interation problems amongst the threads)

			clone.Movies = new List<IMovie>(Movies);

			return clone;
		}

		protected string MapName(string name)
		{
			var result = name;

			if (result != null)
			{
				// BO Prophet does NOT know how to spell.

				result = result.Replace("Juamnji", "Jumanji");
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

		protected string RemoveStudio(string name)
		{
			var result = name;

			// Remove the studio that's in parenthesis
			var parenIndex = result.IndexOf("(");

			if (parenIndex > 0)
			{
				result = result.Substring(0, parenIndex).Trim();
			}

			return result;
		}

		/// <summary>
		/// Thread safe check to see if this miner should load.
		/// </summary>
		/// <returns></returns>
		private bool ShouldLoad()
		{
			bool result = false;

			if ((DateTime.Now > Expiration || Expiration == null) && !_isLoading)
			{
				lock (_isLoadingLock)
				{
					// _isLoading will already be set to true for the losing thread.

					if (!_isLoading)
					{
						// This thread wins and returns true;

						_isLoading = true;
						result = true;
					}
				}
			}

			return result;
		}
	}
}