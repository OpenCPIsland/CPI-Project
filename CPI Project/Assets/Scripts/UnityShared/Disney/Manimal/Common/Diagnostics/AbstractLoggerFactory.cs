using System;
using System.Collections;

namespace Disney.Manimal.Common.Diagnostics
{
	public abstract class AbstractLoggerFactory : ILoggerFactory
	{
		private readonly Hashtable _cachedLoggers;

		private readonly ILogEntryFormatter _formatter;

		public LogLevel LogLevel
		{
			get;
			set;
		}

		protected AbstractLoggerFactory(LogLevel level)
			: this(BasicLogEntryFormatter.Instance, level)
		{
		}

		protected AbstractLoggerFactory(ILogEntryFormatter formatter, LogLevel level)
		{
			_cachedLoggers = new Hashtable();
			_formatter = formatter;
			LogLevel = level;
		}

		public ILogger GetLogger(Type type)
		{
			return GetLoggerInternal(type.FullName);
		}

		public ILogger GetLogger(string key)
		{
			return GetLoggerInternal(key);
		}

		private ILogger GetLoggerInternal(string name)
		{
			ILogger logger = null;
			if (!_cachedLoggers.Contains(name))
			{
				lock (_cachedLoggers.SyncRoot)
				{
					if (!_cachedLoggers.Contains(name))
					{
						logger = CreateLogger(name, _formatter, LogLevel);
						if (logger == null)
						{
							throw new ArgumentException(string.Format("{0} returned null logger for {1}.", GetType().FullName, name));
						}
						_cachedLoggers.Add(name, logger);
					}
				}
			}
			else
			{
				logger = (_cachedLoggers[name] as ILogger);
			}
			return logger;
		}

		protected abstract ILogger CreateLogger(string name, ILogEntryFormatter formatter, LogLevel level);
	}
}
