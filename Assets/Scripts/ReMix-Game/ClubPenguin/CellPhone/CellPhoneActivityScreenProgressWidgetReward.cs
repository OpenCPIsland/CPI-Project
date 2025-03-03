using ClubPenguin.Core;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenProgressWidgetReward : MonoBehaviour
	{
		public Image IconImage;

		public GameObject MemberLockPanel;

		private IRewardIconRenderer rewardRenderer;

		private void Awake()
		{
			IconImage.color = Color.clear;
			ItemImageBuilder.acquire();
		}

		private void OnDestroy()
		{
			ItemImageBuilder.release();
			if (rewardRenderer != null)
			{
				CoroutineRunner.StopAllForOwner(rewardRenderer);
			}
		}

		public void SetItem(DReward item)
		{
			loadItemIcon(item);
			showMemberStatus(item);
		}

		private void loadItemIcon(DReward item)
		{
			RewardIconRendererFactory rewardIconRendererFactory = new RewardIconRendererFactory();
			rewardRenderer = rewardIconRendererFactory.GetRewardIconRenderer(item.Category);
			rewardRenderer.RenderReward(item, onIconRenderComplete);
		}

		private void onIconRenderComplete(Sprite icon, RectTransform iconPrefab, string itemName)
		{
			if (icon != null)
			{
				IconImage.sprite = icon;
				IconImage.color = Color.white;
			}
			else if (iconPrefab != null)
			{
				iconPrefab.SetParent(IconImage.transform, false);
			}
		}

		private void showMemberStatus(DReward item)
		{
			if (MemberLockPanel != null)
			{
				MemberLockPanel.SetActive(!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && item.Category != RewardCategory.emotePacks);
			}
		}
	}
}
