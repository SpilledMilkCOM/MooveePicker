﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MoviePicker.WebApp.Utilities
{
	public class HttpRequestUtility
	{
		//private const int BUFFER_SIZE = 1024 * 64;
		private const int BUFFER_SIZE = 1024 * 1024;    // 1 MB

		private static bool _cancel = false;

		/// <summary>
		/// A cancel method to call from the progress callback if needed.
		/// </summary>
		public static void Cancel()
		{
			// TODO: Use the CancelToken
			_cancel = true;
		}

		/// <summary>
		/// Download a remote file.
		/// </summary>
		/// <param name="remoteFilename">Url to the remote file</param>
		/// <param name="localFilename">Local file name (and path) of the saved file.</param>
		/// <param name="progressCallback">A progress delegate.</param>
		/// <returns></returns>
		public static int DownloadFile(string remoteFilename, string localFilename, Action<int, long> progressCallback = null)
		{
			// Function will return the number of bytes processed
			// to the caller. Initialize to 0 here.
			int bytesProcessed = 0;

			// Assign values to these objects here so that they can
			// be referenced in the finally block
			Stream remoteStream = null;
			Stream localStream = null;
			WebResponse response = null;

			// Use a try/catch/finally block as both the WebRequest and Stream
			// classes throw exceptions upon error
			try
			{
				// Create a request for the specified remote file name
				var request = WebRequest.Create(remoteFilename) as HttpWebRequest;

				if (request != null)
				{
					request.Method = "GET";
					request.Accept = "image/*";
					request.KeepAlive = false;
					request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0";

					// Send the request to the server and retrieve the
					// WebResponse object 
					response = request.GetResponse();

					if (response != null)
					{
						// Once the WebResponse object has been retrieved,
						// get the stream object associated with the response's data
						remoteStream = response.GetResponseStream();

						var maxContentLength = response.ContentLength;

						// Create the local file
						localStream = File.Create(localFilename);

						// Allocate a buffer
						byte[] buffer = new byte[BUFFER_SIZE];
						int bytesRead;

						// Simple do/while loop to read from stream until
						// no bytes are returned
						do
						{
							// Read data (up to 1k) from the stream
							bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

							// Write the data to the local file
							localStream.Write(buffer, 0, bytesRead);

							// Increment total bytes processed
							bytesProcessed += bytesRead;

							progressCallback?.Invoke(bytesProcessed, maxContentLength);

						} while (bytesRead > 0 && !_cancel);
					}
				}
			}
			catch (WebException webEx)
			{
				// Ignore "Forbidden"

				if (webEx.Message.IndexOf("403") < 0)
				{
					throw webEx;
				}
			}
			catch (Exception ex)
			{
				throw ex;
				//Console.WriteLine(e.Message);
			}
			finally
			{
				// Close the response and streams objects here 
				// to make sure they're closed even if an exception
				// is thrown at some point

				response?.Close();
				remoteStream?.Close();
				localStream?.Close();

				if (_cancel)
				{
					File.Delete(localFilename);
				}
			}

			// Return total bytes processed to caller.
			return bytesProcessed;
		}

		public static string DownloadString(string url)
		{
			// Open the requested URL
			WebRequest req = WebRequest.Create(url);

			// Get the stream from the returned web response
			StreamReader stream = new StreamReader(req.GetResponse().GetResponseStream());

			// Get the stream from the returned web response
			var sb = new System.Text.StringBuilder();
			string strLine;

			// Read the stream a line at a time and place each one
			// into the stringbuilder
			while ((strLine = stream.ReadLine()) != null)
			{
				// Ignore blank lines
				if (strLine.Length > 0)
				{
					sb.Append(strLine);
				}
			}

			// Finished with the stream so close it now
			stream.Close();

			// Cache the streamed site now so it can be used
			// without reconnecting later
			return sb.ToString();
		}

		public static async Task<string> DownloadStringAsync(string uri)
		{
			var client = new WebClient();

			return await client.DownloadStringTaskAsync(uri);
		}
	}
}