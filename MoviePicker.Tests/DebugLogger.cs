using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MoviePicker.Common.Interfaces;

namespace MoviePicker.Tests
{
	[ExcludeFromCodeCoverage]
	public class DebugLogger : ILogger
	{
		public ConsoleColor ForegroundColor { get; set; }

		public void WriteLine(string message)
		{
			Debug.WriteLine(message);
		}
	}
}