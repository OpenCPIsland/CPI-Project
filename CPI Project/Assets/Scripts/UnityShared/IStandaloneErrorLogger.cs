using System;

public interface IStandaloneErrorLogger
{
	void LogHandledException(Exception ex, string stackTrace);

	void LogHandledExceptionMessage(string message);

	void LogUnhandledException(Exception ex);

	void LogUnhandledException(string message);

	void LogError(string errorMsg);

	void Flush();
}
