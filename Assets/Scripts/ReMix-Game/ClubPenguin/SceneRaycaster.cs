using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
			if (UnityEngine.Object.FindObjectsOfType<SceneRaycaster>().Length > 1)
			{
				throw new Exception("Scene should only contain 1 SceneRaycaster.");
			}
			minSecBetweenRaycasts = 1f / (float)RayCastTouchFPS;
		}

		private TouchPhase GetTouchPhaseAndPosition(out Vector2 touchPosition)
		{
			TouchPhase result = TouchPhase.Canceled;
			if (UnityEngine.Input.touchSupported)
			{
				if (UnityEngine.Input.touchCount > 0)
				{
					touchPosition = UnityEngine.Input.GetTouch(0).position;
					return UnityEngine.Input.GetTouch(0).phase;
				}
				touchPosition = default(Vector2);
				return result;
			}
			touchPosition = UnityEngine.Input.mousePosition;
			if (UnityEngine.Input.GetMouseButtonDown(0))
			{
				result = TouchPhase.Began;
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				result = TouchPhase.Ended;
			}
			else if (UnityEngine.Input.GetMouseButton(0))
			{
				result = ((!UnityEngine.Input.mousePosition.Equals(previousMousePosition)) ? TouchPhase.Moved : TouchPhase.Stationary);
			}
			previousMousePosition = UnityEngine.Input.mousePosition;
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
			TouchPhase touchPhaseAndPosition = GetTouchPhaseAndPosition(out touchPosition);
			int num;
			switch (touchPhaseAndPosition)
			{
			case TouchPhase.Canceled:
				return;
			default:
				num = ((touchPhaseAndPosition == TouchPhase.Ended) ? 1 : 0);
				break;
			case TouchPhase.Began:
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
			bool uiWasHit = EventSystem.current.IsPointerOverGameObject();
			for (int i = 0; i < num2; i++)
			{
				RaycastHit hit = raycastHits[i];
				SceneRaycastHitListener value;
				if (transformToListenerDownState.TryGetValue(hit.transform, out value))
				{
					int num3;
					switch (touchPhaseAndPosition)
					{
					case TouchPhase.Began:
						value.DispatchTouchBegan(hit, i, uiWasHit);
						continue;
					case TouchPhase.Ended:
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
					else if (touchPhaseAndPosition == TouchPhase.Moved && value.IsTouchDown)
					{
						value.DispatchMoved(hit, i, uiWasHit);
					}
					else if (touchPhaseAndPosition == TouchPhase.Stationary && value.IsTouchDown)
					{
						value.DispatchStationary(hit, i, uiWasHit);
					}
				}
			}
			if (touchPhaseAndPosition == TouchPhase.Ended)
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
