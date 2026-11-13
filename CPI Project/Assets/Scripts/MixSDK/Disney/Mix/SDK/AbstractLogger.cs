namespace Disney.Mix.SDK
{
	public abstract class AbstractLogger
	{
		public abstract void Debug(string message);

		public abstract void Warning(string message);

		public abstract void Error(string message);

		public abstract void Critical(string message);

		public abstract void Fatal(string message);
	}
}
