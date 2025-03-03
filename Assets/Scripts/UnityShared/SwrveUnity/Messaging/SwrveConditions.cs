using System;
using System.Collections.Generic;
using System.Linq;

namespace SwrveUnity.Messaging
{
	public class SwrveConditions
	{
		public enum TriggerOperatorType
		{
			AND,
			EQUALS
		}

		private const string OP_KEY = "op";

		private const string OP_EQ_KEY = "eq";

		private const string OP_AND_KEY = "and";

		private const string KEY_KEY = "key";

		private const string VALUE_KEY = "value";

		private const string ARGS_KEY = "args";

		private string key;

		private TriggerOperatorType? op;

		private string value;

		private List<SwrveConditions> args;

		public string GetKey()
		{
			return key;
		}

		public TriggerOperatorType? GetOp()
		{
			return op;
		}

		public string GetValue()
		{
			return value;
		}

		public List<SwrveConditions> GetArgs()
		{
			return args;
		}

		private SwrveConditions(TriggerOperatorType? op)
		{
			this.op = op;
		}

		private SwrveConditions(TriggerOperatorType? op, string key, string value)
			: this(op)
		{
			this.key = key;
			this.value = value;
		}

		private SwrveConditions(TriggerOperatorType? op, List<SwrveConditions> args)
			: this(op)
		{
			this.args = args;
		}

		private bool isEmpty()
		{
			return !op.HasValue;
		}

		private bool matchesEquals(IDictionary<string, string> payload)
		{
			return op == TriggerOperatorType.EQUALS && payload.ContainsKey(key) && string.Equals(payload[key], value, StringComparison.OrdinalIgnoreCase);
		}

		private bool matchesAll(IDictionary<string, string> payload)
		{
			TriggerOperatorType? triggerOperatorType = op;
			return triggerOperatorType.GetValueOrDefault() == TriggerOperatorType.AND && triggerOperatorType.HasValue && args.All((SwrveConditions cond) => cond.Matches(payload));
		}

		public bool Matches(IDictionary<string, string> payload)
		{
			return isEmpty() || (payload != null && (matchesEquals(payload) || matchesAll(payload)));
		}

		public static SwrveConditions LoadFromJson(IDictionary<string, object> json, bool isRoot)
		{
			if (0 == json.Keys.Count)
			{
				if (isRoot)
				{
					return new SwrveConditions(null);
				}
				return null;
			}
			string a = (string)json["op"];
			if (a == "eq")
			{
				string text = (string)json["key"];
				string text2 = (string)json["value"];
				if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
				{
					return null;
				}
				return new SwrveConditions(TriggerOperatorType.EQUALS, text, text2);
			}
			if (isRoot && a == "and")
			{
				IList<object> list = (IList<object>)json["args"];
				List<SwrveConditions> list2 = new List<SwrveConditions>();
				IEnumerator<object> enumerator = list.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SwrveConditions swrveConditions = LoadFromJson((Dictionary<string, object>)enumerator.Current, false);
					if (swrveConditions == null)
					{
						return null;
					}
					list2.Add(swrveConditions);
				}
				if (list2.Count == 0)
				{
					return null;
				}
				return new SwrveConditions(TriggerOperatorType.AND, list2);
			}
			return null;
		}

		public override string ToString()
		{
			return string.Concat("Conditions{key='", key, '\'', ", op='", op, '\'', ", value='", value, '\'', ", args=", args, '}');
		}
	}
}
