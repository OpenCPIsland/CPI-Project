using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.UI
{
	public class TapButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
	{
		[Range(0f, 1f)]
		[Tooltip("The maximum time between the start and end of the tap, in seconds")]
		public float maxTapTime = 0.2f;

		[Range(0f, 1f)]
		[Tooltip("The maximum distance between the start and end of the tap, as a screen percent")]
		public float maxTapDist = 0.1f;

		private Vector2 touchStartPos;

		private float touchStartTime;

		private bool touchStarted = false;

		public event Action OnPressed;

		public virtual void OnPointerDown(PointerEventData pointerData)
		{
			touchStartPos = pointerData.position;
			touchStartTime = Time.time;
			touchStarted = true;
		}

		public virtual void OnPointerUp(PointerEventData pointerData)
		{
			if (touchStarted)
			{
				Vector2 vector = touchStartPos - new Vector2(pointerData.position.x, pointerData.position.y);
				vector = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
				float num = Time.time - touchStartTime;
				if (num < maxTapTime && vector.magnitude < maxTapDist && this.OnPressed != null)
				{
					this.OnPressed();
				}
			}
			touchStarted = false;
		}

		public void OnDrag(PointerEventData pointerData)
		{
		}
	}
}
