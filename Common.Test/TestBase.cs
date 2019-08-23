using SM.Common.Interfaces;
using SM.Common.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace SM.Common.Tests
{
	public abstract class TestBase
	{
		private ILogger _logger;

		/// <summary>
		/// This is public so it can be modified from the TestHarness.
		/// </summary>
		public abstract IUnityContainer UnityContainer { get; }

		protected ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					lock (this)
					{
						if (_logger == null)
						{
							AddDefaultLogger();

							_logger = UnityContainer.Resolve<ILogger>();
						}
					}
				}
				return _logger;
			}
		}

		public void AddDefaultLogger()
		{
			if (UnityContainer.Registrations.FirstOrDefault(registration => registration.RegisteredType == typeof(ILogger)) == null)
			{
				// Register the DebugLogger if the interface is not defined.
				UnityContainer.RegisterType<ILogger, DebugLogger>();
			}
		}
	}
}