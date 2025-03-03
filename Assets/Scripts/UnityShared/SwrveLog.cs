public static class SwrveLog
{
	public enum LogLevel
	{
		Verbose,
		Info,
		Warning,
		Error,
		Disabled
	}

	public delegate void SwrveLogEventHandler(LogLevel level, object message, string tag);

	public static LogLevel Level = LogLevel.Verbose;

	public static event SwrveLogEventHandler OnLog;

	public static void Log(object message)
	{
		Log(message, "activity");
	}

	public static void LogInfo(object message)
	{
		LogInfo(message, "activity");
	}

	public static void LogWarning(object message)
	{
		LogWarning(message, "activity");
	}

	public static void LogError(object message)
	{
		LogError(message, "activity");
	}

	public static void Log(object message, string tag)
	{
		if (Level == LogLevel.Verbose && SwrveLog.OnLog != null)
		{
			SwrveLog.OnLog(LogLevel.Verbose, message, tag);
		}
	}

	public static void LogInfo(object message, string tag)
	{
		if ((Level == LogLevel.Verbose || Level == LogLevel.Info) && SwrveLog.OnLog != null)
		{
			SwrveLog.OnLog(LogLevel.Info, message, tag);
		}
	}

	public static void LogWarning(object message, string tag)
	{
		if ((Level == LogLevel.Verbose || Level == LogLevel.Info || Level == LogLevel.Warning) && SwrveLog.OnLog != null)
		{
			SwrveLog.OnLog(LogLevel.Warning, message, tag);
		}
	}

	public static void LogError(object message, string tag)
	{
		if ((Level == LogLevel.Verbose || Level == LogLevel.Info || Level == LogLevel.Warning || Level == LogLevel.Error) && SwrveLog.OnLog != null)
		{
			SwrveLog.OnLog(LogLevel.Error, message, tag);
		}
	}
}
