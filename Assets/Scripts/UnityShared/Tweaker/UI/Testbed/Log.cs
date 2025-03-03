using Disney.LaunchPadFramework;
using Tweaker.Core;

namespace Tweaker.UI.Testbed
{
	public class Log : ITweakerLogger
	{
		private string name;

		public Log(string name)
		{
			this.name = name;
		}

		public void Trace(string format, params object[] args)
		{
		}

		public void Debug(string format, params object[] args)
		{
		}

		public void Info(string format, params object[] args)
		{
		}

		public void Warn(string format, params object[] args)
		{
		}

		public void Error(string format, params object[] args)
		{
			Disney.LaunchPadFramework.Log.LogError(this, string.Format("{0} | {1} | {2}", "ERROR", name, string.Format(format, args)));
		}

		public void Fatal(string format, params object[] args)
		{
			Disney.LaunchPadFramework.Log.LogFatal(this, string.Format("{0} | {1} | {2}", "FATAL", name, string.Format(format, args)));
		}
	}
}
