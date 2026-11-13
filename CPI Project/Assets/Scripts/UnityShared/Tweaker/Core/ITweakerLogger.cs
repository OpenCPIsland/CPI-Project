namespace Tweaker.Core
{
	public interface ITweakerLogger
	{
		void Trace(string format, params object[] args);

		void Debug(string format, params object[] args);

		void Info(string format, params object[] args);

		void Warn(string format, params object[] args);

		void Error(string format, params object[] args);

		void Fatal(string format, params object[] args);
	}
}
