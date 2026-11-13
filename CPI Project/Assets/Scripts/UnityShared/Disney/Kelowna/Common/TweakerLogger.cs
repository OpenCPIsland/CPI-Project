using Disney.LaunchPadFramework;
using Tweaker.Core;

namespace Disney.Kelowna.Common
{
	public class TweakerLogger : ITweakerLogger
	{
		private string name;

		public TweakerLogger(string name)
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
			Log.LogFatal(name, string.Format(format, args));
		}

		public void Fatal(string format, params object[] args)
		{
			Log.LogFatal(name, string.Format(format, args));
		}
	}
}
