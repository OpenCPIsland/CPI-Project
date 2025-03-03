using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.PartyGames
{
	public class TubeRaceEndGamePopupRow : MonoBehaviour
	{
		public Text NameText;

		public Text PlacementText;

		public Text ScoreText;

		public SpriteSelector TrophySelector;

		public GameObject TrophyGameObject;

		public Text TimeText;

		public Text ModifierText;

		public PartyGameEndPlacement MaxPlacementForTrophy;

		public TintSelector[] TintSelectors;

		public SpriteSelector ModifierSpriteSelector;

		public Text ModifierSignText;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<TubeRaceEvents.EndGameResultsReceived>(onEndGameResults);
			TrophyGameObject.SetActive(false);
		}

		public void SetData(PartyGameSessionMessages.TubeRacePlayerResult data, PartyGameDefinition partyGameDefinition)
		{
			skinForDefinition(partyGameDefinition);
			ScoreText.text = data.OverallScore.ToString();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle dataEntityHandle = cPDataEntityCollection.FindEntity<SessionIdData, long>(data.PlayerId);
			DisplayNameData component;
			if (!dataEntityHandle.IsNull && cPDataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				NameText.text = component.DisplayName;
			}
			if (data.PlayerId == cPDataEntityCollection.LocalPlayerSessionId)
			{
				setLocalPlayerData(data);
			}
		}

		private void setLocalPlayerData(PartyGameSessionMessages.TubeRacePlayerResult data)
		{
			DateTime dateTime = DateTimeUtils.DateTimeFromUnixTime(data.CompletionTimeInMilliseconds);
			TimeText.text = dateTime.ToString("mm:ss:ff");
			ModifierText.text = Math.Abs(data.ScoreModifier).ToString();
			ModifierSpriteSelector.SelectSprite((!(data.ScoreModifier >= 0f)) ? 1 : 0);
			ModifierSignText.text = ((data.ScoreModifier >= 0f) ? "+" : "-");
		}

		public void ShowPlacementText(int placement)
		{
			PlacementText.text = placement.ToString();
		}

		private bool onEndGameResults(TubeRaceEvents.EndGameResultsReceived evt)
		{
			if (evt.PlayerIdToPlacement.Count > 1 && base.transform.GetSiblingIndex() <= (int)MaxPlacementForTrophy)
			{
				TrophySelector.Select(base.transform.GetSiblingIndex() + 1);
				TrophyGameObject.SetActive(true);
			}
			return false;
		}

		private void skinForDefinition(PartyGameDefinition definition)
		{
			int index;
			switch (definition.Id)
			{
			case 4:
				index = 1;
				break;
			case 5:
				index = 0;
				break;
			default:
				index = 0;
				break;
			}
			for (int i = 0; i < TintSelectors.Length; i++)
			{
				TintSelectors[i].SelectColor(index);
			}
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<TubeRaceEvents.EndGameResultsReceived>(onEndGameResults);
		}
	}
}
