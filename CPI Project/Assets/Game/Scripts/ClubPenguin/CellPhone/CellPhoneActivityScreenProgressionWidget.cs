using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenProgressionWidget : MonoBehaviour, ICellPhoneAcitivtyScreenWidget
	{
		private const string LEVEL_TEXT_TOKEN = "GoGuide.LevelRewards.Next";

		public Text TipText;

		public Text LevelText;

		public Transform RewardIconParentTransform;

		public PrefabContentKey RewardIconPrefabKey;

		private CellPhoneProgressionActivityDefinition widgetData;

		public void SetWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneProgressionActivityDefinition x = widgetData as CellPhoneProgressionActivityDefinition;
			if (x != null)
			{
				this.widgetData = x;
				showLevelText();
				showTipText(this.widgetData.TipToken);
				loadIconPrefab();
				showMascotLevelProgress();
			}
		}

		private void loadIconPrefab()
		{
			Content.LoadAsync(onIconPrefabLoadComplete, RewardIconPrefabKey);
		}

		private void onIconPrefabLoadComplete(string path, GameObject prefab)
		{
			loadRewardIcons(widgetData.RewardItems, prefab);
		}

		private void showLevelText()
		{
			int num = Service.Get<ProgressionService>().Level + 1;
			Dictionary<int, ProgressionUnlockDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, ProgressionUnlockDefinition>>();
			if (dictionary.ContainsKey(num))
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("GoGuide.LevelRewards.Next");
				tokenTranslation = string.Format(tokenTranslation, Service.Get<Localizer>().GetTokenTranslation(dictionary[num].ThemeDefinition.LongThemeToken), num);
				LevelText.text = tokenTranslation;
			}
		}

		private void showTipText(string tipToken)
		{
			TipText.text = Service.Get<Localizer>().GetTokenTranslation(tipToken);
		}

		private void loadRewardIcons(Reward reward, GameObject iconPrefab)
		{
			List<DReward> dRewardFromReward = RewardUtils.GetDRewardFromReward(reward);
			for (int i = 0; i < dRewardFromReward.Count; i++)
			{
				createRewardIcon(dRewardFromReward[i], iconPrefab);
			}
		}

		private void showMascotLevelProgress()
		{
			MascotLevelDisplay[] componentsInChildren = GetComponentsInChildren<MascotLevelDisplay>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].MascotKey.Id == widgetData.Mascot.Definition.name)
				{
					componentsInChildren[i].SetUpMascotLevel(true, null, -1L);
				}
				else
				{
					componentsInChildren[i].gameObject.SetActive(false);
				}
			}
		}

		private void createRewardIcon(DReward dReward, GameObject iconPrefab)
		{
			GameObject gameObject = Object.Instantiate(iconPrefab, RewardIconParentTransform, false);
			gameObject.GetComponent<CellPhoneActivityScreenProgressWidgetReward>().SetItem(dReward);
		}
	}
}
