using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class GenericWrapperAnalytics : GameAnalytics
	{
		private string _action = string.Empty;

		private Dictionary<string, object> _payload = null;

		public GenericWrapperAnalytics(string action, Dictionary<string, object> payload)
		{
			_action = action;
			_payload = payload;
		}

		public override string GetSwrveEvent()
		{
			return _action;
		}

		public override Dictionary<string, object> Serialize()
		{
			return _payload;
		}
	}
}
