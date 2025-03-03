using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin
{
	public class MembershipPlans
	{
		private Dictionary<string, MembershipPlansDefinition> membershipPlanData;

		public Dictionary<string, string> RegionCountryMapping;

		public string CountryCode
		{
			get;
			private set;
		}

		public string DefaultResubSKU
		{
			get;
			private set;
		}

		public string DefaultFirstTimeSKU
		{
			get;
			private set;
		}

		public List<string> AllResubSKUs
		{
			get;
			private set;
		}

		public List<string> AllFirstTimeSKUs
		{
			get;
			private set;
		}

		public List<string> ToOfferResubSKUs
		{
			get;
			private set;
		}

		public List<string> ToOfferFirstTimeSKUs
		{
			get;
			private set;
		}

		public MembershipPlans(string countryCode)
		{
			CountryCode = countryCode;
			initializeSKUs();
		}

		public void SetCountryCode(string countryCode)
		{
			CountryCode = countryCode;
			initializeSKUs();
		}

		private void loadMembershipPlanData()
		{
			membershipPlanData = Service.Get<GameData>().Get<Dictionary<string, MembershipPlansDefinition>>();
			if (membershipPlanData != null && membershipPlanData.Count > 0)
			{
				RegionCountryMapping = new Dictionary<string, string>();
				foreach (KeyValuePair<string, MembershipPlansDefinition> membershipPlanDatum in membershipPlanData)
				{
					if (membershipPlanDatum.Value.Countries.Length > 0)
					{
						string[] countries = membershipPlanDatum.Value.Countries;
						foreach (string text in countries)
						{
							RegionCountryMapping.Add(text.ToString(), membershipPlanDatum.Key);
						}
					}
				}
			}
		}

		private void initializeSKUs()
		{
			if (membershipPlanData == null)
			{
				loadMembershipPlanData();
			}
			DefaultFirstTimeSKU = "";
			AllFirstTimeSKUs = new List<string>();
			ToOfferFirstTimeSKUs = new List<string>();
			DefaultResubSKU = "";
			AllResubSKUs = new List<string>();
			ToOfferResubSKUs = new List<string>();
			MembershipPlansDefinition value;
			if (membershipPlanData.TryGetValue("default", out value))
			{
				DefaultFirstTimeSKU = value.FirstTime.defaultSKU;
				AllFirstTimeSKUs = value.FirstTime.AllSKUs.ToList();
				ToOfferFirstTimeSKUs = value.FirstTime.OfferSKUs.ToList();
				DefaultResubSKU = value.Resubscribe.defaultSKU;
				AllResubSKUs = value.Resubscribe.AllSKUs.ToList();
				ToOfferResubSKUs = value.Resubscribe.OfferSKUs.ToList();
			}
			if (!string.IsNullOrEmpty(CountryCode) && RegionCountryMapping.ContainsKey(CountryCode) && membershipPlanData.TryGetValue(RegionCountryMapping[CountryCode], out value))
			{
				if (value.FirstTime.overrideDefaultSKU)
				{
					DefaultFirstTimeSKU = value.FirstTime.defaultSKU;
				}
				if (value.FirstTime.overrideAllSKUs)
				{
					AllFirstTimeSKUs = value.FirstTime.AllSKUs.ToList();
				}
				if (value.FirstTime.overrideOfferSKUs)
				{
					ToOfferFirstTimeSKUs = value.FirstTime.OfferSKUs.ToList();
				}
				if (value.Resubscribe.overrideDefaultSKU)
				{
					DefaultResubSKU = value.Resubscribe.defaultSKU;
				}
				if (value.Resubscribe.overrideAllSKUs)
				{
					AllResubSKUs = value.Resubscribe.AllSKUs.ToList();
				}
				if (value.Resubscribe.overrideOfferSKUs)
				{
					ToOfferResubSKUs = value.Resubscribe.OfferSKUs.ToList();
				}
			}
		}
	}
}
