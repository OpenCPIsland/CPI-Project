using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class SunsetPartyController : MonoBehaviour
	{
		public SceneDefinition EndCreditsScene;

		public string SunsetPartyActivePlayerPrefsKey;

		public string SunsetQuestCompletedPlayerPrefsKey;

		public PromptDefinitionKey ShowCreditsDuringPartyPromptKey;

		public PromptDefinitionKey SunsetPartyPromptKey;

		public PromptDefinitionKey SunsetPartyEvergreenPromptKey;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public string[] SunsetQuestPlayerPrefsKeys;

		private new Collider collider;

		private QuestService questService;

		private void Awake()
		{
			bool flag = false;
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(DateDefinitionKey))
			{
				flag |= !isSunsetQuestCompleted();
			}
			questService = Service.Get<QuestService>();
			flag |= questService.IsQuestActive();
			collider = GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = !flag;
			}
			else
			{
				Log.LogError(this, "Could not find Collider to disable");
			}
		}

		private void Update()
		{
			if (collider != null)
			{
				collider.enabled = !questService.IsQuestActive();
			}
		}

		public void ShowSunsetPartyPrompts()
		{
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(DateDefinitionKey))
			{
				if (isSunsetQuestCompleted())
				{
					Service.Get<PromptManager>().ShowPrompt(ShowCreditsDuringPartyPromptKey.Id, onResetSunsetQuestDismissed);
				}
			}
			else if (Service.Get<ContentSchedulerService>().IsAfterScheduleEventDates(DateDefinitionKey))
			{
				if (DisplayNamePlayerPrefs.HasKey(SunsetPartyActivePlayerPrefsKey))
				{
					Service.Get<PromptManager>().ShowPrompt(SunsetPartyPromptKey.Id, onSunsetPartyPromptDismissed);
				}
				else
				{
					Service.Get<PromptManager>().ShowPrompt(SunsetPartyEvergreenPromptKey.Id, onSunsetPartyEvergreenPromptDismissed);
				}
			}
		}

		private void onResetSunsetQuestDismissed(DPrompt.ButtonFlags buttonFlags)
		{
			if (buttonFlags == DPrompt.ButtonFlags.YES)
			{
				showCredits();
			}
		}

		private void onSunsetPartyPromptDismissed(DPrompt.ButtonFlags buttonFlags)
		{
			switch (buttonFlags)
			{
			case DPrompt.ButtonFlags.OK:
				showCredits();
				break;
			case DPrompt.ButtonFlags.YES:
				toggleSunsetPartyAndReload();
				break;
			}
		}

		private void onSunsetPartyEvergreenPromptDismissed(DPrompt.ButtonFlags buttonFlags)
		{
			switch (buttonFlags)
			{
			case DPrompt.ButtonFlags.OK:
				showCredits();
				break;
			case DPrompt.ButtonFlags.YES:
				resetSunsetQuest();
				toggleSunsetPartyAndReload();
				break;
			}
		}

		private bool isSunsetQuestCompleted()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				string key = component.DisplayName + SunsetQuestCompletedPlayerPrefsKey;
				return PlayerPrefs.HasKey(key);
			}
			Log.LogError(this, "Could not find DisplayNameData on LocalPlayerHandle");
			return false;
		}

		private void showCredits()
		{
			Service.Get<SceneTransitionService>().LoadScene(EndCreditsScene.SceneName, null);
		}

		private void resetSunsetQuest()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				for (int i = 0; i < SunsetQuestPlayerPrefsKeys.Length; i++)
				{
					string key = component.DisplayName + SunsetQuestPlayerPrefsKeys[i];
					PlayerPrefs.DeleteKey(key);
				}
				Service.Get<ZoneTransitionService>().LoadZone(Service.Get<ZoneTransitionService>().CurrentZone);
			}
			else
			{
				Log.LogError(this, "Could not find DisplayNameData on LocalPlayerHandle");
			}
		}

		private void toggleSunsetPartyAndReload()
		{
			if (DisplayNamePlayerPrefs.HasKey(SunsetPartyActivePlayerPrefsKey))
			{
				DisplayNamePlayerPrefs.DeleteKey(SunsetPartyActivePlayerPrefsKey);
			}
			else
			{
				DisplayNamePlayerPrefs.SetInt(SunsetPartyActivePlayerPrefsKey, 1);
			}
			Service.Get<ZoneTransitionService>().LoadZone(Service.Get<ZoneTransitionService>().CurrentZone);
		}
	}
}
