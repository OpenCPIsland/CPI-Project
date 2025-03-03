using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class GameTimingAnalytics : GameAnalytics
	{
		public virtual string location
		{
			get
			{
				return null;
			}
		}

		public virtual float elapsedTime
		{
			get
			{
				return 0f;
			}
		}

		public virtual string result
		{
			get
			{
				return null;
			}
		}

		public override string GetSwrveEvent()
		{
			return "timing_action." + location;
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["elapsed_time"] = elapsedTime;
			if (result != null)
			{
				dictionary["result"] = result;
			}
			return dictionary;
		}
	}
}
