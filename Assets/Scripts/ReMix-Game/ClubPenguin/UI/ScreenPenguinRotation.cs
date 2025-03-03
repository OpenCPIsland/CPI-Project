using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ScreenPenguinRotation : MonoBehaviour
	{
		private Transform rotationTransform;

		private float rotationMultiplier = 0.5f;

		private Vector2 previousTouchPosition = Vector2.zero;

		private Quaternion initialRotation;

		private float maxTouchPositionY;

		private float minTouchPositionY;

		private float worldContainerHeight;

		private RectTransform worldContainerTransform;

		private Canvas canvas;

		public void Start()
		{
			canvas = GetComponentInParent<Canvas>();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			Transform transform = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).transform;
			worldContainerTransform = (transform.Find("WorldContainer") as RectTransform);
			if (worldContainerTransform == null)
			{
				Transform transform2 = transform.Find("VertLayout");
				if (transform2 != null)
				{
					worldContainerTransform = (transform2.Find("WorldContainer") as RectTransform);
				}
			}
			if (worldContainerTransform == null)
			{
				Log.LogError(this, "Could not find the world container rect transform");
			}
			if (localPlayerGameObject != null)
			{
				rotationTransform = localPlayerGameObject.GetComponent<Transform>();
				initialRotation = rotationTransform.rotation;
			}
		}

		private void Update()
		{
			checkInput();
		}

		public void OnDestroy()
		{
			if (rotationTransform != null)
			{
				rotationTransform.rotation = initialRotation;
			}
		}

		private void checkInput()
		{
			Vector2 lhs = Vector2.zero;
			int touchCount = UnityEngine.Input.touchCount;
			if (touchCount > 0)
			{
				Touch touch = UnityEngine.Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began && isInputWithinRect(touch.position))
				{
					previousTouchPosition = touch.position;
				}
				else if (touch.phase == TouchPhase.Moved && isInputWithinRect(touch.position))
				{
					lhs = touch.position;
				}
			}
			if (lhs != Vector2.zero)
			{
				float num = lhs.x - previousTouchPosition.x;
				float num2 = rotationMultiplier * num;
				rotationTransform.Rotate(new Vector3(0f, num2 * -1f, 0f));
				previousTouchPosition = lhs;
			}
		}

		private bool isInputWithinRect(Vector2 inputPosition)
		{
			if (worldContainerTransform != null && Math.Abs(worldContainerHeight - worldContainerTransform.rect.height) > float.Epsilon)
			{
				worldContainerHeight = worldContainerTransform.rect.height;
				float num = (1f - worldContainerTransform.pivot.y) * worldContainerHeight + worldContainerTransform.anchoredPosition.y;
				maxTouchPositionY = canvas.pixelRect.height + num;
				minTouchPositionY = maxTouchPositionY - worldContainerHeight;
			}
			return inputPosition.y > minTouchPositionY && inputPosition.y < maxTouchPositionY;
		}
	}
}
