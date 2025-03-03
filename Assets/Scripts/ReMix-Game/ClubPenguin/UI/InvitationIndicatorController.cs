using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InvitationIndicatorController : MonoBehaviour
	{
		private const float TWEEN_TIME = 0.5f;

		private const string CONSUMABLE_NAVBUTTON_NAME = "MainNavButton_Consumables";

		private const string ANIMATOR_INTRO = "Open";

		private const string ANIMATOR_EXIT = "Close";

		[SerializeField]
		private Text AvailableQuantityText;

		[SerializeField]
		private RadialProgressBar AvailableQuantityRadial;

		[SerializeField]
		private Image ItemImage;

		public Animator IndicatorAnimator;

		private RoundedSinTweener tween;

		private WorldSpeechBubble speechBubble;

		private WorldChatController worldChatController;

		public int TotalQuantity
		{
			get;
			set;
		}

		public int AvailableQuantity
		{
			get
			{
				return AvailableQuantity;
			}
			set
			{
				if (AvailableQuantityText != null)
				{
					AvailableQuantityText.text = value.ToString();
				}
				if (AvailableQuantityRadial != null && TotalQuantity > 0)
				{
					AvailableQuantityRadial.SetProgress((float)value / (float)TotalQuantity);
				}
			}
		}

		public SpriteContentKey ItemImageContentKey
		{
			get
			{
				return ItemImageContentKey;
			}
			set
			{
				if (ItemImage != null && value != null && !string.IsNullOrEmpty(value.Key))
				{
					CoroutineRunner.Start(loadItemImage(value), this, "loadItemImage");
				}
			}
		}

		private WorldChatController chatController
		{
			get
			{
				if (worldChatController == null)
				{
					worldChatController = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).transform.parent.GetComponentInChildren<WorldChatController>();
				}
				return worldChatController;
			}
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage, EventDispatcher.Priority.LAST);
			Service.Get<EventDispatcher>().AddListener<ChatServiceEvents.SendChatActivity>(onSendChatActivity, EventDispatcher.Priority.LAST);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
			Service.Get<EventDispatcher>().RemoveListener<ChatServiceEvents.SendChatActivity>(onSendChatActivity);
			if (speechBubble != null)
			{
				speechBubble.OnCompleteEvent -= onChatComplete;
			}
		}

		private bool onSendChatActivity(ChatServiceEvents.SendChatActivity evt)
		{
			hideIndicatorForChat();
			return false;
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			hideIndicatorForChat();
			return false;
		}

		private void hideIndicatorForChat()
		{
			if (speechBubble == null)
			{
				speechBubble = chatController.GetActiveSpeechBubble(Service.Get<CPDataEntityCollection>().LocalPlayerSessionId);
				speechBubble.OnCompleteEvent += onChatComplete;
				IndicatorAnimator.SetTrigger("Close");
			}
		}

		private void onChatComplete(WorldSpeechBubble bubble)
		{
			speechBubble.OnCompleteEvent -= onChatComplete;
			IndicatorAnimator.SetTrigger("Open");
			speechBubble = null;
		}

		private IEnumerator loadItemImage(SpriteContentKey contentKey)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(contentKey);
			yield return assetRequest;
			ItemImage.sprite = assetRequest.Asset;
			ItemImage.gameObject.SetActive(true);
		}

		public void TweenToMainNav()
		{
			base.transform.SetParent(GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).transform, true);
			IndicatorAnimator.enabled = false;
			tween = base.gameObject.AddComponent<RoundedSinTweener>();
			RoundedSinTweener roundedSinTweener = tween;
			roundedSinTweener.TweenCompleteAction = (System.Action)Delegate.Combine(roundedSinTweener.TweenCompleteAction, new System.Action(onTweenToMainNavComplete));
			tween.DestinationScale = Vector3.zero;
			tween.CurveDampener = 20f;
			GameObject gameObject = GameObject.Find("MainNavButton_Consumables");
			if (gameObject != null)
			{
				tween.StartTween(gameObject.transform, 0.5f);
			}
			else
			{
				onTweenToMainNavComplete();
			}
		}

		private void onTweenToMainNavComplete()
		{
			RoundedSinTweener roundedSinTweener = tween;
			roundedSinTweener.TweenCompleteAction = (System.Action)Delegate.Remove(roundedSinTweener.TweenCompleteAction, new System.Action(onTweenToMainNavComplete));
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
