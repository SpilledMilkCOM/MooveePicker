using System;

namespace SM.Common.Interfaces
{
	public interface ILogger
	{
		ConsoleColor ForegroundColor { get; set; }

		void WriteLine(string message);
	}
}