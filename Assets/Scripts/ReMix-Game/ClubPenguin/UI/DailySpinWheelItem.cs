using ClubPenguin.CellPhone;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DailySpinWheelItem : MonoBehaviour
	{
		private enum CoinsXpSelectorValues
		{
			AAXp,
			RHXp,
			RKXp,
			DJXp,
			Coins,
			PartyBlaster
		}

		public GameObject CoinXPContainer;

		public GameObject ChestContainer;

		public GameObject RespinContainer;

		public SpriteSelector CoinsXpSpriteSelector;

		public Text ValueText;

		public Text RespinValueText;

		public SpriteSelector ChestSpriteSelector;

		public Image RespinIcon;

		public Image RespinCoinIcon;

		private Image sliceImage;

		private Color DIM_COLOR = new Color(0.7f, 0.7f, 0.7f);

		private Color DefaultSliceColor;

		public int ItemRewardId
		{
			get;
			private set;
		}

		public void SetReward(CellPhoneDailySpinActivityDefinition.SpinReward spinReward, CellPhoneDailySpinActivityDefinition widgetData, int currentChestId)
		{
			CoinXPContainer.SetActive(false);
			ChestContainer.SetActive(false);
			RespinContainer.SetActive(false);
			ItemRewardId = spinReward.SpinOutcomeId;
			ChestSpriteSelector.Select(currentChestId);
			CoinReward rewardable;
			if (spinReward.Reward != null)
			{
				Reward reward = spinReward.Reward.ToReward();
				MascotXPReward rewardable2;
				if (reward.TryGetValue(out rewardable) && rewardable.Coins > 0)
				{
					ValueText.text = rewardable.Coins.ToString();
					CoinsXpSpriteSelector.SelectSprite(4);
				}
				else if (reward.TryGetValue(out rewardable2))
				{
					using (Dictionary<string, int>.Enumerator enumerator = rewardable2.XP.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, int> current = enumerator.Current;
							ValueText.text = current.Value.ToString();
							int index = 0;
							if (Service.Get<ProgressionService>().IsMascotMaxLevel(current.Key))
							{
								index = 5;
								ValueText.text = "1";
							}
							else
							{
								if (current.Key == "AuntArctic")
								{
									index = 0;
								}
								if (current.Key == "Rockhopper")
								{
									index = 1;
								}
								if (current.Key == "Rookie")
								{
									index = 2;
								}
								if (current.Key == "DJCadence")
								{
									index = 3;
								}
							}
							CoinsXpSpriteSelector.SelectSprite(index);
						}
					}
				}
				CoinXPContainer.SetActive(true);
			}
			else if (spinReward.SpinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				ChestContainer.SetActive(true);
			}
			else if (spinReward.SpinOutcomeId == widgetData.RespinSpinOutcomeId)
			{
				if (widgetData.RespinReward.ToReward().TryGetValue(out rewardable))
				{
					RespinValueText.text = rewardable.Coins.ToString();
				}
				RespinContainer.SetActive(true);
			}
		}

		public void SetSliceImage(Image image)
		{
			sliceImage = image;
			DefaultSliceColor = image.color;
		}

		public void SetCurrentChestId(int id)
		{
			ChestSpriteSelector.Select(id);
		}

		public void SetDimState(bool dim, bool dimIcon)
		{
			Color color = dim ? DIM_COLOR : Color.white;
			ValueText.color = color;
			RespinValueText.color = color;
			if (dimIcon)
			{
				CoinsXpSpriteSelector.GetComponent<Image>().color = color;
				ChestSpriteSelector.GetComponent<Image>().color = color;
				RespinIcon.color = color;
				RespinCoinIcon.color = color;
			}
			else
			{
				CoinsXpSpriteSelector.GetComponent<Image>().color = Color.white;
				ChestSpriteSelector.GetComponent<Image>().color = Color.white;
				RespinIcon.color = Color.white;
				RespinCoinIcon.color = Color.white;
			}
			if (sliceImage != null)
			{
				Color color2 = dim ? (DefaultSliceColor * 0.75f) : DefaultSliceColor;
				color2.a = 1f;
				sliceImage.color = color2;
			}
		}
	}
}
