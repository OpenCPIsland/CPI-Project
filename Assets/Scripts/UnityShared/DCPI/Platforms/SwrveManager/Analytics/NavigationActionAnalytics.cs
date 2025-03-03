using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class NavigationActionAnalytics : GameAnalytics
	{
		private string _buttonPressed = null;

		private string _fromLocation = null;

		private string _toLocation = null;

		private string _module = null;

		private int _order = -1;

		private IDictionary<string, object> _custom = null;

		public string ButtonPressed
		{
			get
			{
				return _buttonPressed;
			}
		}

		public string FromLocation
		{
			get
			{
				return _fromLocation;
			}
		}

		public string ToLocation
		{
			get
			{
				return _toLocation;
			}
		}

		public string Module
		{
			get
			{
				return _module;
			}
		}

		public int Order
		{
			get
			{
				return _order;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public NavigationActionAnalytics(string buttonPressed)
		{
			InitNavigationActionAnalytics(buttonPressed, null, null, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation)
		{
			InitNavigationActionAnalytics(buttonPressed, fromLocation, null, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation)
		{
			InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module)
		{
			InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order)
		{
			InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, order, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order, IDictionary<string, object> custom)
		{
			InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, order, custom);
		}

		private void InitNavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order, IDictionary<string, object> custom)
		{
			_buttonPressed = buttonPressed;
			_fromLocation = fromLocation;
			_toLocation = toLocation;
			_module = module;
			_order = order;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["button_pressed"] = _buttonPressed;
			if (!string.IsNullOrEmpty(_fromLocation))
			{
				dictionary["from_location"] = _fromLocation;
			}
			if (!string.IsNullOrEmpty(_toLocation))
			{
				dictionary["to_location"] = _toLocation;
			}
			if (!string.IsNullOrEmpty(_module))
			{
				dictionary["module"] = _module;
			}
			if (_order > -1)
			{
				dictionary["order"] = _order;
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
			return "navigation_action";
		}
	}
}
