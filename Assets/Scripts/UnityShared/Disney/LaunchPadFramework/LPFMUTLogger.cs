using Disney.Manimal.Common.Diagnostics;
using System;

namespace Disney.LaunchPadFramework
{
	public class LPFMUTLogger : AbstractLogger
	{
		public LPFMUTLogger(string name, ILogEntryFormatter formatter, LogLevel level)
			: base(name, formatter, level)
		{
		}

		protected override void WriteLogEntry(LogEntry entry)
		{
			string message = FormatLogEntry(entry);
			Type type = GetType();
			switch (entry.Level)
			{
			case LogLevel.Trace:
				break;
			case LogLevel.Debug:
				break;
			case LogLevel.Info:
				break;
			case LogLevel.Warning:
				break;
			case LogLevel.Error:
				break;
			case LogLevel.Fatal:
				Log.LogFatal(type, message);
				break;
			}
		}
	}
}
