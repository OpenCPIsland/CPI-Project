using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class GenericStringAnalytics : GameAnalytics
	{
		private string _message = string.Empty;

		private string _action = string.Empty;

		public string message
		{
			get
			{
				return _message;
			}
		}

		public GenericStringAnalytics(string action)
		{
			_action = action;
		}

		public GenericStringAnalytics(string action, string message)
		{
			_action = action;
			_message = message;
		}

		public override string GetSwrveEvent()
		{
			return _action;
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (!string.IsNullOrEmpty(_message))
			{
				dictionary.Add("message", _message);
			}
			return dictionary;
		}
	}
}
