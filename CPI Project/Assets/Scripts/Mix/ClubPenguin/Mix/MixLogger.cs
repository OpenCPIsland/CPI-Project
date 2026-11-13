using Disney.LaunchPadFramework;
using Disney.Mix.SDK;

namespace ClubPenguin.Mix
{
	public class MixLogger : AbstractLogger
	{
		public override void Debug(string message)
		{
		}

		public override void Warning(string message)
		{
		}

		public override void Error(string message)
		{
		}

		public override void Critical(string message)
		{
		}

		public override void Fatal(string message)
		{
			Log.LogFatal(this, message);
		}
	}
}
