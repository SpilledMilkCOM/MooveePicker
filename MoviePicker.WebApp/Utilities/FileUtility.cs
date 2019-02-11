using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoviePicker.WebApp.Utilities
{
	public static class FileUtility
	{
		// Keep the movie posters around for a while, so they will fill out the film strip better.
		private const int MOVIE_EXPIRATION_DAYS = 90;

		//private const int SHARED_EXPIRATION_MINUTES = 5;
		private const int SHARED_EXPIRATION_MINUTES = 60;			// One hour.
		private const int TWITTER_EXPIRATION_MINUTES = 24 * 60;		// One day - may want to back this off later.
		private const string MOVIE_POSTER_PREFIX = "MoviePoster_";

		private static bool _isCleaningUp = false;
		private static object _isCleaningUpLock = new object();

		static FileUtility()
		{
			NextCleanup = DateTime.Now;
		}

		private static DateTime NextCleanup { get; set; }

		public static double NextCleanupDuration => NextCleanup.Subtract(DateTime.Now).TotalMinutes;

		/// <summary>
		/// Every so often clean up the files.
		/// </summary>
		public static void CleanupFiles(string directoryPath, bool forceNow = false)
		{
			if (forceNow)
			{
				NextCleanup = DateTime.Now;
			}

			if (ShouldCleanup() && directoryPath != null)
			{
				var directory = $"{Path.GetDirectoryName(directoryPath)}{Path.DirectorySeparatorChar}";

				// Loop through the MoviePosters.

				foreach (var file in Directory.GetFiles(directory, $"{MOVIE_POSTER_PREFIX}*"))
				{
					if (file.IndexOf(".temp") > 0)
					{
						// Just delete the temp files each possible pass.

						File.Delete(file);
					}
					else if (File.GetCreationTime(file) < DateTime.Now.AddDays(MOVIE_EXPIRATION_DAYS * -1))
					{
						File.Delete(file);
					}
				}

				// Loop through the Shared images.

				foreach (var file in Directory.GetFiles(directory, "Shared_*"))
				{
					if (File.GetCreationTime(file) < DateTime.Now.AddMinutes(SHARED_EXPIRATION_MINUTES * -1))
					{
						File.Delete(file);
					}
				}

				// Loop through the Twitter images.

				foreach (var file in Directory.GetFiles(directory, "Twitter_*"))
				{
					if (File.GetCreationTime(file) < DateTime.Now.AddMinutes(TWITTER_EXPIRATION_MINUTES * -1))
					{
						File.Delete(file);
					}
				}

				NextCleanup = DateTime.Now.AddMinutes(SHARED_EXPIRATION_MINUTES);
			}
		}

		/// <summary>
		/// Download a list of files.
		/// </summary>
		/// <param name="files">A list of file Urls</param>
		/// <param name="localFilePrefix">Full path plus local file prefix</param>
		public static bool DownloadFiles(IEnumerable<string> files, string localFilePrefix)
		{
			bool filesDownloaded = false;

			// TODO: Support files with the same name, but on different base Urls

			CleanupFiles(localFilePrefix);

			// Loop through the files.
			// Don't need to download or verify the file multiple times.

			foreach (var fileUrl in files.Distinct())
			{
				var localFile = LocalFile(fileUrl, localFilePrefix);

				// Verify the file doesn't already exist.

				if (!File.Exists(localFile))
				{
					try
					{
						HttpRequestUtility.DownloadFile(fileUrl, localFile);

						filesDownloaded = true;
					}
					catch (Exception ex)
					{
						// It's accpetable to ignore the error here, the app can use the direct URL or a "not found" image.
					}
				}
			}

			return filesDownloaded;
		}

		/// <summary>
		/// Return a list of file names given the server path and filter.
		/// </summary>
		/// <param name="serverPath">Server path to directory to filter.</param>
		/// <param name="filter">The file system filter/wildcard.</param>
		/// <returns></returns>
		public static List<string> FilterImagesInPath(string serverPath, string filter, int count = 0)
		{
			var files = Directory.GetFiles(serverPath, filter);
			var fileCount = files.Length;
			var result = new List<string>();
			var addFiles = (count > 0 && count < fileCount) ? files.Take(count) : files;

			foreach (var file in addFiles)
			{
				result.Add(Path.GetFileName(file));
			}

			return result;
		}

		/// <summary>
		/// Map a file URL to a local file name (removing/replacing illegal characters)
		/// </summary>
		/// <param name="fileUrl">The url to the file (may include path)</param>
		/// <param name="localFilePrefix">Prefix of the file name to prepend.</param>
		/// <returns></returns>
		public static string LocalFile(string fileUrl, string localFilePrefix)
		{
			var localFileName = Path.GetFileName(fileUrl);
			var questionIndex = localFileName.IndexOf('?');

			// Check for illegal characters.

			if (questionIndex >= 0)
			{
				// Remove everything to the right of the '?'

				localFileName = localFileName.Substring(0, questionIndex);
			}

			return $"{localFilePrefix}{localFileName}";
		}

		/// <summary>
		/// Map file URLs to a local file names (removing/replacing illegal characters)
		/// </summary>
		/// <param name="files">The list of file urls (may include path)</param>
		/// <param name="localFilePrefix">Prefix of the file name to prepend to each local file name.</param>
		/// <returns></returns>
		public static List<string> LocalFiles(IEnumerable<string> files, string localFilePrefix)
		{
			var result = new List<string>();

			if (files != null)
			{
				foreach (var fileUrl in files)
				{
					var localFile = LocalFile(fileUrl, localFilePrefix);

					if (File.Exists(localFile))
					{
						result.Add(localFile);
					}
					else if (Path.GetExtension(localFile).ToLower() == ".jpg")
					{
						result.Add($"{Path.GetDirectoryName(localFile)}{Path.DirectorySeparatorChar}MooveePosterNotFound.jpg");
					}
				}
			}

			return result;
		}

		//----==== PRIVATE ====------------------------------------------

		/// <summary>
		/// Thread safe check to see if this miner should load.
		/// </summary>
		/// <returns></returns>
		private static bool ShouldCleanup()
		{
			bool result = false;

			if (DateTime.Now >= NextCleanup && !_isCleaningUp)
			{
				lock (_isCleaningUpLock)
				{
					// _isCleaningUp will already be set to true for the losing thread.

					if (!_isCleaningUp)
					{
						// This thread wins and returns true;

						_isCleaningUp = true;
						result = true;
					}
				}
			}

			return result;
		}
	}
}