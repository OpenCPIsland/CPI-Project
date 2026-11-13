using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class IAPAnalytics : GameAnalytics
	{
		public enum IAPStore
		{
			APPLE,
			GOOGLE,
			UNKNOWN_STORE
		}

		private string _productId;

		private float _cost;

		private int _quantity = 1;

		private string _local_currency;

		private IAPStore _app_store;

		private string _durability;

		private int _level;

		private string _context;

		private string _type;

		private string _subtype;

		public string ProductId
		{
			get
			{
				return _productId;
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

		public string LocalCurrency
		{
			get
			{
				return _local_currency;
			}
		}

		public IAPStore AppStore
		{
			get
			{
				return _app_store;
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

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPStore app_store, string durablity, int level)
		{
			InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, null, null, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPStore app_store, string durablity, int level, string context)
		{
			InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, null, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPStore app_store, string durablity, int level, string context, string type)
		{
			InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, type, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPStore app_store, string durablity, int level, string context, string type, string subtype)
		{
			InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, type, subtype);
		}

		private void InitIAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPStore app_store, string durablity, int level, string context, string type, string subtype)
		{
			_productId = productId;
			_cost = cost;
			_quantity = quantity;
			_local_currency = local_currency;
			_app_store = app_store;
			_durability = durablity;
			_level = level;
			_context = context;
			_type = type;
			_subtype = subtype;
		}

		public override string GetSwrveEvent()
		{
			return "IAP_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["product_id"] = _productId;
			dictionary["cost"] = _cost;
			dictionary["quantity"] = _quantity;
			dictionary["local_currency"] = _local_currency;
			dictionary["app_store"] = _app_store.ToString().ToLower();
			dictionary["durability"] = _durability;
			if (_level > -1)
			{
				dictionary["level"] = _level;
			}
			if (!string.IsNullOrEmpty(_context))
			{
				dictionary["context"] = _context;
			}
			if (!string.IsNullOrEmpty(_type))
			{
				dictionary["type"] = _type;
			}
			if (!string.IsNullOrEmpty(_subtype))
			{
				dictionary["subtype"] = _subtype;
			}
			return dictionary;
		}
	}
}
