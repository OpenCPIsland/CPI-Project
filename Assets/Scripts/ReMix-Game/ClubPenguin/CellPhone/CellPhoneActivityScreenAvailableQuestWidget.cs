using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.NPC;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenAvailableQuestWidget : MonoBehaviour, ICellPhoneAcitivtyScreenWidget
	{
		[Serializable]
		public class MascotToSelectorIndex
		{
			public MascotDefinition Mascot;

			public int SelectorIndex;
		}

		public List<MascotToSelectorIndex> MascotsToSelectorIndexs;

		public Text CoinText;

		public Text XPText;

		public List<TintSelector> TintSelectors;

		public List<GameObjectSelector> GameObjectSelectors;

		public List<SpriteSelector> SpriteSelectors;

		public List<Text> TitleTexts;

		private CellPhoneQuestActivityDefinition widgetData;

		public void SetWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneQuestActivityDefinition x = widgetData as CellPhoneQuestActivityDefinition;
			if (x != null)
			{
				this.widgetData = x;
				skinForQuest(this.widgetData.Quest);
			}
		}

		public void OnGoButtonClicked()
		{
			if (widgetData != null)
			{
				startQuest(widgetData.Quest);
			}
		}

		private void skinForQuest(QuestDefinition quest)
		{
			setRewardText(quest);
			int selectedIndexForMascot = getSelectedIndexForMascot(quest.Mascot);
			setTintSelectors(selectedIndexForMascot);
			setSpriteSelectors(selectedIndexForMascot);
			setGameObjectSelectors(selectedIndexForMascot);
			setTitleText(Service.Get<Localizer>().GetTokenTranslation(quest.Title));
		}

		private int getSelectedIndexForMascot(MascotDefinition mascot)
		{
			int result = 0;
			for (int i = 0; i < MascotsToSelectorIndexs.Count; i++)
			{
				if (MascotsToSelectorIndexs[i].Mascot == mascot)
				{
					result = MascotsToSelectorIndexs[i].SelectorIndex;
					break;
				}
			}
			return result;
		}

		private void setTintSelectors(int selectedIndex)
		{
			for (int i = 0; i < TintSelectors.Count; i++)
			{
				TintSelectors[i].Select(selectedIndex);
			}
		}

		private void setSpriteSelectors(int selectedIndex)
		{
			for (int i = 0; i < SpriteSelectors.Count; i++)
			{
				SpriteSelectors[i].SelectSprite(selectedIndex);
			}
		}

		private void setGameObjectSelectors(int selectedIndex)
		{
			for (int i = 0; i < GameObjectSelectors.Count; i++)
			{
				GameObjectSelectors[i].SelectGameObject(selectedIndex);
			}
		}

		private void setTitleText(string titleText)
		{
			for (int i = 0; i < TitleTexts.Count; i++)
			{
				TitleTexts[i].text = titleText;
			}
		}

		private void setRewardText(QuestDefinition quest)
		{
			CoinReward rewardable;
			if (quest.CompleteReward.ToReward().TryGetValue(out rewardable))
			{
				CoinText.text = rewardable.Coins.ToString();
			}
			else
			{
				CoinText.enabled = false;
			}
			MascotXPReward rewardable2;
			if (quest.CompleteReward.ToReward().TryGetValue(out rewardable2))
			{
				foreach (KeyValuePair<string, int> item in rewardable2.XP)
				{
					XPText.text = item.Value.ToString();
				}
			}
		}

		private void startQuest(QuestDefinition questDefinition)
		{
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				Quest quest = Service.Get<QuestService>().GetQuest(questDefinition);
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Quest = quest;
				spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.StartQuest;
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(quest.Mascot.Definition.SpawnPlayerNearMascotPosition).Zone(quest.Mascot.Definition.Zone).SpawnedAction(spawnedAction).Build());
				if (Service.Get<SceneTransitionService>().CurrentScene == quest.Mascot.Definition.Zone.ZoneName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
				}
			}
			Service.Get<ICPSwrveService>().Action("activity_tracker", "go", "adventure", string.Format("{0}_{1}_{2}", questDefinition.Mascot.name, questDefinition.ChapterNumber, questDefinition.QuestNumber));
		}
	}
}
