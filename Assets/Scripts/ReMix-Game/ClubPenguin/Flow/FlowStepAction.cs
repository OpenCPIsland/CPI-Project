using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Flow
{
	public class FlowStepAction : ScriptableAction
	{
		[Serializable]
		public struct Option
		{
			public string ButtonName;

			public ScriptableAction NextStep;
		}

		public GameObject PopupPrefab;

		public ScriptableAction SetupAction;

		public Option[] Options;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			GameObject popup = UnityEngine.Object.Instantiate(PopupPrefab);
			popup.name = base.name + "Popup";
			for (int i = 0; i < Options.Length; i++)
			{
				setupButton(i, popup, player);
			}
			if (SetupAction != null)
			{
				ScriptableActionPlayer popupPlayer = popup.AddComponent<ScriptableActionPlayer>();
				popupPlayer.Action = SetupAction;
				popupPlayer.enabled = true;
				while (popupPlayer != null && popupPlayer.enabled)
				{
					yield return null;
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup));
			while (!player.ActionIsFinished)
			{
				yield return null;
			}
			UnityEngine.Object.Destroy(popup);
		}

		private void setupButton(int index, GameObject popup, ScriptableActionPlayer player)
		{
			Option option = Options[index];
			Transform transform = popup.transform.Find(option.ButtonName);
			if (transform == null)
			{
				Log.LogErrorFormatted(player, "Flow Step {0}: Could not find button '{1}' in prefab popup '{2}'", base.name, option.ButtonName, PopupPrefab.name);
			}
			Button component = transform.GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				onClick(player, option.NextStep);
			});
		}

		private void onClick(ScriptableActionPlayer player, ScriptableAction nextStep)
		{
			player.NextAction = nextStep;
			player.ActionIsFinished = true;
		}
	}
}
