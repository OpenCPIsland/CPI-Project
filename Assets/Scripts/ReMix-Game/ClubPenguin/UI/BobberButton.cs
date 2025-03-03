using ClubPenguin.MiniGames.Fishing;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class BobberButton : MonoBehaviour
	{
		private TrayInputButton trayInputButton;

		private EventChannel eventChannel;

		private void Start()
		{
			Button componentInParent = GetComponentInParent<Button>();
			trayInputButton = componentInParent.GetComponent<TrayInputButton>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<FishingEvents.ActivateBobberButton>(onActivateBobberButton);
			eventChannel.AddListener<FishingEvents.DeactivateBobberButton>(onDeactivateBobberButton);
			eventChannel.AddListener<FishingEvents.PulseBobberButton>(onPulseBobberButton);
			eventChannel.AddListener<FishingEvents.StopBobberButtonPulse>(onTopBobberButtonPulse);
		}

		private bool onActivateBobberButton(FishingEvents.ActivateBobberButton evt)
		{
			trayInputButton.SetState(TrayInputButton.ButtonState.Default);
			return false;
		}

		private bool onDeactivateBobberButton(FishingEvents.DeactivateBobberButton evt)
		{
			trayInputButton.SetState(TrayInputButton.ButtonState.Disabled);
			return false;
		}

		private bool onPulseBobberButton(FishingEvents.PulseBobberButton evt)
		{
			trayInputButton.SetState(TrayInputButton.ButtonState.Pulsing);
			return false;
		}

		private bool onTopBobberButtonPulse(FishingEvents.StopBobberButtonPulse evt)
		{
			trayInputButton.SetState(TrayInputButton.ButtonState.Default);
			return false;
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}
	}
}
