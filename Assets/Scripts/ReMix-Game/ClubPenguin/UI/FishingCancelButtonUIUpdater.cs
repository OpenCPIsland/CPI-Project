using ClubPenguin.Adventure;
using ClubPenguin.MiniGames.Fishing;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FishingCancelButtonUIUpdater : MonoBehaviour
	{
		private Button button;

		private TrayInputButton trayInputButton;

		private EventChannel eventChannel;

		private void Start()
		{
			button = GetComponentInParent<Button>();
			trayInputButton = button.GetComponent<TrayInputButton>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<FishingEvents.SetFishingState>(onSetFishingState);
			eventChannel.AddListener<FishingEvents.FishingGameplayStateChanged>(onFishingGameplayStateChanged);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onSetFishingState(FishingEvents.SetFishingState evt)
		{
			if (evt.State == FishingEvents.FishingState.Hold)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Default);
			}
			else if (evt.State == FishingEvents.FishingState.Cast)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Disabled);
			}
			return false;
		}

		private bool onFishingGameplayStateChanged(FishingEvents.FishingGameplayStateChanged evt)
		{
			if (evt.State == FishingController.FishingGameplayStates.Catch)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Default);
			}
			else if (evt.State == FishingController.FishingGameplayStates.Reel)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Disabled);
			}
			return false;
		}
	}
}
