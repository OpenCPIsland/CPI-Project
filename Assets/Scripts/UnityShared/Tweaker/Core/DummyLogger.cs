#define DEBUG
using System.Diagnostics;

namespace Tweaker.Core
{
	internal class DummyLogger : ITweakerLogger
	{
		public void Trace(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "TRACE", string.Format(format, args)));
		}

		public void Debug(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "DEBUG", string.Format(format, args)));
		}

		public void Info(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "INFO", string.Format(format, args)));
		}

		public void Warn(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "WARN", string.Format(format, args)));
			System.Diagnostics.Debug.WriteLine(new StackTrace(true).ToString());
		}

		public void Error(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "ERROR", string.Format(format, args)));
			System.Diagnostics.Debug.WriteLine(new StackTrace(true).ToString());
		}

		public void Fatal(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("{0} | {1}", "FATAL", string.Format(format, args)));
			System.Diagnostics.Debug.WriteLine(new StackTrace(true).ToString());
		}
	}
}
