using SwrveUnityMiniJSON;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public abstract class GameAnalytics : IAnalytics
	{
		public abstract Dictionary<string, object> Serialize();

		public abstract string GetSwrveEvent();

		public override string ToString()
		{
			return Json.Serialize(Serialize());
		}
	}
}
