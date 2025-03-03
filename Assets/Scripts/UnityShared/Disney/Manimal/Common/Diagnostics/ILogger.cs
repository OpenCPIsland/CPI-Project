using System;

namespace Disney.Manimal.Common.Diagnostics
{
	public interface ILogger
	{
		string Name
		{
			get;
		}

		LogLevel LogLevel
		{
			get;
		}

		bool IsTraceEnabled
		{
			get;
		}

		bool IsDebugEnabled
		{
			get;
		}

		bool IsInfoEnabled
		{
			get;
		}

		bool IsWarnEnabled
		{
			get;
		}

		bool IsErrorEnabled
		{
			get;
		}

		bool IsFatalEnabled
		{
			get;
		}

		void Trace(object message);

		void TraceFormat(string format, params object[] args);

		void TraceFormat(IFormatProvider formatProvider, string format, params object[] args);

		void Debug(object message);

		void DebugFormat(string format, params object[] args);

		void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);

		void Info(object message);

		void InfoFormat(string format, params object[] args);

		void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);

		void Warn(object message);

		void WarnFormat(string format, params object[] args);

		void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);

		void Error(object message);

		void ErrorFormat(string format, params object[] args);

		void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);

		void Fatal(object message);

		void FatalFormat(string format, params object[] args);

		void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);
	}
}
