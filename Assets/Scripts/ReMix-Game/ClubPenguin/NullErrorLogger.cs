using System;

namespace ClubPenguin
{
	public class NullErrorLogger : IStandaloneErrorLogger
	{
		public void LogHandledException(Exception ex, string stackTrace)
		{
		}

		public void LogHandledExceptionMessage(string message)
		{
		}

		public void LogUnhandledException(Exception ex)
		{
		}

		public void LogUnhandledException(string message)
		{
		}

		public void LogError(string errorMsg)
		{
		}

		public void Flush()
		{
		}
	}
}
