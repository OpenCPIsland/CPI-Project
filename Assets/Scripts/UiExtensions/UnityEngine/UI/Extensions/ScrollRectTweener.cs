using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectTweener")]
	public class ScrollRectTweener : MonoBehaviour, IDragHandler, IEventSystemHandler
	{
		private ScrollRect scrollRect;

		private Vector2 startPos;

		private Vector2 targetPos;

		private bool wasHorizontal;

		private bool wasVertical;

		public float moveSpeed = 5000f;

		public bool disableDragWhileTweening = false;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			wasHorizontal = scrollRect.horizontal;
			wasVertical = scrollRect.vertical;
		}

		public void ScrollHorizontal(float normalizedX)
		{
			Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition));
		}

		public void ScrollHorizontal(float normalizedX, float duration)
		{
			Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition), duration);
		}

		public void ScrollVertical(float normalizedY)
		{
			Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY));
		}

		public void ScrollVertical(float normalizedY, float duration)
		{
			Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY), duration);
		}

		public void Scroll(Vector2 normalizedPos)
		{
			Scroll(normalizedPos, GetScrollDuration(normalizedPos));
		}

		private float GetScrollDuration(Vector2 normalizedPos)
		{
			Vector2 currentPos = GetCurrentPos();
			return Vector2.Distance(DeNormalize(currentPos), DeNormalize(normalizedPos)) / moveSpeed;
		}

		private Vector2 DeNormalize(Vector2 normalizedPos)
		{
			return new Vector2(normalizedPos.x * scrollRect.content.rect.width, normalizedPos.y * scrollRect.content.rect.height);
		}

		private Vector2 GetCurrentPos()
		{
			return new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
		}

		public void Scroll(Vector2 normalizedPos, float duration)
		{
			startPos = GetCurrentPos();
			targetPos = normalizedPos;
			if (disableDragWhileTweening)
			{
				LockScrollability();
			}
			StopAllCoroutines();
			StartCoroutine(DoMove(duration));
		}

		private IEnumerator DoMove(float duration)
		{
			if (!(duration < 0.05f))
			{
				Vector2 posOffset = targetPos - startPos;
				float currentTime = 0f;
				while (currentTime < duration)
				{
					currentTime += Time.deltaTime;
					scrollRect.normalizedPosition = EaseVector(currentTime, startPos, posOffset, duration);
					yield return null;
				}
				scrollRect.normalizedPosition = targetPos;
				if (disableDragWhileTweening)
				{
					RestoreScrollability();
				}
			}
		}

		public Vector2 EaseVector(float currentTime, Vector2 startValue, Vector2 changeInValue, float duration)
		{
			return new Vector2(changeInValue.x * Mathf.Sin(currentTime / duration * ((float)Math.PI / 2f)) + startValue.x, changeInValue.y * Mathf.Sin(currentTime / duration * ((float)Math.PI / 2f)) + startValue.y);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!disableDragWhileTweening)
			{
				StopScroll();
			}
		}

		private void StopScroll()
		{
			StopAllCoroutines();
			if (disableDragWhileTweening)
			{
				RestoreScrollability();
			}
		}

		private void LockScrollability()
		{
			scrollRect.horizontal = false;
			scrollRect.vertical = false;
		}

		private void RestoreScrollability()
		{
			scrollRect.horizontal = wasHorizontal;
			scrollRect.vertical = wasVertical;
		}
	}
}
