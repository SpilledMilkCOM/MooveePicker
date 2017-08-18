using System;

using MoviePicker.Common.Interfaces;

namespace MoviePicker.Tests
{
	public class StandardOutLogger : ILogger
	{
		public ConsoleColor ForegroundColor
		{
			get { return Console.ForegroundColor; }
			set { Console.ForegroundColor = value; }
		}

		public void WriteLine(string message)
		{
			Console.WriteLine(message);
		}
	}
}