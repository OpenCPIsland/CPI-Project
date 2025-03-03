using ClubPenguin.Adventure;
using ClubPenguin.Input;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Canvas))]
	public class CinematicSpeechController : MonoBehaviour
	{
		private const string DEFAULT_SPEECH_PREFAB = "Prefabs/Quest/CinematicSpeechBubbles/CinematicSpeechBubbleDynamic";

		[SerializeField]
		private GameObject fullScreenButton = null;

		private Canvas canvas;

		private List<CinematicSpeechBubble> activeSpeechBubbles;

		private Camera gameCamera;

		private EventChannel eventChannel;

		public void Awake()
		{
			canvas = GetComponent<Canvas>();
			activeSpeechBubbles = new List<CinematicSpeechBubble>();
			fullScreenButton.SetActive(false);
		}

		public void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CinematicSpeechEvents.ShowSpeechEvent>(onShowCinematicSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.HideSpeechEvent>(onHideCinematicSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.HideAllSpeechEvent>(onHideAllCinematicSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(onCinematicSpeechComplete);
			gameCamera = Camera.main;
		}

		private void OnEnable()
		{
			fullScreenButton.GetComponent<ButtonClickListener>().OnClick.AddListener(onFullscreenButtonClicked);
		}

		private void OnDisable()
		{
			fullScreenButton.GetComponent<ButtonClickListener>().OnClick.RemoveListener(onFullscreenButtonClicked);
		}

		public void Update()
		{
			updateSpeechBubblePositions();
		}

		private void updateSpeechBubblePositions()
		{
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				updateSpeechBubblePosition(activeSpeechBubbles[i]);
			}
		}

		private void updateSpeechBubblePosition(CinematicSpeechBubble bubble)
		{
			if (bubble.SpeechData.MascotTarget == null)
			{
				float x = canvas.pixelRect.width / 2f * (1f / canvas.scaleFactor);
				float y = canvas.pixelRect.height / 2f * (1f / canvas.scaleFactor);
				bubble.RectTransform.anchoredPosition = new Vector2(x, y);
				return;
			}
			Vector3 vector = bubble.SpeechData.MascotTarget.transform.position;
			MascotController component = bubble.SpeechData.MascotTarget.GetComponent<MascotController>();
			if (component != null)
			{
				vector += component.QuestIndicatorOffset * 0.6f;
			}
			vector.y += (float)Screen.height * bubble.SpeechData.OffsetYPercent + bubble.SpeechData.OffsetY;
			vector = gameCamera.WorldToScreenPoint(vector);
			vector.z = 0f;
			if (bubble.SpeechData.CenterX)
			{
				vector.x = (float)Screen.width * 0.5f;
			}
			if (bubble.SpeechData.CenterY)
			{
				vector.y = (float)Screen.height * 0.5f + (float)Screen.height * bubble.SpeechData.OffsetYPercent + bubble.SpeechData.OffsetY;
			}
			vector.x *= 1f / canvas.scaleFactor;
			vector.y *= 1f / canvas.scaleFactor;
			bubble.RectTransform.anchoredPosition = vector;
		}

		public void OnDestroy()
		{
			clearSpeechBubbles();
			eventChannel.RemoveAllListeners();
		}

		private void onFullscreenButtonClicked(ButtonClickListener.ClickType clickType)
		{
			clickActiveSpeechBubbles();
		}

		private void clickActiveSpeechBubbles()
		{
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				activeSpeechBubbles[i].OnBubbleClick();
			}
		}

		private void clearSpeechBubbles()
		{
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				if (!activeSpeechBubbles[i].gameObject.IsDestroyed())
				{
					Object.Destroy(activeSpeechBubbles[i].gameObject);
				}
			}
			activeSpeechBubbles.Clear();
			if (!fullScreenButton.IsDestroyed())
			{
				fullScreenButton.SetActive(false);
			}
		}

		private void clearSpeechBubble(CinematicSpeechBubble speechBubble)
		{
			activeSpeechBubbles.Remove(speechBubble);
			Object.Destroy(speechBubble.gameObject);
			fullScreenButton.SetActive(clickToClose());
		}

		private bool onShowCinematicSpeech(CinematicSpeechEvents.ShowSpeechEvent evt)
		{
			CoroutineRunner.Start(showSpeech(evt.SpeechData), this, "showSpeech");
			return false;
		}

		private bool onHideCinematicSpeech(CinematicSpeechEvents.HideSpeechEvent evt)
		{
			CinematicSpeechBubble cinematicSpeechBubble = speechDataToActiveBubble(evt.SpeechData);
			if (cinematicSpeechBubble != null)
			{
				cinematicSpeechBubble.HideSpeech();
			}
			return false;
		}

		private bool onHideAllCinematicSpeech(CinematicSpeechEvents.HideAllSpeechEvent evt)
		{
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				activeSpeechBubbles[i].HideSpeech();
			}
			return false;
		}

		private bool onCinematicSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			clearSpeechBubble(speechDataToActiveBubble(evt.SpeechData));
			return false;
		}

		private IEnumerator showSpeech(DCinematicSpeech speechData)
		{
			if (string.IsNullOrEmpty(speechData.MascotName) && Service.Get<QuestService>().ActiveQuest != null)
			{
				speechData.MascotName = Service.Get<QuestService>().ActiveQuest.Mascot.Name;
			}
			GameObject mascotGO = getMascotFromName(speechData.MascotName);
			if (mascotGO != null)
			{
				speechData.MascotTarget = mascotGO;
			}
			else
			{
				speechData.HideTail = true;
			}
			if (string.IsNullOrEmpty(speechData.BubbleContentKey))
			{
				speechData.BubbleContentKey = "Prefabs/Quest/CinematicSpeechBubbles/CinematicSpeechBubbleDynamic";
			}
			AssetRequest<GameObject> assetRequest = Content.LoadAsync<GameObject>(speechData.BubbleContentKey);
			yield return assetRequest;
			GameObject speechGameObject = Object.Instantiate(assetRequest.Asset);
			speechGameObject.transform.SetParent(canvas.transform, false);
			CinematicSpeechBubble cinematicSpeechBubble = speechGameObject.GetComponent<CinematicSpeechBubble>();
			activeSpeechBubbles.Add(cinematicSpeechBubble);
			cinematicSpeechBubble.ShowSpeech(speechData);
			fullScreenButton.SetActive(clickToClose());
		}

		private bool clickToClose()
		{
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				if (activeSpeechBubbles[i].SpeechData.ClickToClose)
				{
					return true;
				}
			}
			return false;
		}

		private GameObject getMascotFromName(string mascotName)
		{
			GameObject gameObject = null;
			GameObject[] array = GameObject.FindGameObjectsWithTag("NPC");
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name == mascotName)
				{
					gameObject = array[i];
					break;
				}
			}
			if (gameObject == null)
			{
				gameObject = GameObject.Find(mascotName);
			}
			return gameObject;
		}

		private CinematicSpeechBubble speechDataToActiveBubble(DCinematicSpeech speechData)
		{
			CinematicSpeechBubble result = null;
			for (int i = 0; i < activeSpeechBubbles.Count; i++)
			{
				if (activeSpeechBubbles[i].SpeechData == speechData)
				{
					result = activeSpeechBubbles[i];
				}
			}
			return result;
		}
	}
}
