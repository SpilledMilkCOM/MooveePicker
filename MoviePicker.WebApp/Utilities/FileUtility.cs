using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoviePicker.WebApp.Utilities
{
	public static class FileUtility
	{
		private static bool _isCleaningUp = false;
		private static object _isCleaningUpLock = new object();

		private static DateTime? Expiration { get; set; }

		/// <summary>
		/// Every so often clean up the files.
		/// </summary>
		public static void CleanupFiles()
		{
			if (ShouldCleanup())
			{
				// Loop through the MoviePosters

				// Loop through the Shared images.
			}
		}

		/// <summary>
		/// Download a list of files.
		/// </summary>
		/// <param name="files">A list of file Urls</param>
		/// <param name="localFilePrefix">Full path plus local file prefix</param>
		public static void DownloadFiles(IEnumerable<string> files, string localFilePrefix)
		{
			// TODO: Support files with the same name, but on different base Urls

			// Loop through the files.
			// Don't need to download or verify the file multiple times.

			foreach (var fileUrl in files.Distinct())
			{
				var localFile = LocalFile(fileUrl, localFilePrefix);

				// Verify the file doesn't already exist.

				if (!File.Exists(localFile))
				{
					HttpRequestUtility.DownloadFile(fileUrl, localFile);
				}
			}
		}

		public static List<string> LocalFiles(IEnumerable<string> files, string localFilePrefix)
		{
			var result = new List<string>();

			foreach (var fileUrl in files)
			{
				result.Add(LocalFile(fileUrl, localFilePrefix));
			}

			return result;
		}

		private static string LocalFile(string fileUrl, string localFilePrefix)
		{
			return $"{localFilePrefix}{Path.GetFileName(fileUrl)}";
		}

		/// <summary>
		/// Thread safe check to see if this miner should load.
		/// </summary>
		/// <returns></returns>
		private static bool ShouldCleanup()
		{
			bool result = false;

			if ((DateTime.Now > Expiration || Expiration == null) && !_isCleaningUp)
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