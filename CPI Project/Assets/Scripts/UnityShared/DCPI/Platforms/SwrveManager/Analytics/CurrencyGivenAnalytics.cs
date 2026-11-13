using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class CurrencyGivenAnalytics : GameAnalytics
	{
		private string _given_currency;

		private float _given_amount;

		private int _level = -1;

		private string _context;

		private string _type;

		private string _subtype;

		private string _subtype2;

		private IDictionary<string, object> _custom = null;

		public string GivenCurrency
		{
			get
			{
				return _given_currency;
			}
		}

		public float GivenAmount
		{
			get
			{
				return _given_amount;
			}
		}

		public int Level
		{
			get
			{
				return _level;
			}
		}

		public string Context
		{
			get
			{
				return _context;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public string Subtype
		{
			get
			{
				return _subtype;
			}
		}

		public string Subtype2
		{
			get
			{
				return _subtype2;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, -1, null, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, null, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, subtype2, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2, IDictionary<string, object> custom)
		{
			InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, subtype2, custom);
		}

		private void InitCurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2, IDictionary<string, object> custom)
		{
			_given_currency = given_currency;
			_given_amount = given_amount;
			_level = level;
			_context = context;
			_type = type;
			_subtype = subtype;
			_subtype2 = subtype2;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "currency_given_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("given_currency", _given_currency);
			dictionary.Add("given_amount", _given_amount);
			if (_level > -1)
			{
				dictionary.Add("level", _level);
			}
			if (!string.IsNullOrEmpty(_context))
			{
				dictionary.Add("context", _context);
			}
			if (!string.IsNullOrEmpty(_type))
			{
				dictionary.Add("type", _type);
			}
			if (!string.IsNullOrEmpty(_subtype))
			{
				dictionary.Add("subtype", _subtype);
			}
			if (!string.IsNullOrEmpty(_subtype2))
			{
				dictionary.Add("subtype2", _subtype2);
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
