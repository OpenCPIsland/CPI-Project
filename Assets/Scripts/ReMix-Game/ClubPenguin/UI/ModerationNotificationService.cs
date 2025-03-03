using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ModerationNotificationService : MonoBehaviour
	{
		private Queue<IModerationAlert> queuedAlerts;

		private bool isShowingMessage;

		private void Start()
		{
			queuedAlerts = new Queue<IModerationAlert>();
			Service.Get<EventDispatcher>().AddListener<ModerationServiceEvents.ShowAlerts>(onShowAlerts);
		}

		private bool onShowAlerts(ModerationServiceEvents.ShowAlerts evt)
		{
			foreach (IModerationAlert alert in evt.Alerts)
			{
				enqueueAlert(alert);
			}
			checkAndDisplayMessage();
			return false;
		}

		private void checkAndDisplayMessage()
		{
			if (!isShowingMessage && queuedAlerts.Count > 0)
			{
				isShowingMessage = true;
				IModerationAlert alert = queuedAlerts.Peek();
				PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition(alert.IsCritical ? "ModerationCriticalPrompt" : "ModerationWarningPrompt");
				PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, delegate(PromptLoaderCMD loader)
				{
					showModerationPrompt(loader, alert);
				});
				promptLoaderCMD.Execute();
			}
		}

		private void showModerationPrompt(PromptLoaderCMD promptLoader, IModerationAlert alert)
		{
			if (alert.IsCritical)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				DisplayNameData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_TITLE, component.DisplayName, true);
				}
				else
				{
					Log.LogError(this, "Could not find display name data on local player");
				}
				promptLoader.PromptData.SetText("Moderation.Text.Time", "11hrs 26m", true);
			}
			promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_BODY, alert.Text, true);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onModerationPromptClosed, promptLoader.Prefab);
		}

		private void onModerationPromptClosed(DPrompt.ButtonFlags button)
		{
			Service.Get<NetworkController>().ClearAlert(queuedAlerts.Dequeue());
			isShowingMessage = false;
			checkAndDisplayMessage();
		}

		private void enqueueAlert(IModerationAlert alert)
		{
			if (!containsText(alert))
			{
				queuedAlerts.Enqueue(alert);
			}
		}

		private bool containsText(IModerationAlert alert)
		{
			foreach (IModerationAlert queuedAlert in queuedAlerts)
			{
				if (queuedAlert.Text == alert.Text)
				{
					return true;
				}
			}
			return false;
		}
	}
}
