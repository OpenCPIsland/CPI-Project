using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public abstract class AbstractLogger : ILogger
	{
		private readonly string _name;

		private readonly ILogEntryFormatter _formatter;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public ILogEntryFormatter Formatter
		{
			get
			{
				return _formatter;
			}
		}

		public LogLevel LogLevel
		{
			get;
			set;
		}

		public bool IsTraceEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Trace);
			}
		}

		public bool IsDebugEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Debug);
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Info);
			}
		}

		public bool IsWarnEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Warning);
			}
		}

		public bool IsErrorEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Error);
			}
		}

		public bool IsFatalEnabled
		{
			get
			{
				return IsLevelEnabled(LogLevel.Fatal);
			}
		}

		protected AbstractLogger(string name, LogLevel level)
			: this(name, BasicLogEntryFormatter.Instance, level)
		{
		}

		protected AbstractLogger(string name, ILogEntryFormatter formatter, LogLevel level)
		{
			_name = name;
			_formatter = formatter;
			LogLevel = level;
		}

		public void Trace(object message)
		{
			if (IsTraceEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Trace,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void TraceFormat(string format, params object[] args)
		{
			TraceFormat(null, format, args);
		}

		public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Trace,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		public void Debug(object message)
		{
			if (IsDebugEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Debug,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void DebugFormat(string format, params object[] args)
		{
			DebugFormat(null, format, args);
		}

		public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Debug,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		public void Info(object message)
		{
			if (IsInfoEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Info,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void InfoFormat(string format, params object[] args)
		{
			InfoFormat(null, format, args);
		}

		public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Info,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		public void Warn(object message)
		{
			if (IsWarnEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Warning,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void WarnFormat(string format, params object[] args)
		{
			WarnFormat(null, format, args);
		}

		public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Warning,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		public void Error(object message)
		{
			if (IsErrorEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Error,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void ErrorFormat(string format, params object[] args)
		{
			ErrorFormat(null, format, args);
		}

		public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Error,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		public void Fatal(object message)
		{
			if (IsFatalEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Fatal,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					Message = message
				});
			}
		}

		public void FatalFormat(string format, params object[] args)
		{
			FatalFormat(null, format, args);
		}

		public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				WriteLogEntry(new LogEntry
				{
					Level = LogLevel.Fatal,
					Timestamp = DateTime.UtcNow,
					LoggerName = Name,
					FormattedMessage = new FormattedMessage(formatProvider, format, args)
				});
			}
		}

		protected virtual bool IsLevelEnabled(LogLevel level)
		{
			return level >= LogLevel;
		}

		protected virtual string FormatLogEntry(LogEntry entry)
		{
			return Formatter.FormatLogEntry(entry);
		}

		protected abstract void WriteLogEntry(LogEntry entry);
	}
}
