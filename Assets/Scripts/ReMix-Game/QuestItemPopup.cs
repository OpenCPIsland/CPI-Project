using ClubPenguin.Input;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemPopup : AnimatedPopup
{
	private const float TWEEN_TIME = 0.25f;

	private const float TWEEN_INTERVAL = 0.25f;

	public const string TweenDestinationName = "MainNavButton_Quest";

	public Text HeaderText;

	public ButtonClickListener TakeItemButton;

	public RectTransform ItemPanel;

	public Transform ItemPrefab;

	public string NotificationText;

	private Transform[] Images;

	private bool hasLastTweenCompleted;

	protected override void awake()
	{
		AutoOpen = false;
	}

	protected override void popupOpenAnimationComplete()
	{
		TakeItemButton.OnClick.AddListener(onCollectClicked);
		base.popupOpenAnimationComplete();
	}

	private void OnDisable()
	{
		TakeItemButton.OnClick.RemoveListener(onCollectClicked);
	}

	private void onCollectClicked(ButtonClickListener.ClickType clickType)
	{
		TakeItemButton.OnClick.RemoveListener(onCollectClicked);
		hasLastTweenCompleted = false;
		CoroutineRunner.Start(tweenImage(), this, "");
	}

	public void SetData(DQuestItemPopup questItemPopupData)
	{
		if (!string.IsNullOrEmpty(questItemPopupData.Message))
		{
			HeaderText.text = questItemPopupData.Message;
		}
		else
		{
			HeaderText.gameObject.SetActive(false);
		}
		NotificationText = questItemPopupData.NotificationMessage;
		if (questItemPopupData.ItemInfos != null)
		{
			Images = new Transform[questItemPopupData.ItemInfos.Length];
			for (int i = 0; i < questItemPopupData.ItemInfos.Length; i++)
			{
				Images[i] = UnityEngine.Object.Instantiate(ItemPrefab);
				Images[i].SetParent(ItemPanel);
				CoroutineRunner.Start(loadImage(i, questItemPopupData.ItemInfos[i], questItemPopupData.ItemInfos.Length), this, "loadQuestItemImage");
			}
		}
	}

	private IEnumerator tweenImage()
	{
		GameObject tweenDestinationGO = GameObject.Find("MainNavButton_Quest");
		RoundedSinTweener[] tweens = GetComponentsInChildren<RoundedSinTweener>();
		for (int j = 0; j < tweens.Length; j++)
		{
			tweens[j].transform.parent = base.transform;
		}
		if (tweenDestinationGO == null || tweens.Length == 0)
		{
			onTweenComplete();
			yield break;
		}
		RoundedSinTweener obj = tweens[tweens.Length - 1];
		obj.TweenCompleteAction = (Action)Delegate.Combine(obj.TweenCompleteAction, new Action(onTweenComplete));
		for (int i = 0; i < tweens.Length; i++)
		{
			tweens[i].StartTween(tweenDestinationGO.transform, 0.25f);
			yield return new WaitForSeconds(0.25f);
		}
	}

	private void onTweenComplete()
	{
		if (!hasLastTweenCompleted)
		{
			hasLastTweenCompleted = true;
			ShowNotification();
			ClosePopup();
		}
	}

	private void ShowNotification()
	{
		if (!string.IsNullOrEmpty(NotificationText))
		{
			DNotification dNotification = new DNotification();
			dNotification.Message = NotificationText;
			dNotification.ContainsButtons = false;
			dNotification.PopUpDelayTime = 3f;
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
		}
	}

	private IEnumerator loadImage(int imageIndex, QuestItemPopupImageInfo imageInfo, int numImages)
	{
		AssetRequest<Sprite> assetRequest = Content.LoadAsync<Sprite>(imageInfo.ItemImagePath);
		yield return assetRequest;
		string itemName = Service.Get<Localizer>().GetTokenTranslation(imageInfo.i18nItemNameText);
		Images[imageIndex].GetComponent<QuestItemPopupImage>().LoadItem(itemName, assetRequest.Asset, imageInfo.ItemCount);
		if (OpenDelay == 0f || openDelayComplete)
		{
			OpenPopup();
		}
	}

	protected override void onDestroy()
	{
		base.onDestroy();
		if (!hasLastTweenCompleted)
		{
			hasLastTweenCompleted = true;
			ShowNotification();
		}
		CoroutineRunner.StopAllForOwner(this);
	}
}
