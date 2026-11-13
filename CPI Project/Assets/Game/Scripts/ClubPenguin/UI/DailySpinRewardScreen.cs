using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Net.Domain;
using ClubPenguin.Rewards;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DailySpinRewardScreen : MonoBehaviour
	{
		public enum RewardScreenState
		{
			ChestUnlocked,
			ShowingRewards,
			NewChest,
			Closed
		}

		private const string CHEST_UNLOCKED_HEADER_TOKEN = "Cellphone.DailySpin.ChestUnlocked.Title";

		public Action RewardScreenComplete;

		public Text HeaderText;

		public Text NewChestNameText;

		public GameObject ChestPanel;

		public GameObject RewardPanel;

		public GameObject RewardContainer;

		public GameObject NewChestPanel;

		public SpriteSelector ChestSpriteSelector;

		public SpriteSelector NewChestSpriteSelector;

		public float CloseTime = 3f;

		public float ExtraItemCellSize = 140f;

		private RewardScreenState currentState;

		private Localizer localizer;

		private Reward chestReward;

		private CellPhoneDailySpinActivityDefinition widgetData;

		private DailySpinEntityData spinData;

		private CellPhoneDailySpinActivityDefinition.ChestDefinition currentChestDefinition;

		private bool gotNewChest;

		private ItemImageBuilder itemImageBuilder;

		private float defaultCellSize;

		private readonly PrefabContentKey RewardItemKey = new PrefabContentKey("Rewards/RewardPopup/RewardPopupItem");

		private void OnDisable()
		{
			if (itemImageBuilder != null)
			{
				ItemImageBuilder.release();
				itemImageBuilder = null;
			}
		}

		private void Start()
		{
			defaultCellSize = RewardContainer.GetComponent<GridLayoutGroup>().cellSize.x;
		}

		public void ShowChest(Reward chestReward, CellPhoneDailySpinActivityDefinition widgetData, DailySpinEntityData spinData, CellPhoneDailySpinActivityDefinition.ChestDefinition currentChestDefinition, bool gotNewChest)
		{
			this.chestReward = chestReward;
			this.widgetData = widgetData;
			this.spinData = spinData;
			this.currentChestDefinition = currentChestDefinition;
			this.gotNewChest = gotNewChest;
			ChestSpriteSelector.SelectSprite(currentChestDefinition.ChestId);
			localizer = Service.Get<Localizer>();
			SetState(RewardScreenState.ChestUnlocked);
			base.gameObject.SetActive(true);
			itemImageBuilder = ItemImageBuilder.acquire();
			Content.LoadAsync(onRewardItemLoaded, RewardItemKey);
		}

		public void SetState(RewardScreenState newState)
		{
			switch (newState)
			{
			case RewardScreenState.ChestUnlocked:
			{
				string text = string.Format(localizer.GetTokenTranslation("Cellphone.DailySpin.ChestUnlocked.Title"), localizer.GetTokenTranslation(currentChestDefinition.ChestTypeToken));
				if (currentChestDefinition.NumChestsToNextLevel > 0)
				{
					text = string.Format("{0} {1}/{2}", text, spinData.NumChestsReceivedOfCurrentChestId, currentChestDefinition.NumChestsToNextLevel);
				}
				HeaderText.text = text;
				ChestPanel.SetActive(true);
				RewardPanel.SetActive(false);
				NewChestPanel.SetActive(false);
				break;
			}
			case RewardScreenState.ShowingRewards:
				ChestPanel.SetActive(false);
				RewardPanel.SetActive(true);
				NewChestPanel.SetActive(false);
				break;
			case RewardScreenState.NewChest:
				ChestPanel.SetActive(false);
				RewardPanel.SetActive(false);
				NewChestPanel.SetActive(true);
				CoroutineRunner.Start(waitForClose(), this, "DailySpinRewardScreenClose");
				break;
			case RewardScreenState.Closed:
				base.gameObject.SetActive(false);
				if (RewardScreenComplete != null)
				{
					RewardScreenComplete();
				}
				break;
			}
			currentState = newState;
		}

		private void onRewardItemLoaded(string path, GameObject prefab)
		{
			List<DReward> list = new List<DReward>();
			foreach (IRewardable item in chestReward)
			{
				if (!item.IsEmpty() && Enum.IsDefined(typeof(RewardCategory), item.RewardType))
				{
					RewardCategory category = (RewardCategory)Enum.Parse(typeof(RewardCategory), item.RewardType);
					Type type = item.Reward.GetType();
					if (item is EquipmentInstanceReward)
					{
						EquipmentInstanceReward equipmentInstanceReward = (EquipmentInstanceReward)item;
						for (int i = 0; i < equipmentInstanceReward.EquipmentInstances.Count; i++)
						{
							DReward dReward = new DReward();
							dReward.Category = RewardCategory.equipmentInstances;
							dReward.EquipmentRequest = equipmentInstanceReward.EquipmentInstances[i];
							list.Add(dReward);
						}
					}
					else if (typeof(IList).IsAssignableFrom(type))
					{
						IList list2 = item.Reward as IList;
						if (list2 != null && list2.Count > 0)
						{
							for (int j = 0; j < list2.Count; j++)
							{
								DReward dReward2 = new DReward();
								dReward2.UnlockID = list2[j];
								dReward2.Category = category;
								list.Add(dReward2);
							}
						}
					}
					else if (typeof(IDictionary).IsAssignableFrom(type))
					{
						IDictionary dictionary = item.Reward as IDictionary;
						if (dictionary != null && dictionary.Count > 0)
						{
							IDictionaryEnumerator enumerator2 = dictionary.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								DReward dReward2 = new DReward();
								dReward2.UnlockID = enumerator2.Key;
								dReward2.Data = enumerator2.Value;
								dReward2.Category = category;
								list.Add(dReward2);
							}
						}
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				createRewardItem(list[j], prefab);
			}
			float num = (list.Count > 2) ? ExtraItemCellSize : defaultCellSize;
			RewardContainer.GetComponent<GridLayoutGroup>().cellSize = new Vector2(num, num);
			logRewardBI(list);
		}

		private void createRewardItem(DReward rewardData, GameObject rewardItemPrefab)
		{
			RewardPopupRewardItem component = UnityEngine.Object.Instantiate(rewardItemPrefab, RewardContainer.transform, false).GetComponent<RewardPopupRewardItem>();
			component.LoadItem(rewardData.Category, rewardData, true, true);
		}

		public void OnClaimPressed()
		{
			switch (currentState)
			{
			case RewardScreenState.ChestUnlocked:
				SetState(RewardScreenState.ShowingRewards);
				break;
			case RewardScreenState.ShowingRewards:
				destroyRewardIcons();
				if (gotNewChest)
				{
					applyNextChest();
					SetState(RewardScreenState.NewChest);
				}
				else
				{
					SetState(RewardScreenState.Closed);
				}
				break;
			}
		}

		private void applyNextChest()
		{
			for (int i = 0; i < widgetData.ChestDefinitions.Count; i++)
			{
				if (widgetData.ChestDefinitions[i].ChestId == currentChestDefinition.ChestId + 1)
				{
					currentChestDefinition = widgetData.ChestDefinitions[i];
					break;
				}
			}
			NewChestSpriteSelector.SelectSprite(currentChestDefinition.ChestId);
			NewChestNameText.text = localizer.GetTokenTranslation(currentChestDefinition.ChestTypeToken);
		}

		private void destroyRewardIcons()
		{
			for (int num = RewardContainer.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(RewardContainer.transform.GetChild(num).gameObject);
			}
		}

		private IEnumerator waitForClose()
		{
			yield return new WaitForSeconds(CloseTime);
			SetState(RewardScreenState.Closed);
		}

		private void logRewardBI(List<DReward> rewards)
		{
			string tier = "";
			if (rewards.Count > 0)
			{
				tier = string.Format("{0}_{1}", rewards[0].Category.ToString(), rewards[0].UnlockID);
			}
			string tier2 = "";
			if (rewards.Count > 1)
			{
				tier2 = string.Format("{0}_{1}", rewards[1].Category.ToString(), rewards[1].UnlockID);
			}
			Service.Get<ICPSwrveService>().Action("milestone_box", tier, tier2, spinData.NumChestsReceivedOfCurrentChestId.ToString());
		}
	}
}
