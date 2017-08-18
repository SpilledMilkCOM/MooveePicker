using System;
using System.Diagnostics;

using MoviePicker.Common.Interfaces;

namespace MoviePicker.Tests
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