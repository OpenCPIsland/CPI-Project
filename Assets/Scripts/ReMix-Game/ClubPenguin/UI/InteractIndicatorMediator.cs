using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.SledRacer;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(InteractInWorldIconController))]
	public class InteractIndicatorMediator : MonoBehaviour
	{
		private EventChannel eventChannel;

		private ControlsScreenData controlsScreenData;

		private InteractInWorldIconController interactInWorldIconController;

		private bool isControlsScreenActive;

		private bool isActionSequenceInProgress;

		private bool isRaceInProgress;

		private void Start()
		{
			interactInWorldIconController = GetComponent<InteractInWorldIconController>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<ControlsScreenData>();
			controlsScreenData = cPDataEntityCollection.GetComponent<ControlsScreenData>(entityByType);
			interactInWorldIconController.SetEnabled(controlsScreenData.IsControlsScreenActive);
			controlsScreenData.OnControlsScreenActiveChanged += onControlsScreenActiveChanged;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<ActionSequencerEvents.ActionSequenceStarted>(onActionSequenceStarted);
			eventChannel.AddListener<ActionSequencerEvents.ActionSequenceCompleted>(onActionSequenceCompleted);
			eventChannel.AddListener<TubeRaceEvents.LocalPlayerJoinedLobby>(onRaceLobbyEntered);
			eventChannel.AddListener<TubeRaceEvents.LocalPlayerLeftLobby>(onRaceLobbyLeft);
			eventChannel.AddListener<RaceGameEvents.Start>(onRaceGameStart);
			eventChannel.AddListener<RaceGameEvents.RaceFinished>(onRaceFinished);
		}

		private void enable()
		{
			if (isControlsScreenActive && !isActionSequenceInProgress && !isRaceInProgress)
			{
				interactInWorldIconController.SetEnabled(true);
			}
		}

		private void disable()
		{
			interactInWorldIconController.SetEnabled(false);
		}

		private void onControlsScreenActiveChanged(bool isControlsScreenActive)
		{
			this.isControlsScreenActive = isControlsScreenActive;
			if (isControlsScreenActive)
			{
				enable();
			}
			else
			{
				disable();
			}
		}

		private bool onActionSequenceStarted(ActionSequencerEvents.ActionSequenceStarted evt)
		{
			isActionSequenceInProgress = true;
			disable();
			return false;
		}

		private bool onActionSequenceCompleted(ActionSequencerEvents.ActionSequenceCompleted evt)
		{
			isActionSequenceInProgress = false;
			enable();
			return false;
		}

		private bool onRaceLobbyEntered(TubeRaceEvents.LocalPlayerJoinedLobby evt)
		{
			enterRace();
			return false;
		}

		private bool onRaceLobbyLeft(TubeRaceEvents.LocalPlayerLeftLobby evt)
		{
			exitRace();
			return false;
		}

		private bool onRaceGameStart(RaceGameEvents.Start evt)
		{
			enterRace();
			return false;
		}

		private bool onRaceFinished(RaceGameEvents.RaceFinished evt)
		{
			exitRace();
			return false;
		}

		private void enterRace()
		{
			isRaceInProgress = true;
			disable();
		}

		private void exitRace()
		{
			isRaceInProgress = false;
			enable();
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (controlsScreenData != null)
			{
				controlsScreenData.OnControlsScreenActiveChanged -= onControlsScreenActiveChanged;
			}
		}
	}
}
