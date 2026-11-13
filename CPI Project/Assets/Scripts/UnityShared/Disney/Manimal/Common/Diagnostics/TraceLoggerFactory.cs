namespace Disney.Manimal.Common.Diagnostics
{
	public class TraceLoggerFactory : AbstractLoggerFactory
	{
		public TraceLoggerFactory()
			: base(LogLevel.Trace)
		{
		}

		public TraceLoggerFactory(ILogEntryFormatter formatter, LogLevel level)
			: base(formatter, level)
		{
		}

		protected override ILogger CreateLogger(string name, ILogEntryFormatter formatter, LogLevel level)
		{
			return new TraceLogger(name, formatter, level);
		}
	}
}
