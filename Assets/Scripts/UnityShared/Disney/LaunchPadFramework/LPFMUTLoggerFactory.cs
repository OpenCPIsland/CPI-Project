using Disney.Manimal.Common.Diagnostics;

namespace Disney.LaunchPadFramework
{
	public class LPFMUTLoggerFactory : AbstractLoggerFactory
	{
		public LPFMUTLoggerFactory(LogLevel level)
			: base(level)
		{
		}

		public LPFMUTLoggerFactory(ILogEntryFormatter formatter, LogLevel level)
			: base(formatter, level)
		{
		}

		protected override ILogger CreateLogger(string name, ILogEntryFormatter formatter, LogLevel level)
		{
			return new LPFMUTLogger(name, formatter, level);
		}
	}
}
