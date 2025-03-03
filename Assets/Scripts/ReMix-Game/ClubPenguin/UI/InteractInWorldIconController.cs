#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.Participation;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Canvas))]
	public class InteractInWorldIconController : MonoBehaviour
	{
		public enum InteractInWorldIconType
		{
			None,
			UICanvas,
			UICanvasPenguin
		}

		private static InteractInWorldIconType tweakableType;

		private InteractInWorldIconType type;

		public PrefabContentKey InteractIndicatorContentKey;

		public float YOffset;

		public bool IsFollowingPenguin;

		private Canvas canvas;

		private Camera gameCamera;

		private RectTransform indicatorTransform;

		private InteractIndicator interactIndicator;

		private InteractIndicatorOffset interactIndicatorOffset;

		private PropService propService;

		private Vector3 lastPosition;

		private ParticipationData participationData;

		private MembershipData membershipData;

		private GameObjectReferenceData gameObjectReferenceData;

		private bool isEnabled;

		private AssetRequest<GameObject> indicatorRequest;

		private Camera GameCamera
		{
			get
			{
				if (gameCamera == null)
				{
					gameCamera = Camera.main;
				}
				return gameCamera;
			}
		}

		public Canvas MyCanvas
		{
			get
			{
				if (canvas == null)
				{
					canvas = GetComponent<Canvas>();
				}
				return canvas;
			}
		}

		[Invokable("UI.WorldUI.SetInteractInWorldIconType")]
		public static void SetInteractInWorldIconType(InteractInWorldIconType type)
		{
			tweakableType = type;
			InteractInWorldIconController interactInWorldIconController = Object.FindObjectOfType<InteractInWorldIconController>();
			if (interactInWorldIconController != null)
			{
				interactInWorldIconController.setState(type);
			}
		}

		private void setState(InteractInWorldIconType type)
		{
			switch (type)
			{
			case InteractInWorldIconType.None:
				base.gameObject.SetActive(false);
				break;
			case InteractInWorldIconType.UICanvas:
				base.gameObject.SetActive(true);
				IsFollowingPenguin = false;
				break;
			case InteractInWorldIconType.UICanvasPenguin:
				base.gameObject.SetActive(true);
				IsFollowingPenguin = true;
				break;
			}
		}

		private void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			participationData = cPDataEntityCollection.GetComponent<ParticipationData>(cPDataEntityCollection.LocalPlayerHandle);
			membershipData = cPDataEntityCollection.GetComponent<MembershipData>(cPDataEntityCollection.LocalPlayerHandle);
			gameObjectReferenceData = cPDataEntityCollection.GetComponent<GameObjectReferenceData>(cPDataEntityCollection.LocalPlayerHandle);
			type = InteractInWorldIconType.UICanvas;
			type = ((tweakableType != 0) ? tweakableType : type);
			setState(type);
			propService = Service.Get<PropService>();
			indicatorRequest = Content.LoadAsync(onInteractIndicatorLoaded, InteractIndicatorContentKey);
		}

		private void onInteractIndicatorLoaded(string path, GameObject interactIndicatorPrefab)
		{
			if (!(base.gameObject == null) && indicatorRequest != null)
			{
				GameObject gameObject = Object.Instantiate(interactIndicatorPrefab, base.transform);
				indicatorTransform = (gameObject.transform as RectTransform);
				Assert.IsNotNull(indicatorTransform);
				interactIndicator = gameObject.GetComponent<InteractIndicator>();
				Assert.IsNotNull(interactIndicator);
				gameObject.SetActive(false);
				indicatorRequest = null;
			}
		}

		public void SetEnabled(bool enabled)
		{
			isEnabled = enabled;
			if (!enabled && interactIndicator != null)
			{
				interactIndicator.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			if (indicatorRequest != null)
			{
				indicatorRequest.Cancelled = true;
				indicatorRequest = null;
			}
		}

		private void Update()
		{
			if (!(indicatorTransform != null) || !(interactIndicator != null))
			{
				return;
			}
			if (isEnabled)
			{
				float yOffset = YOffset;
				if ((propService.LocalPlayerPropUser == null || (propService.LocalPlayerPropUser.Prop == null && propService.LocalPlayerPropUser.IsPropUseCompleted)) && participationData != null && participationData.IsInteractButtonAvailable && participationData.ParticipatingGO != null && participationData.ParticipatingGO.Value != null && participationData.CurrentParticipationState == ParticipationState.Pending)
				{
					Vector3 zero = Vector3.zero;
					if (IsFollowingPenguin)
					{
						zero = gameObjectReferenceData.GameObject.transform.position;
					}
					else
					{
						zero = participationData.ParticipatingGO.Value.transform.position;
						interactIndicatorOffset = participationData.ParticipatingGO.Value.GetComponent<InteractIndicatorOffset>();
						if (interactIndicatorOffset != null)
						{
							yOffset = interactIndicatorOffset.Offset;
						}
					}
					setCanvasPosition(zero, yOffset);
					lastPosition = zero;
					MemberAccess component = participationData.ParticipatingGO.Value.GetComponent<MemberAccess>();
					bool flag = membershipData.IsMember || component == null || !component.IsMemberLocked;
					interactIndicator.SetMemberLockedStatus(!flag);
					interactIndicator.Show();
					return;
				}
				if (interactIndicator.gameObject.activeSelf)
				{
					Vector3 zero = Vector3.zero;
					zero = ((!IsFollowingPenguin) ? lastPosition : gameObjectReferenceData.GameObject.transform.position);
					if (interactIndicatorOffset != null)
					{
						yOffset = interactIndicatorOffset.Offset;
					}
					setCanvasPosition(zero, yOffset);
				}
				interactIndicator.Hide();
			}
			else
			{
				interactIndicator.gameObject.SetActive(false);
			}
		}

		private void setCanvasPosition(Vector3 target, float yOffset)
		{
			if (MyCanvas != null && GameCamera != null && GameCamera.isActiveAndEnabled)
			{
				target.y += yOffset;
				Vector3 a = GameCamera.WorldToScreenPoint(target);
				Vector3 v = a * (1f / MyCanvas.scaleFactor);
				indicatorTransform.anchoredPosition = v;
			}
		}
	}
}
