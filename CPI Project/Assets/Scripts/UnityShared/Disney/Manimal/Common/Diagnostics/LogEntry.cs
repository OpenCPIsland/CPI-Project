using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public class LogEntry
	{
		public LogLevel Level
		{
			get;
			set;
		}

		public DateTime Timestamp
		{
			get;
			set;
		}

		public string LoggerName
		{
			get;
			set;
		}

		public object Message
		{
			get;
			set;
		}

		public FormattedMessage FormattedMessage
		{
			get;
			set;
		}
	}
}
