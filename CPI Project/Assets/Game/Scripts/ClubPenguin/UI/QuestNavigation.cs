using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestNavigation : MonoBehaviour
	{
		private enum QuestNavigationState
		{
			inactive,
			offscreen,
			onscreen
		}

		public RectTransform TransformRotation;

		public RectTransform TransformDerotation;

		public RectTransform TransformPosition;

		public GameObject OnScreenGO;

		public GameObject OffScreenGO;

		public Image NavArrow;

		public Image NavCircle;

		public float PADDING_LEFT = 0.07f;

		public float PADDING_RIGHT = 0.07f;

		public float PADDING_TOP = 0.15f;

		public float PADDING_BOTTOM = 0.15f;

		public float MOVEMENT_SPEED = 25f;

		public float ARROW_SCALE_DIST = 40f;

		public float MIN_ARROW_SCALE = 0.5f;

		public float DIST_TEXT_FADE_START_DIST = 5f;

		public float DIST_TEXT_FADE_END_DIST = 3f;

		public Vector3 OnscreenIndicatorOffset = new Vector3(0f, 2f, 0f);

		private bool showOnscreenIndicator;

		private Transform targetTransform;

		private Vector3 customOnscreenIndicatorOffset = Vector3.zero;

		private RectTransform worldContainerRect;

		private RectTransform childContainerRect;

		private QuestNavigationState currentState;

		private bool isOnScreenVisible;

		private bool isOffScreenVisible;

		private bool isDisabled = false;

		private Vector2 desiredIndicatorPos = Vector2.zero;

		private Quaternion desiredIndicatorRot = Quaternion.identity;

		private SpriteContentKey navArrowContentKey = new SpriteContentKey("Sprites/Quests_Inworld_Nav_Arrow_*");

		private SpriteContentKey navCircleContentKey = new SpriteContentKey("Sprites/Quests_Nav_Spot_*");

		private EventChannel eventChannel;

		private void Start()
		{
			Vector2 vector2 = TransformPosition.anchorMax = (TransformPosition.anchorMin = Vector2.zero);
			worldContainerRect = GetComponent<RectTransform>();
			childContainerRect = GetComponent<RectTransform>();
			currentState = QuestNavigationState.inactive;
			OffScreenGO.gameObject.SetActive(false);
			OnScreenGO.gameObject.SetActive(false);
			showOnscreenIndicator = true;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<HudEvents.SetNavigationTarget>(onNavigationTargetSet);
			eventChannel.AddListener<CinematicSpeechEvents.ShowSpeechEvent>(onShowSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(onSpeechComplete);
			eventChannel.AddListener<TrayEvents.TrayOpened>(onTrayOpened);
			eventChannel.AddListener<TrayEvents.TrayClosed>(onTrayClosed);
			eventChannel.AddListener<HudEvents.HideQuestNavigation>(onHideQuestNavigation);
			eventChannel.AddListener<HudEvents.ShowQuestNavigation>(onShowQuestNavigation);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Update()
		{
			if (currentState != 0 && targetTransform != null)
			{
				updateIndicatorTransform();
			}
		}

		private bool onNavigationTargetSet(HudEvents.SetNavigationTarget evt)
		{
			targetTransform = evt.TargetTransform;
			showOnscreenIndicator = evt.ShowOnscreenIndicator;
			customOnscreenIndicatorOffset = evt.OnscreenIndicatorOffset;
			if (targetTransform != null)
			{
				updateIndicatorTransform();
				if (currentState == QuestNavigationState.inactive)
				{
					TransformPosition.anchoredPosition = desiredIndicatorPos;
				}
			}
			else
			{
				setState(QuestNavigationState.inactive);
			}
			return false;
		}

		private bool onShowSpeech(CinematicSpeechEvents.ShowSpeechEvent evt)
		{
			hideQuestNavigation();
			return false;
		}

		private bool onSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			showQuestNavigation();
			return false;
		}

		private void updateIndicatorTransform()
		{
			Vector3 viewportPos = Camera.main.WorldToViewportPoint(targetTransform.position);
			bool snap = false;
			if (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0.11f || viewportPos.y > 1f || viewportPos.z < 0f)
			{
				if (currentState != QuestNavigationState.offscreen)
				{
					setState(QuestNavigationState.offscreen);
					snap = true;
				}
				updateOffscreenIndicatorTransform(ref viewportPos);
			}
			else
			{
				if (currentState != QuestNavigationState.onscreen)
				{
					setState(QuestNavigationState.onscreen);
					snap = true;
				}
				updateOnscreenIndicatorTransform();
			}
			applyDesiredIndicatorTransform(snap);
		}

		private Vector2 calcDesiredScreenOffset()
		{
			return new Vector2(worldContainerRect.rect.width - childContainerRect.rect.width, 0f - (worldContainerRect.rect.height - childContainerRect.rect.height));
		}

		private void applyDesiredIndicatorTransform(bool snap)
		{
			TransformPosition.anchoredPosition = (snap ? desiredIndicatorPos : Vector2.Lerp(TransformPosition.anchoredPosition, desiredIndicatorPos, Time.deltaTime * MOVEMENT_SPEED));
			TransformRotation.localRotation = desiredIndicatorRot;
		}

		private void updateOnscreenIndicatorTransform()
		{
			Vector3 vector = Camera.main.WorldToViewportPoint(targetTransform.position + OnscreenIndicatorOffset + customOnscreenIndicatorOffset);
			desiredIndicatorPos.x = Mathf.Clamp(vector.x, PADDING_LEFT, 1f - PADDING_RIGHT);
			desiredIndicatorPos.y = Mathf.Clamp(vector.y, PADDING_BOTTOM, 1f - PADDING_TOP);
			desiredIndicatorPos.x = worldContainerRect.rect.width * vector.x;
			desiredIndicatorPos.y = worldContainerRect.rect.height * vector.y;
			desiredIndicatorPos += calcDesiredScreenOffset();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			float num = Vector3.Distance(targetTransform.position, localPlayerGameObject.transform.position);
			float num2 = Vector3.Distance(targetTransform.position, Camera.main.transform.position);
			float b = 1f - Mathf.Max(0f, num2 / ARROW_SCALE_DIST);
			b = Mathf.Max(MIN_ARROW_SCALE, b);
			float a = 1f;
			if (num < DIST_TEXT_FADE_START_DIST)
			{
				float num3 = num - DIST_TEXT_FADE_END_DIST;
				a = num3 / (DIST_TEXT_FADE_START_DIST - DIST_TEXT_FADE_END_DIST);
			}
			OnScreenGO.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, a);
			OnScreenGO.GetComponentInChildren<Text>().text = string.Format("{0}m", (int)num);
			OnScreenGO.transform.localScale = new Vector3(b, b, 1f);
		}

		private void updateOffscreenIndicatorTransform(ref Vector3 viewportPos)
		{
			Vector2 vector = new Vector2(worldContainerRect.rect.width, worldContainerRect.rect.height);
			if (viewportPos.z < 0f)
			{
				viewportPos.x = 0f - viewportPos.x;
				viewportPos.y = 0f - viewportPos.y;
			}
			Vector2 vector2 = viewportPos;
			vector2.x -= 0.5f;
			vector2.y -= 0.5f;
			float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f;
			desiredIndicatorRot = Quaternion.Euler(0f, 0f, z);
			if (TransformDerotation != null)
			{
				Vector3 eulerAngles = TransformRotation.rotation.eulerAngles;
				eulerAngles.z = 0f;
				TransformDerotation.rotation = Quaternion.Euler(eulerAngles);
			}
			float num = vector2.y / vector2.x;
			vector2.y = Mathf.Clamp(vector2.y, 0f - (0.5f - PADDING_BOTTOM), 0.5f - PADDING_TOP);
			vector2.x = vector2.y / num;
			vector2.x = Mathf.Clamp(vector2.x, 0f - (0.5f - PADDING_LEFT), 0.5f - PADDING_RIGHT);
			vector2.x = (vector2.x + 0.5f) * vector.x;
			vector2.y = (vector2.y + 0.5f) * vector.y;
			desiredIndicatorPos = vector2;
			desiredIndicatorPos += calcDesiredScreenOffset();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			float num2 = Vector3.Distance(targetTransform.position, localPlayerGameObject.transform.position);
			OffScreenGO.GetComponentInChildren<Text>().text = string.Format("{0}m", (int)num2);
		}

		[Invokable("Quest.SetNavigationTarget")]
		public static void TweakerSetNavigationTarget(string targetName)
		{
			GameObject gameObject = GameObject.Find(targetName);
			if (gameObject != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SetNavigationTarget(gameObject.transform));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SetNavigationTarget(null));
			}
		}

		private void setState(QuestNavigationState newState)
		{
			if (currentState != newState)
			{
				switch (newState)
				{
				case QuestNavigationState.inactive:
					isOffScreenVisible = false;
					isOnScreenVisible = false;
					break;
				case QuestNavigationState.offscreen:
					isOffScreenVisible = true;
					isOnScreenVisible = false;
					break;
				case QuestNavigationState.onscreen:
					isOffScreenVisible = false;
					isOnScreenVisible = showOnscreenIndicator;
					break;
				}
				if (currentState == QuestNavigationState.inactive)
				{
					updateNavigationSprites();
				}
				else
				{
					updateVisibility();
				}
				currentState = newState;
			}
		}

		private void updateNavigationSprites()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				string abbreviatedName = activeQuest.Mascot.AbbreviatedName;
				Content.LoadAsync(onNavArrowLoaded, navArrowContentKey, abbreviatedName);
				Content.LoadAsync(onNavCircleLoaded, navCircleContentKey, abbreviatedName);
			}
			else
			{
				updateVisibility();
			}
		}

		private void updateVisibility()
		{
			OffScreenGO.SetActive(isOffScreenVisible && !isDisabled);
			OnScreenGO.SetActive(isOnScreenVisible && !isDisabled);
		}

		private void onNavArrowLoaded(string path, Sprite navArrow)
		{
			NavArrow.sprite = navArrow;
			OnScreenGO.SetActive(isOnScreenVisible && !isDisabled);
		}

		private void onNavCircleLoaded(string path, Sprite navCircle)
		{
			NavCircle.sprite = navCircle;
			OffScreenGO.SetActive(isOffScreenVisible && !isDisabled);
		}

		private bool onTrayOpened(TrayEvents.TrayOpened evt)
		{
			if (currentState == QuestNavigationState.offscreen)
			{
				OffScreenGO.SetActive(false);
			}
			return false;
		}

		private bool onTrayClosed(TrayEvents.TrayClosed evt)
		{
			if (currentState == QuestNavigationState.offscreen)
			{
				OffScreenGO.SetActive(true);
			}
			return false;
		}

		private bool onHideQuestNavigation(HudEvents.HideQuestNavigation evt)
		{
			hideQuestNavigation();
			return false;
		}

		private bool onShowQuestNavigation(HudEvents.ShowQuestNavigation evt)
		{
			showQuestNavigation();
			return false;
		}

		private void hideQuestNavigation()
		{
			OnScreenGO.SetActive(false);
			OffScreenGO.SetActive(false);
			isDisabled = true;
		}

		private void showQuestNavigation()
		{
			if (currentState == QuestNavigationState.onscreen)
			{
				OnScreenGO.SetActive(true);
			}
			else if (currentState == QuestNavigationState.offscreen)
			{
				OffScreenGO.SetActive(true);
			}
			isDisabled = false;
		}
	}
}
