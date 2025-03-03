using ClubPenguin.Adventure;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Animator))]
	public class CinematicSpeechBubble : MonoBehaviour
	{
		private const float PADDING_WIDTH = 50f;

		public Text MessageText;

		public Image ContinueImage;

		public Image BubbleBackgroundImage;

		public Image BubbleContentImage;

		public Image BubbleArrowImage;

		public RectTransform ButtonParentTransform;

		public RectTransform RectTransform;

		private DCinematicSpeech speechData;

		private Animator animator;

		private bool clickToClose = true;

		private bool isOpen = false;

		public DCinematicSpeech SpeechData
		{
			get
			{
				return speechData;
			}
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			RectTransform = GetComponent<RectTransform>();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void ShowSpeech(DCinematicSpeech speechData)
		{
			this.speechData = speechData;
			Service.Get<EventDispatcher>().AddListener<QuestEvents.MascotAudioComplete>(onMascotAudioComplete);
			MessageText.text = speechData.Text;
			if (!speechData.KeepTextStyle)
			{
				handleTextColor();
				handleTextFont();
				handleTextSize();
			}
			MessageText.supportRichText = speechData.RichText;
			handleBackgroundImage();
			handleButtons();
			handleTimeout();
			handleArrowImage();
			clickToClose = speechData.ClickToClose;
			if (ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(speechData.ShowContinueImageImmediately);
			}
			CoroutineRunner.Start(handleContentImage(), this, "CinematicSpeechBubble.handleContentImage");
			animator.SetTrigger("OpenBubble");
			setSpeechBubbleVisible(false);
			CoroutineRunner.Start(setVisibleAfterFrame(), this, "");
		}

		private void setSpeechBubbleVisible(bool visible)
		{
			Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = visible;
			}
		}

		private IEnumerator setVisibleAfterFrame()
		{
			yield return new WaitForEndOfFrame();
			setSpeechBubbleVisible(true);
		}

		public void HideSpeech()
		{
			isOpen = false;
			animator.SetTrigger("CloseBubble");
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.MascotAudioComplete>(onMascotAudioComplete);
			if (ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(false);
			}
		}

		public void OnCloseAnimationComplete()
		{
			Service.Get<EventDispatcher>().RemoveListener<ButtonEvents.ClickEvent>(onBubbleButtonClick);
			Service.Get<EventDispatcher>().DispatchEvent(new CinematicSpeechEvents.SpeechCompleteEvent(speechData));
		}

		public void OnOpenAnimationComplete()
		{
			isOpen = true;
		}

		public void OnBubbleClick()
		{
			if (clickToClose && isOpen)
			{
				EventManager.Instance.PostEvent("SFX/UI/Quest/Text/ButtonClose", EventAction.PlaySound);
				HideSpeech();
			}
		}

		private bool onMascotAudioComplete(QuestEvents.MascotAudioComplete evt)
		{
			if (speechData.ClickToClose && isOpen && ContinueImage != null)
			{
				ContinueImage.gameObject.SetActive(true);
			}
			return false;
		}

		private void handleTextColor()
		{
			if (speechData.TextStyle != null && !string.IsNullOrEmpty(speechData.TextStyle.ColorHex))
			{
				if (!string.IsNullOrEmpty(speechData.TextStyle.ColorHex))
				{
					MessageText.color = hexToColor(speechData.TextStyle.ColorHex);
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
				MessageText.fontSize = Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.SpeechBubbleFontSize;
			}
		}

		private void handleButtons()
		{
			if (speechData.Buttons != null && speechData.Buttons.Length > 0)
			{
				ButtonParentTransform.gameObject.SetActive(true);
				for (int i = 0; i < speechData.Buttons.Length; i++)
				{
					CoroutineRunner.Start(loadButton(speechData.Buttons[i]), this, "CinematicSpeechBubble.loadButton");
				}
				Service.Get<EventDispatcher>().AddListener<ButtonEvents.ClickEvent>(onBubbleButtonClick);
			}
			else
			{
				ButtonParentTransform.gameObject.SetActive(false);
			}
		}

		private IEnumerator loadButton(DButton buttonData)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync<GameObject>(buttonData.ButtonPrefabKey);
			yield return assetRequest;
			GameObject buttonGameObject = Object.Instantiate(assetRequest.Asset);
			buttonGameObject.transform.SetParent(ButtonParentTransform, false);
			ButtonController buttonController = buttonGameObject.GetComponent<ButtonController>();
			buttonController.ShowButton(buttonData);
		}

		private bool onBubbleButtonClick(ButtonEvents.ClickEvent evt)
		{
			Button[] componentsInChildren = ButtonParentTransform.GetComponentsInChildren<Button>();
			foreach (Button button in componentsInChildren)
			{
				button.interactable = false;
			}
			if (isOpen)
			{
				HideSpeech();
			}
			return false;
		}

		private void handleTimeout()
		{
			CancelInvoke();
			if (speechData.DismissTime > 0f)
			{
				Invoke("HideSpeech", speechData.DismissTime);
			}
		}

		private IEnumerator handleContentImage()
		{
			if (!string.IsNullOrEmpty(speechData.ContentImageKey))
			{
				BubbleContentImage.gameObject.SetActive(true);
				yield return CoroutineRunner.Start(loadImage(speechData.ContentImageKey, BubbleContentImage), this, "CinematicSpeechBubble.loadImage");
				BubbleContentImage.color = Color.white;
			}
			else
			{
				BubbleContentImage.gameObject.SetActive(false);
			}
		}

		private void handleArrowImage()
		{
			BubbleArrowImage.gameObject.SetActive(!speechData.HideTail);
			BubbleArrowImage.color = Service.Get<MascotService>().GetMascot(speechData.MascotName).Definition.DialogBubbleArrowColor;
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
			if (assetRequest.Asset != null)
			{
				destinationImage.sprite = assetRequest.Asset;
			}
		}

		private Color hexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, byte.MaxValue);
		}
	}
}
