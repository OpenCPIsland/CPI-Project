using ClubPenguin.Accessibility;
using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SettingsController : MonoBehaviour
	{
		private enum SettingsStates
		{
			Main,
			ServerList,
			Transition,
			Rules,
			PrivacyPolicy,
			TermsOfUse,
			ChildrensPrivacyPolicy,
			LicenseCredits,
			Help,
			MembershipInfo,
			AllAccessEventMembershipInfo,
			CustomGraphics
		}

		private SettingsTweener settingsTweener;

		private StateMachineContext smContext;

		private void Start()
		{
			settingsTweener = GetComponentInChildren<SettingsTweener>();
			smContext = GetComponentInParent<StateMachineContext>();
			Service.Get<ICPSwrveService>().Action("view.settings", "start");
			Service.Get<EventDispatcher>().AddListener<AccessibilityEvents.AccessibilityScaleUpdated>(onTextScaleChanged);
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.HideLoadingScreen));
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<AccessibilityEvents.AccessibilityScaleUpdated>(onTextScaleChanged);
		}

		public void OnStateChanged(string state)
		{
			switch ((SettingsStates)Enum.Parse(typeof(SettingsStates), state))
			{
			case SettingsStates.Main:
				break;
			case SettingsStates.Rules:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_rules");
				settingsTweener.Open();
				break;
			case SettingsStates.ServerList:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_serverlist");
				settingsTweener.Open();
				break;
			case SettingsStates.PrivacyPolicy:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_privacypolicy");
				settingsTweener.Open();
				break;
			case SettingsStates.ChildrensPrivacyPolicy:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_childrensprivacypolicy");
				settingsTweener.Open();
				break;
			case SettingsStates.LicenseCredits:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_licensecredits");
				settingsTweener.Open();
				break;
			case SettingsStates.TermsOfUse:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_termsofuse");
				settingsTweener.Open();
				break;
			case SettingsStates.Help:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_support");
				openSettingsTweener();
				break;
			case SettingsStates.MembershipInfo:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_membershipinfo");
				settingsTweener.Open();
				break;
			case SettingsStates.AllAccessEventMembershipInfo:
				settingsTweener.Open();
				break;
			case SettingsStates.CustomGraphics:
				Service.Get<ICPSwrveService>().Action("view.settings", "settings_customgraphics");
				settingsTweener.Open();
				break;
			case SettingsStates.Transition:
				settingsTweener.Close();
				settingsTweener.OnComplete += OnTweenerComplete;
				break;
			}
		}

		private void openSettingsTweener()
		{
			if (!settingsTweener.IsOpen)
			{
				settingsTweener.Open();
			}
		}

		private void OnTweenerComplete()
		{
			settingsTweener.OnComplete -= OnTweenerComplete;
			smContext.SendEvent(new ExternalEvent("Settings", "transitioncomplete"));
		}

		private bool onTextScaleChanged(AccessibilityEvents.AccessibilityScaleUpdated evt)
		{
			if (settingsTweener != null)
			{
				settingsTweener.CalculateTweenPosition();
			}
			return false;
		}
	}
}
