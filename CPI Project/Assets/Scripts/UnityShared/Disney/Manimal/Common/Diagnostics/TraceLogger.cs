#define TRACE
using System.Diagnostics;

namespace Disney.Manimal.Common.Diagnostics
{
	public class TraceLogger : AbstractLogger
	{
		public TraceLogger(string name, LogLevel level)
			: base(name, level)
		{
		}

		public TraceLogger(string name, ILogEntryFormatter formatter, LogLevel level)
			: base(name, formatter, level)
		{
		}

		protected override void WriteLogEntry(LogEntry entry)
		{
			string message = FormatLogEntry(entry);
			switch (entry.Level)
			{
			case LogLevel.Info:
				System.Diagnostics.Trace.TraceInformation(message);
				break;
			case LogLevel.Warning:
				System.Diagnostics.Trace.TraceWarning(message);
				break;
			case LogLevel.Error:
			case LogLevel.Fatal:
				System.Diagnostics.Trace.TraceError(message);
				break;
			default:
				System.Diagnostics.Trace.WriteLine(message);
				break;
			}
		}
	}
}
