using System;

namespace SM.Common.Utils
{
	/// <summary> This class contains methods to help with dealing with elapsed time.
	/// Author:        Parker Smart
	/// Created:       04/01/2004
	/// </summary>
	public class ElapsedTime
	{
		private readonly char PADDING_CHAR = Convert.ToChar("0");

		protected bool _autoUpdate = true;
		protected DateTime _current;
		protected DateTime _started;
		protected Int32 _currentCount = 0;
		protected Int32 _maxCount = 0;

		public ElapsedTime()
		{
			Start();
		}

		/// <summary> This method is used to Get/Set the Auto Update flag.
		/// </summary>
		public bool AutoUpdate
		{
			get
			{
				return _autoUpdate;
			}
			set
			{
				_autoUpdate = value;
			}
		}

		/// <summary> This method is used to Get/Set the Current Item.
		/// </summary>
		public int Current
		{
			get
			{
				return _currentCount;
			}
			set
			{
				_currentCount = value;
			}
		}

		/// <summary> This method is used to Get/Set the Elapsed TimeSpan.
		/// </summary>
		public TimeSpan Elapsed
		{
			get
			{
				if (_autoUpdate)
				{
					_current = DateTime.Now;
				}
				return _current.Subtract(_started);
			}
		}

		public TimeSpan ElapsedSinceLastElapsed
		{
			get
			{
				DateTime lastCurrent = _current;

				if (_autoUpdate)
				{
					_current = DateTime.Now;
				}
				return _current.Subtract(lastCurrent);
			}
		}

		/// <summary> This method is used to calculate Items Per Second.
		/// </summary>
		public double ItemsPerSecond
		{
			get
			{
				TimeSpan diff = Elapsed;
				double dResult = 0;

				if (diff.Ticks > 0 && _currentCount > 0)
				{
					double iSeconds = (Convert.ToDouble(DateTime.Now.Subtract(_started).Ticks) / TimeSpan.TicksPerSecond);

					dResult = Convert.ToDouble(_currentCount) / iSeconds;
				}

				return dResult;
			}
		}

		/// <summary> This method is used to Get/Set the Max Items.
		/// </summary>
		public int Max
		{
			get
			{
				return _maxCount;
			}
			set
			{
				_maxCount = value;
			}
		}

		/// <summary> This method is used to calculate ESTIMATED Remaining time.
		/// </summary>
		public TimeSpan Remaining
		{
			get
			{
				if (_currentCount != 0)
				{
					TimeSpan diff = Elapsed;

					return new TimeSpan(Convert.ToInt64(((double)diff.Ticks * _maxCount / _currentCount)) - diff.Ticks);
				}
				return new TimeSpan(0);
			}
		}

		/// <summary> This method is used to Get/Set the Started DateTime.
		/// </summary>
		public DateTime Started
		{
			get
			{
				return _started;
			}
			set
			{
				_started = value;
			}
		}

		/// <summary> This method is used to mark the time started.
		/// </summary>
		public void Start()
		{
			_started = DateTime.Now;
			_current = _started;
		}

		/// <summary> This method is used to mark the time started.
		/// </summary>
		public void Stop()
		{
			AutoUpdate = false;
			Update();
		}

		/// <summary> This method is used to return a formatted remaining time.
		/// </summary>
		public string FormatRemaining()
		{
			string result = "(~ ??:??:??)";

			if (_currentCount != 0)
			{
				TimeSpan diff = Remaining;

				result = string.Format("(~ {0}:{1}:{2})"
								, diff.Hours.ToString().PadLeft(2, PADDING_CHAR)
								, diff.Minutes.ToString().PadLeft(2, PADDING_CHAR)
								, diff.Seconds.ToString().PadLeft(2, PADDING_CHAR));
			}

			return result;
		}

		/// <summary> This method is used to return a formatted elapsed time.
		/// </summary>
		public override string ToString()
		{
			TimeSpan diff = Elapsed;

			return (diff.Hours.ToString().PadLeft(2, PADDING_CHAR) + (":"
						+ (diff.Minutes.ToString().PadLeft(2, PADDING_CHAR) + (":"
						+ (diff.Seconds.ToString().PadLeft(2, PADDING_CHAR) + ("." + diff.Milliseconds.ToString().PadLeft(3, PADDING_CHAR)))))));
		}

		/// <summary> This method is used to refresh the current timestamp.
		/// </summary>
		public void Update()
		{
			_current = DateTime.Now;
		}
	}
}
