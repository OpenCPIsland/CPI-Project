using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Input;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestMessageHud : MonoBehaviour
	{
		private const string DEFAULT_CANVAS_PARENT_NAME = "HudLeftCanvas";

		private const string TOP_CANVAS_PARENT_NAME = "TopCanvas";

		private const float MESSAGE_TAP_DELAY = 0.8f;

		public Image MascotImage;

		public ButtonClickListener FullScreenButton;

		public Text MessageText;

		public Image ContinueImage;

		public Image BubbleBackgroundImage;

		public Image BubbleArrowImage;

		public Image CommunicatorIconImage;

		private DQuestMessage speechData;

		private EventChannel eventChannel;

		private bool clickToClose;

		private Transform defaultCanvasParentTransform;

		private Transform topCanvasParentTransform;

		private RectTransform questHudTransform;

		public MonoBehaviour[] componentsToToggle;

		public bool IsOpening
		{
			get;
			private set;
		}

		public bool IsOpen
		{
			get;
			private set;
		}

		public DQuestMessage SpeechData
		{
			get
			{
				return speechData;
			}
		}

		public event System.Action MessageClosed;

		public event System.Action MascotImageLoaded;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<HudEvents.ShowQuestMessage>(onShowQuestMessage);
			findCanvasTransforms();
			FullScreenButton.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private void OnEnable()
		{
			FullScreenButton.OnClick.AddListener(onBubbleClicked);
		}

		private void OnDisable()
		{
			FullScreenButton.OnClick.RemoveListener(onBubbleClicked);
			CancelInvoke();
		}

		public void ToggleActive(bool active)
		{
			for (int i = 0; i < componentsToToggle.Length; i++)
			{
				if (componentsToToggle[i].enabled != active)
				{
					componentsToToggle[i].enabled = active;
				}
			}
		}

		public void OnMessageOpenAnimationComplete()
		{
		}

		private bool onShowQuestMessage(HudEvents.ShowQuestMessage evt)
		{
			IsOpening = true;
			CoroutineRunner.Start(showQuestMessage(evt), this, "QuestMessageHud.showQuestMessage");
			return false;
		}

		private IEnumerator showQuestMessage(HudEvents.ShowQuestMessage evt)
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(evt.SpeechData.MascotName);
			Service.Get<EventDispatcher>().AddListener<QuestEvents.MascotAudioComplete>(onMascotAudioComplete);
			if (ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(false);
			}
			if (mascot == null)
			{
				Log.LogError(this, "QuestMascotMessageHud recieved a ShowQuestMessage event with a mascot that was not present in the mascot service: " + evt.SpeechData.MascotName);
				yield break;
			}
			yield return CoroutineRunner.Start(loadImageSprite(mascot.Definition.CommunicatorIconContentKey, CommunicatorIconImage), this, "LoadIconImage");
			yield return CoroutineRunner.Start(loadImageSprite(mascot.Definition.HudAvatarContentKey, MascotImage), this, "LoadMascotImage");
			if (this.MascotImageLoaded != null)
			{
				this.MascotImageLoaded();
			}
			SetSpeechData(evt.SpeechData);
			setCanvasParent(topCanvasParentTransform);
			questHudTransform.anchorMin = Vector2.zero;
			questHudTransform.anchorMax = Vector2.one;
			questHudTransform.sizeDelta = Vector2.zero;
			questHudTransform.anchoredPosition = Vector2.zero;
			yield return new WaitForSeconds(0.8f);
			IsOpening = false;
			IsOpen = true;
			FullScreenButton.gameObject.SetActive(evt.SpeechData.ClickToClose);
		}

		private void onBubbleClicked(ButtonClickListener.ClickType clickType)
		{
			if (clickToClose && IsOpen)
			{
				closeBubble();
			}
		}

		private bool onMascotAudioComplete(QuestEvents.MascotAudioComplete evt)
		{
			if (speechData.ClickToClose && IsOpen && ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(true);
			}
			return false;
		}

		private void closeBubble()
		{
			CancelInvoke();
			IsOpen = false;
			FullScreenButton.gameObject.SetActive(false);
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.MascotAudioComplete>(onMascotAudioComplete);
			if (ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(false);
			}
			if (this.MessageClosed != null)
			{
				this.MessageClosed();
			}
		}

		public void ResetMessage()
		{
			MessageText.text = "";
			BubbleBackgroundImage.sprite = null;
			BubbleBackgroundImage.enabled = false;
			if (ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(false);
			}
			setCanvasParent(defaultCanvasParentTransform);
			IsOpen = false;
		}

		private IEnumerator loadImageSprite(SpriteContentKey imageContentPath, Image image)
		{
			AssetRequest<Sprite> request = Content.LoadAsync(imageContentPath);
			yield return request;
			image.sprite = request.Asset;
		}

		public void SetSpeechData(DQuestMessage speechData)
		{
			this.speechData = speechData;
			handleTextColor();
			handleTextFont();
			handleTextSize();
			handleBackgroundImage();
			handleArrowImage();
			handleTimeout();
			clickToClose = speechData.ClickToClose;
		}

		private void handleTextColor()
		{
			MessageText.text = speechData.Text;
			MessageText.supportRichText = speechData.RichText;
			if (speechData.TextStyle != null && !string.IsNullOrEmpty(speechData.TextStyle.ColorHex))
			{
				if (!string.IsNullOrEmpty(speechData.TextStyle.ColorHex))
				{
					MessageText.color = ColorUtils.HexToColor(speechData.TextStyle.ColorHex);
				}
			}
			else if (!string.IsNullOrEmpty(speechData.MascotName))
			{
				MessageText.color = Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.TextColor;
			}
		}

		private void handleTextFont()
		{
			if (speechData.TextStyle != null && !string.IsNullOrEmpty(speechData.TextStyle.FontContentKey))
			{
				CoroutineRunner.Start(loadFont(speechData.TextStyle.FontContentKey), this, "CinematicSpeechBubble.loadFont");
			}
			else if (!string.IsNullOrEmpty(speechData.MascotName))
			{
				CoroutineRunner.Start(loadFont(Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.FontContentKey.Key), this, "CinematicSpeechBubble.loadFont");
			}
		}

		private IEnumerator loadFont(string fontContentKey)
		{
			AssetRequest<Font> assetRequest = Content.LoadAsync<Font>(fontContentKey);
			yield return assetRequest;
			MessageText.font = assetRequest.Asset;
		}

		private void handleTextSize()
		{
			if (speechData.TextStyle != null && speechData.TextStyle.FontSize != 0)
			{
				MessageText.fontSize = speechData.TextStyle.FontSize;
			}
			else if (!string.IsNullOrEmpty(speechData.MascotName))
			{
				MessageText.fontSize = Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.MessageBubbleFontSize;
			}
		}

		private void handleBackgroundImage()
		{
			if (!string.IsNullOrEmpty(speechData.BackgroundImageKey))
			{
				CoroutineRunner.Start(loadImage(speechData.BackgroundImageKey, BubbleBackgroundImage), this, "CinematicSpeechBubble.loadImage");
			}
			else if (!string.IsNullOrEmpty(speechData.MascotName))
			{
				CoroutineRunner.Start(loadImage(Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.BubbleContentKey.Key, BubbleBackgroundImage), this, "CinematicSpeechBubble.loadImage");
			}
		}

		private IEnumerator loadImage(string imageContentKey, Image destinationImage)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync<Sprite>(imageContentKey);
			yield return assetRequest;
			destinationImage.sprite = assetRequest.Asset;
			destinationImage.enabled = true;
			destinationImage.gameObject.SetActive(true);
		}

		private void handleArrowImage()
		{
			BubbleArrowImage.color = Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.DialogBubbleArrowColor;
		}

		private void handleTimeout()
		{
			CancelInvoke();
			if (speechData.DismissTime > 0f)
			{
				Invoke("closeBubble", speechData.DismissTime);
			}
		}

		private void findCanvasTransforms()
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_HUD);
			defaultCanvasParentTransform = gameObject.transform.Find("HudLeftCanvas").GetChild(0);
			topCanvasParentTransform = gameObject.transform.Find("TopCanvas");
			questHudTransform = (GetComponentInParent<AdjustRectPositionForNotification>().transform as RectTransform);
		}

		private void setCanvasParent(Transform parent)
		{
			if (questHudTransform.parent != parent)
			{
				questHudTransform.SetParent(parent, false);
			}
		}
	}
}
