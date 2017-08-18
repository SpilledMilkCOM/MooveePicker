using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NReco.PhantomJS;

namespace MovieMiner
{
	public static class HttpRequestUtil
	{
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

		public static string DownloadStringPhantomJS(string url)
		{
			var phantomJS = new PhantomJS();

			phantomJS.OutputReceived += (sender, e) => {
				Console.WriteLine("PhantomJS output: {0}", e.Data);
			};
			phantomJS.RunScript("for (var i=0; i<10; i++) console.log('hello from js '+i); phantom.exit();", null);

			return null;
		}
	}
}