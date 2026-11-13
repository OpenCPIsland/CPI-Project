using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class PurchaseAnalytics : GameAnalytics
	{
		private string _item;

		private float _cost;

		private string _currency;

		private string _durability;

		private int _quantity = -1;

		private int _level = -1;

		private string _context;

		private string _type;

		private string _subtype;

		private string _subtype2;

		public string Item
		{
			get
			{
				return _item;
			}
		}

		public float Cost
		{
			get
			{
				return _cost;
			}
		}

		public int Quantity
		{
			get
			{
				return _quantity;
			}
		}

		public string Currency
		{
			get
			{
				return _currency;
			}
		}

		public string Durability
		{
			get
			{
				return _durability;
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

		public PurchaseAnalytics(string item, float cost, string currency, string durability)
		{
			InitPurchaseAnalytics(item, cost, -1, currency, durability, -1, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, -1, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, subtype, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype, string subtype2)
		{
			InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, subtype, subtype2);
		}

		private void InitPurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype, string subtype2)
		{
			_item = item;
			_cost = cost;
			_quantity = quantity;
			_currency = currency;
			_durability = durability;
			_level = level;
			_context = context;
			_type = type;
			_subtype = subtype;
			_subtype2 = subtype2;
		}

		public override string GetSwrveEvent()
		{
			return "purchase_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("item", _item);
			dictionary.Add("cost", _cost);
			if (_quantity > -1)
			{
				dictionary.Add("quantity", _quantity);
			}
			dictionary.Add("currency", _currency);
			dictionary.Add("durability", _durability);
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
			return dictionary;
		}
	}
}
