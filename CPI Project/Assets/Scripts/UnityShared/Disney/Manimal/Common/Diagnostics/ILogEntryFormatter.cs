namespace Disney.Manimal.Common.Diagnostics
{
	public interface ILogEntryFormatter
	{
		string FormatLogEntry(LogEntry entry);
	}
}
