using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ClubPenguin
{
	public class SceneRaycaster : MonoBehaviour
	{
		public int RayCastTouchFPS = 20;

		private float minSecBetweenRaycasts;

		private float secElapsedBetweenRaycasts = 0f;

		private Vector3 previousMousePosition;

		private Dictionary<Transform, SceneRaycastHitListener> transformToListenerDownState = new Dictionary<Transform, SceneRaycastHitListener>();

		private static readonly int maxRaycastHits = 16;

		private RaycastHit[] raycastHits = new RaycastHit[maxRaycastHits];

		public void RegisterListener(SceneRaycastHitListener listener)
		{
			SceneRaycastHitListener value;
			if (transformToListenerDownState.TryGetValue(listener.transform, out value))
			{
				throw new ArgumentException("Listener <" + listener.gameObject.name + "> has already been registered.");
			}
			transformToListenerDownState.Add(listener.transform, listener);
		}

		public void UnRegisterListener(SceneRaycastHitListener listener)
		{
			SceneRaycastHitListener value;
			if (!transformToListenerDownState.TryGetValue(listener.transform, out value))
			{
				throw new ArgumentException("Listener <" + listener.gameObject.name + "> was not registered.");
			}
			transformToListenerDownState.Remove(listener.transform);
		}

		private void Start()
		{
			if (UnityEngine.Object.FindObjectsByType<SceneRaycaster>(FindObjectsSortMode.None).Length > 1)
			{
				throw new Exception("Scene should only contain 1 SceneRaycaster.");
			}
			minSecBetweenRaycasts = 1f / (float)RayCastTouchFPS;
		}

		private UnityEngine.TouchPhase GetTouchPhaseAndPosition(out Vector2 touchPosition)
		{
			UnityEngine.TouchPhase result = UnityEngine.TouchPhase.Canceled;

			// Use new Input System for touch
			if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
			{
				var touch = Touchscreen.current.touches[0];
				touchPosition = touch.position.ReadValue();
				if (touch.press.wasPressedThisFrame)
					return UnityEngine.TouchPhase.Began;
				if (touch.press.wasReleasedThisFrame)
					return UnityEngine.TouchPhase.Ended;
				if (!touch.press.isPressed)
					return UnityEngine.TouchPhase.Canceled;

				// If moved, compare with previousMousePosition (approximation)
				if (!touchPosition.Equals(previousMousePosition))
					result = UnityEngine.TouchPhase.Moved;
				else
					result = UnityEngine.TouchPhase.Stationary;

				previousMousePosition = touchPosition;
				return result;
			}

			// Use new Input System for mouse
			touchPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				result = UnityEngine.TouchPhase.Began;
			}
			else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
			{
				result = UnityEngine.TouchPhase.Ended;
			}
			else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
			{
				result = (!touchPosition.Equals(previousMousePosition)) ? UnityEngine.TouchPhase.Moved : UnityEngine.TouchPhase.Stationary;
			}
			previousMousePosition = touchPosition;
			return result;
		}

		private void Update()
		{
			if (Camera.main == null)
			{
				return;
			}
			secElapsedBetweenRaycasts += Time.deltaTime;
			Vector2 touchPosition;
			UnityEngine.TouchPhase touchPhaseAndPosition = GetTouchPhaseAndPosition(out touchPosition);
			int num;
			switch (touchPhaseAndPosition)
			{
				case UnityEngine.TouchPhase.Canceled:
					return;
				default:
					num = ((touchPhaseAndPosition == UnityEngine.TouchPhase.Ended) ? 1 : 0);
					break;
				case UnityEngine.TouchPhase.Began:
					num = 1;
					break;
			}
			bool flag = (byte)num != 0;
			if (secElapsedBetweenRaycasts > minSecBetweenRaycasts)
			{
				flag = true;
				secElapsedBetweenRaycasts -= minSecBetweenRaycasts;
			}
			if (!flag)
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
			int num2 = Physics.RaycastNonAlloc(ray, raycastHits);
			Array.Sort(raycastHits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
			bool uiWasHit = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
			for (int i = 0; i < num2; i++)
			{
				RaycastHit hit = raycastHits[i];
				SceneRaycastHitListener value;
				if (transformToListenerDownState.TryGetValue(hit.transform, out value))
				{
					int num3;
					switch (touchPhaseAndPosition)
					{
						case UnityEngine.TouchPhase.Began:
							value.DispatchTouchBegan(hit, i, uiWasHit);
							continue;
						case UnityEngine.TouchPhase.Ended:
							num3 = ((!value.IsTouchDown) ? 1 : 0);
							break;
						default:
							num3 = 1;
							break;
					}
					if (num3 == 0)
					{
						value.DispatchTouchEnded(hit, i, uiWasHit);
						value.IsTouchDown = false;
					}
					else if (touchPhaseAndPosition == UnityEngine.TouchPhase.Moved && value.IsTouchDown)
					{
						value.DispatchMoved(hit, i, uiWasHit);
					}
					else if (touchPhaseAndPosition == UnityEngine.TouchPhase.Stationary && value.IsTouchDown)
					{
						value.DispatchStationary(hit, i, uiWasHit);
					}
				}
			}
			if (touchPhaseAndPosition == UnityEngine.TouchPhase.Ended)
			{
				foreach (KeyValuePair<Transform, SceneRaycastHitListener> item in transformToListenerDownState)
				{
					SceneRaycastHitListener value2 = item.Value;
					value2.IsTouchDown = false;
				}
			}
		}
	}
}