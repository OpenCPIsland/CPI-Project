using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class AdActionAnalytics : GameAnalytics
	{
		private string _creative = null;

		private string _placement = null;

		private string _type = null;

		private string _referralStoreVersion = null;

		private IDictionary<string, object> _custom = null;

		public string Creative
		{
			get
			{
				return _creative;
			}
		}

		public string Placement
		{
			get
			{
				return _placement;
			}
		}

		public string Type
		{
			get
			{
				return _type;
			}
		}

		public string ReferralStoreVersion
		{
			get
			{
				return _referralStoreVersion;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public AdActionAnalytics(string creative, string placement, string type)
		{
			InitAdActionAnalytics(creative, placement, type, null, null);
		}

		public AdActionAnalytics(string creative, string placement, string type, string referralStoreVersion)
		{
			InitAdActionAnalytics(creative, placement, type, referralStoreVersion, null);
		}

		public AdActionAnalytics(string creative, string placement, string type, string referralStoreVersion, IDictionary<string, object> custom)
		{
			InitAdActionAnalytics(creative, placement, type, referralStoreVersion, custom);
		}

		private void InitAdActionAnalytics(string creative, string placement, string type, string referralStoreVersion, IDictionary<string, object> custom)
		{
			_creative = creative;
			_placement = placement;
			_type = type;
			_referralStoreVersion = referralStoreVersion;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "ad_action";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("creative", _creative);
			dictionary.Add("placement", _placement);
			dictionary.Add("type", _type);
			if (!string.IsNullOrEmpty(_referralStoreVersion))
			{
				dictionary.Add("referralstore_version", _referralStoreVersion);
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
