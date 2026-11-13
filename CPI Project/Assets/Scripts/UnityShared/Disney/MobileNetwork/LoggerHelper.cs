using UnityEngine;

namespace Disney.MobileNetwork
{
	public class LoggerHelper
	{
		public delegate void LoggerDelegate(object sourceObject, string message, LogType logType);

		public event LoggerDelegate LogMessageHandler = delegate
		{
		};

		public void LogAssert(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Assert);
		}

		public void LogError(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Error);
		}

		public void LogException(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Exception);
		}

		public void LogDebug(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Log);
		}

		public void LogInfo(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Log);
		}

		public void LogWarning(object sourceObject, string message)
		{
			this.LogMessageHandler(sourceObject, message, LogType.Warning);
		}

		public void Log(object sourceObject, string message, LogType logType)
		{
			this.LogMessageHandler(sourceObject, message, logType);
		}
	}
}
