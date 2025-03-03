using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.PartyGames;
using ClubPenguin.SledRacer;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRacePartyGameSession : AbstractPartyGameSession
	{
		private readonly PrefabContentKey endGameKey = new PrefabContentKey("Prefabs/TubeRace2Screen");

		private readonly List<PartyGameSessionMessages.TubeRacePlayerResult> playerResults;

		private JsonService jsonService;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		private TubeRaceDefinition tubeRaceDefinition;

		private PartyGameDefinition partyGameDefinition;

		private Dictionary<long, int> gameEnded = null;

		public TubeRacePartyGameSession()
		{
			jsonService = Service.Get<JsonService>();
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			playerResults = new List<PartyGameSessionMessages.TubeRacePlayerResult>();
			addEventListeners();
		}

		private void addEventListeners()
		{
			eventChannel.AddListener<TubeRaceEvents.ScoreModifierCollected>(onScoreModifierCollected);
			eventChannel.AddListener<RaceGameEvents.RaceFinished>(onLocalPlayerFinished);
		}

		protected override void handleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
			switch (type)
			{
			case PartyGameSessionMessageTypes.SetTubeRaceModifierLayout:
				handleSetTubeRaceModifierLayout(data);
				break;
			case PartyGameSessionMessageTypes.TubeRacePlayerResult:
				handleTubeRacePlayerResult(data);
				break;
			}
		}

		private bool onScoreModifierCollected(TubeRaceEvents.ScoreModifierCollected evt)
		{
			sendSessionMessage(PartyGameSessionMessageTypes.CollectTubeRaceScoreModifier, new PartyGameSessionMessages.CollectTubeRaceScoreModifier(evt.ScoreModifierId));
			return false;
		}

		private bool onLocalPlayerFinished(RaceGameEvents.RaceFinished evt)
		{
			sendSessionMessage(PartyGameSessionMessageTypes.PlayerFinishedTubeRace, new PartyGameSessionMessages.PlayerFinishedTubeRace());
			return false;
		}

		protected override void startGame()
		{
			partyGameDefinition = getPartyGameDefinition(partyGameId);
			tubeRaceDefinition = (TubeRaceDefinition)partyGameDefinition.GameData;
			eventDispatcher.DispatchEvent(new TubeRaceEvents.RaceStart((PartyGameDefinition.GameTypes)partyGameId));
			Service.Get<ICPSwrveService>().Action("tube_race_start", "start", base.players.Count.ToString());
		}

		protected override void endGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			eventDispatcher.DispatchEvent(new TubeRaceEvents.RaceEnd((PartyGameDefinition.GameTypes)partyGameId));
			eventDispatcher.DispatchEvent(new TubeRaceEvents.EndGameResultsReceived(partyGameDefinition.Id, playerSessionIdToPlacement, playerResults));
			showLocalPlayerHeadStatus(playerSessionIdToPlacement);
			gameEnded = playerSessionIdToPlacement;
		}

		protected override void destroy()
		{
			eventChannel.RemoveAllListeners();
			PartyGameUtils.EnableCellPhoneButton();
			eventDispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
		}

		private void handleSetTubeRaceModifierLayout(string data)
		{
			try
			{
				PartyGameSessionMessages.SetTubeRaceModifierLayout setTubeRaceModifierLayout = jsonService.Deserialize<PartyGameSessionMessages.SetTubeRaceModifierLayout>(data);
				eventDispatcher.DispatchEvent(new TubeRaceEvents.SetModifierLayout((PartyGameDefinition.GameTypes)partyGameId, setTubeRaceModifierLayout.ModifierLayoutId));
			}
			catch (Exception)
			{
			}
		}

		private void handleTubeRacePlayerResult(string data)
		{
			PartyGameSessionMessages.TubeRacePlayerResult tubeRacePlayerResult = null;
			try
			{
				tubeRacePlayerResult = jsonService.Deserialize<PartyGameSessionMessages.TubeRacePlayerResult>(data);
			}
			catch (Exception)
			{
			}
			if (tubeRacePlayerResult != null)
			{
				playerResults.Add(tubeRacePlayerResult);
				if (tubeRacePlayerResult.PlayerId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId)
				{
					int num = (int)Math.Round(tubeRacePlayerResult.OverallScore);
					Service.Get<ICPSwrveService>().Action("tube_race_finish", num.ToString());
					showEndGameScreen();
				}
				else
				{
					eventDispatcher.DispatchEvent(new TubeRaceEvents.PlayerResultReceived(tubeRacePlayerResult));
				}
			}
		}

		private void showEndGameScreen()
		{
			Content.LoadAsync(onEndGameScreenLoaded, endGameKey);
		}

		private void onEndGameScreenLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
			gameObject.GetComponent<TubeRaceEndGamePopup>().SetInitialData(base.sessionId, playerResults, partyGameDefinition, tubeRaceDefinition);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(gameObject));
			if (gameEnded != null)
			{
				gameObject.GetComponent<TubeRaceEndGamePopup>().EndGame(gameEnded);
			}
		}

		private void showLocalPlayerHeadStatus(Dictionary<long, int> playerSessionIdToPlacement)
		{
			int value;
			if (playerSessionIdToPlacement.Count > 1 && playerSessionIdToPlacement.TryGetValue(Service.Get<CPDataEntityCollection>().LocalPlayerSessionId, out value))
			{
				int num = value + 2;
				if (num <= 4 && num >= 2)
				{
					eventDispatcher.DispatchEvent(new HeadStatusEvents.ShowHeadStatus((TemporaryHeadStatusType)num));
				}
			}
		}
	}
}
