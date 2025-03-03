using UnityEngine;

namespace Fabric
{
	public class FabricTimer
	{
		public static ICustomTimer customTimer = null;

		private static float _realTimeDelta;

		private static float _realTimeSinceStartup;

		public static void Update()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			_realTimeDelta = realtimeSinceStartup - _realTimeSinceStartup;
			_realTimeDelta = Mathf.Min(_realTimeDelta, Time.maximumDeltaTime);
			_realTimeSinceStartup = realtimeSinceStartup;
		}

		public static float GetRealtimeDelta()
		{
			if (!Application.isPlaying)
			{
				return _realTimeDelta;
			}
			return Time.deltaTime;
		}

		public static float Get()
		{
			if (customTimer != null)
			{
				return customTimer.Get();
			}
			if (!Application.isPlaying)
			{
				return Time.realtimeSinceStartup;
			}
			return Time.time;
		}
	}
}
