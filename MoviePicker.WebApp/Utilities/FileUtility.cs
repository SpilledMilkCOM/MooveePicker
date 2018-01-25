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

			if (count > 0 && count < fileCount)
			{
				var random = new Random();

				while (result.Count < count)
				{
					var index = random.Next(0, fileCount - 1);

					AddFileName(result, Path.GetFileName(files[index]), thumbPath);
				}
			}
			else
			{
				foreach (var file in files)
				{
					AddFileName(result, Path.GetFileName(file), thumbPath);
				}
			}

			return result;
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

		private static void AddFileName(List<string> list, string fileName, string thumbPath)
		{
			// Make sure the thumbnail exists before the image is added.

			if (File.Exists($"{thumbPath}{Path.DirectorySeparatorChar}{fileName}")
			&& !list.Contains(fileName))
			{
				list.Add(fileName);
			}
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