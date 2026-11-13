using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public interface IAnalytics
	{
		Dictionary<string, object> Serialize();

		string GetSwrveEvent();
	}
}
