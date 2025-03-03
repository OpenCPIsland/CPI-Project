using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class ActionAnalytics : GameAnalytics
	{
		private string _tier1;

		private string _subEvent = null;

		private int _level = -1;

		private string _tier2;

		private string _context;

		private string _message;

		private string _tier3;

		private string _tier4;

		private IDictionary<string, object> _custom = null;

		public string Tier1
		{
			get
			{
				return _tier1;
			}
		}

		public string SubEvent
		{
			get
			{
				return _subEvent;
			}
		}

		public int Level
		{
			get
			{
				return _level;
			}
		}

		public string Tier2
		{
			get
			{
				return _tier2;
			}
		}

		public string Tier3
		{
			get
			{
				return _tier3;
			}
		}

		public string Tier4
		{
			get
			{
				return _tier4;
			}
		}

		public string Context
		{
			get
			{
				return _context;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public ActionAnalytics(string tier1)
		{
			InitActionAnalytics(tier1, null, -1, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent)
		{
			InitActionAnalytics(tier1, subEvent, -1, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level)
		{
			InitActionAnalytics(tier1, subEvent, level, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, tier3, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, message, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message, IDictionary<string, object> custom)
		{
			InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, message, custom);
		}

		private void InitActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message, IDictionary<string, object> custom)
		{
			_tier1 = tier1;
			_subEvent = subEvent;
			_level = level;
			_context = context;
			_message = message;
			_tier2 = tier2;
			_tier3 = tier3;
			_tier4 = tier4;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "action" + ((!string.IsNullOrEmpty(_subEvent)) ? ("." + _subEvent) : "");
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("tier1", _tier1);
			if (_level > -1)
			{
				dictionary.Add("level", _level);
			}
			if (!string.IsNullOrEmpty(_tier2))
			{
				dictionary.Add("tier2", _tier2);
			}
			if (!string.IsNullOrEmpty(_tier3))
			{
				dictionary.Add("tier3", _tier3);
			}
			if (!string.IsNullOrEmpty(_tier4))
			{
				dictionary.Add("tier4", _tier4);
			}
			if (!string.IsNullOrEmpty(_context))
			{
				dictionary.Add("context", _context);
			}
			if (!string.IsNullOrEmpty(_message))
			{
				dictionary.Add("message", _message);
			}
			if (_custom != null)
			{
				foreach (KeyValuePair<string, object> item in _custom)
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			return dictionary;
		}
	}
}
