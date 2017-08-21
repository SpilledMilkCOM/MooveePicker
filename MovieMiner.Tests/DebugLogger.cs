using System;
using System.Diagnostics;

using MoviePicker.Common.Interfaces;

namespace MovieMiner.Tests
{
	public class DebugLogger : ILogger
	{
		public ConsoleColor ForegroundColor { get; set; }

		public void WriteLine(string message)
		{
			Debug.WriteLine(message);
		}
	}
}