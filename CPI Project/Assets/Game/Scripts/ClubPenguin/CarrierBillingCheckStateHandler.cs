using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class CarrierBillingCheckStateHandler : AbstractAccountStateHandler
	{
		public string SupportedEvent;

		public string NotSupportedEvent;

		public static string CARRIER_BILLING_SUPPORTED_ASSET = "CarrierBillingSupported";

		private JsonData carrierBillingSupported;

		private void initializeCarrierBillingSupported()
		{
			TextAsset textAsset = Resources.Load<TextAsset>(CARRIER_BILLING_SUPPORTED_ASSET);
			if (!(textAsset == null) && !string.IsNullOrEmpty(textAsset.text))
			{
				string text = textAsset.text;
				carrierBillingSupported = JsonMapper.ToObject(text);
				if (carrierBillingSupported.Count > 0)
				{
				}
			}
		}

		public void OnStateChanged(string state)
		{
			if (!(state == HandledState) || !(rootStateMachine != null))
			{
				return;
			}
			MembershipService membershipService = Service.Get<MembershipService>();
			if (carrierBillingSupported == null)
			{
				initializeCarrierBillingSupported();
			}
			string text = NotSupportedEvent;
			string prop_name = "skip";
			if (carrierBillingSupported.Contains(prop_name))
			{
				Dictionary<string, string> deviceInfo = Service.Get<ICPSwrveService>().GetDeviceInfo();
				string value = MembershipService.OverrideSimCountryCode;
				if (string.IsNullOrEmpty(value))
				{
					deviceInfo.TryGetValue("swrve.sim_operator.iso_country_code", out value);
				}
				string value2 = MembershipService.OverrideSimCarrierName;
				if (string.IsNullOrEmpty(value2))
				{
					deviceInfo.TryGetValue("swrve.sim_operator.name", out value2);
				}
				if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && carrierBillingSupported[prop_name].Contains(value.ToUpper()))
				{
					JsonData jsonData = carrierBillingSupported[prop_name][value.ToUpper()];
					foreach (object item in (IEnumerable)jsonData)
					{
						if (item.ToString().Trim().ToUpper() == value2.Trim().ToUpper())
						{
							text = SupportedEvent;
						}
					}
				}
			}
			Service.Get<ICPSwrveService>().Funnel(membershipService.MembershipFunnelName, "02a", "check_carrier_billing_supported", text);
			rootStateMachine.SendEvent(text);
		}
	}
}
