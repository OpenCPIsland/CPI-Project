using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class ErrorAnalytics : GameAnalytics
	{
		private string _reason = null;

		private string _type = null;

		private string _context = null;

		private IDictionary<string, object> _custom = null;

		public string Reason
		{
			get
			{
				return _reason;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public string Context
		{
			get
			{
				return _context;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public ErrorAnalytics(string reason)
		{
			InitErrorActionAnalytics(reason, null, null, null);
		}

		public ErrorAnalytics(string reason, string type)
		{
			InitErrorActionAnalytics(reason, type, null, null);
		}

		public ErrorAnalytics(string reason, string type, string context)
		{
			InitErrorActionAnalytics(reason, type, context, null);
		}

		public ErrorAnalytics(string reason, string type, string context, IDictionary<string, object> custom)
		{
			InitErrorActionAnalytics(reason, type, context, custom);
		}

		private void InitErrorActionAnalytics(string reason, string type, string context, IDictionary<string, object> custom)
		{
			_reason = reason;
			_type = type;
			_context = context;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["reason"] = _reason;
			if (!string.IsNullOrEmpty(_type))
			{
				dictionary["type"] = _type;
			}
			if (!string.IsNullOrEmpty(_context))
			{
				dictionary["context"] = _context;
			}
			if (_custom != null)
			{
				foreach (KeyValuePair<string, object> item in _custom)
				{
					dictionary[item.Key] = item.Value;
				}
			}
			return dictionary;
		}

		public override string GetSwrveEvent()
		{
			return "error";
		}
	}
}
