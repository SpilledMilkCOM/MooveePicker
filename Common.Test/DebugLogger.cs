using System;
using System.Diagnostics;

using SM.Common.Interfaces;

namespace SM.Common.Tests
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