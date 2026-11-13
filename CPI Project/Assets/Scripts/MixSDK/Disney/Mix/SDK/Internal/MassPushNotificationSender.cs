using System;

namespace Disney.Mix.SDK.Internal
{
	public static class MassPushNotificationSender
	{
		public static void SendMassPushNotification(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, Action<bool> callback)
		{
			logger.Critical("Not Implemented : SendMassPushNotification");
			callback(false);
		}
	}
}
