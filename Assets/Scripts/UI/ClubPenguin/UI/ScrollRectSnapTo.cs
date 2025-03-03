using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSnapTo : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum HorizontalSnapPosition
		{
			left,
			right,
			center
		}

		public enum VerticalSnapPosition
		{
			top,
			bottom,
			center
		}

		private const float MIN_SPEED = 0.01f;

		private const float MIN_SNAP_SPEED = 0.1f;

		public HorizontalSnapPosition HorizontalSnapPos = HorizontalSnapPosition.center;

		public VerticalSnapPosition VerticalSnapPos = VerticalSnapPosition.center;

		public float SnapAtSpeed = 100f;

		public float SnapTweenTimeSec = 0.5f;

		private bool snapped = true;

		private bool isDragging = false;

		private bool isPointerDown = false;

		private bool isTweeningToSnapPoint;

		private ScrollRect scrollRect;

		private Vector2 snapPoint;

		private Vector2 tweenDirection = default(Vector2);

		private Vector3 tweenPositionHelper = default(Vector3);

		private float tweenTimeElapsedSec = 0f;

		private float SnapTweenTimeSecRecip = 1f;

		private int previousNumContentChildren = 0;

		private GameObject selectedChild;

		public event Action<GameObject> EElementSelected;

		public event Action EStartedScrolling;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			snapped = false;
			isTweeningToSnapPoint = false;
		}

		private void OnDestroy()
		{
			selectedChild = null;
			this.EElementSelected = null;
			this.EStartedScrolling = null;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			isPointerDown = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isPointerDown = false;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			isDragging = true;
			snapped = false;
			isTweeningToSnapPoint = false;
			if (this.EStartedScrolling != null)
			{
				this.EStartedScrolling();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			isDragging = false;
		}

		private bool numContentChildrenChanged()
		{
			if (previousNumContentChildren != scrollRect.content.childCount)
			{
				previousNumContentChildren = scrollRect.content.childCount;
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (scrollRect.content == null || scrollRect.content.childCount == 0)
			{
				return;
			}
			restrictBounds();
			if (isDragging || isPointerDown)
			{
				return;
			}
			if (numContentChildrenChanged())
			{
				CoroutineRunner.Start(determineSnapToElementAndStartTween(), this, "determineSnapTpElementAndStartTween");
				snapped = true;
			}
			else if (isTweeningToSnapPoint)
			{
				tweenToSnapPoint();
			}
			else
			{
				if (snapped)
				{
					return;
				}
				float magnitude = scrollRect.velocity.magnitude;
				if (magnitude <= SnapAtSpeed)
				{
					if (SnapAtSpeed < 0.1f)
					{
						SnapAtSpeed = 0.1f;
					}
					snapped = true;
					CoroutineRunner.Start(determineSnapToElementAndStartTween(), this, "determineSnapToElementAndStartTween");
				}
			}
		}

		private IEnumerator determineSnapToElementAndStartTween()
		{
			yield return null;
			snapPoint = getSnapPointInScrollRectSpace();
			RectTransform closestChild = null;
			float closestChildToSnapPointMag = float.MaxValue;
			Vector2 closestChildDirectionToSnapPoint = default(Vector2);
			Vector2 currentChildDirectionToSnapPoint = default(Vector2);
			for (int i = 0; i < scrollRect.content.childCount; i++)
			{
				RectTransform rectTransform = scrollRect.content.GetChild(i) as RectTransform;
				Vector3 vector = scrollRect.content.localPosition + rectTransform.localPosition;
				currentChildDirectionToSnapPoint.x = snapPoint.x - vector.x;
				currentChildDirectionToSnapPoint.y = snapPoint.y - vector.y;
				float magnitude = currentChildDirectionToSnapPoint.magnitude;
				if (closestChild == null || magnitude < closestChildToSnapPointMag)
				{
					closestChildToSnapPointMag = magnitude;
					closestChildDirectionToSnapPoint.Set(currentChildDirectionToSnapPoint.x, currentChildDirectionToSnapPoint.y);
					closestChild = rectTransform;
				}
			}
			updateTweenToContentChild(closestChild, closestChildDirectionToSnapPoint);
		}

		public void TweenToContentChildWithIndex(int index)
		{
			if (!(scrollRect.content == null) && scrollRect.content.childCount > index)
			{
				snapPoint = getSnapPointInScrollRectSpace();
				RectTransform rectTransform = scrollRect.content.GetChild(index) as RectTransform;
				Vector3 vector = scrollRect.content.localPosition + rectTransform.localPosition;
				Vector2 contentChildToSnapPoint = default(Vector2);
				contentChildToSnapPoint.x = snapPoint.x - vector.x;
				contentChildToSnapPoint.y = snapPoint.y - vector.y;
				updateTweenToContentChild(rectTransform, contentChildToSnapPoint);
			}
		}

		private void updateTweenToContentChild(RectTransform contentChild, Vector2 contentChildToSnapPoint)
		{
			float num = 0f;
			if (HorizontalSnapPos != HorizontalSnapPosition.center)
			{
				num = ((HorizontalSnapPos == HorizontalSnapPosition.left) ? (contentChild.rect.width * 0.5f) : ((0f - contentChild.rect.width) * 0.5f));
			}
			float num2 = 0f;
			if (VerticalSnapPos != VerticalSnapPosition.center)
			{
				num2 = ((VerticalSnapPos == VerticalSnapPosition.top) ? (contentChild.rect.height * 0.5f) : ((0f - contentChild.rect.height) * 0.5f));
			}
			scrollRect.StopMovement();
			tweenDirection.x = contentChildToSnapPoint.x + num;
			tweenDirection.y = contentChildToSnapPoint.y + num2;
			resetTweenData();
			isTweeningToSnapPoint = true;
			selectedChild = contentChild.gameObject;
		}

		private void resetTweenData()
		{
			tweenTimeElapsedSec = 0f;
			SnapTweenTimeSecRecip = 1f / SnapTweenTimeSec;
		}

		private void tweenToSnapPoint()
		{
			if (tweenTimeElapsedSec >= SnapTweenTimeSec)
			{
				if (this.EElementSelected != null)
				{
					this.EElementSelected(selectedChild);
				}
				isTweeningToSnapPoint = false;
			}
			else
			{
				float num = (tweenTimeElapsedSec + Time.deltaTime <= SnapTweenTimeSec) ? Time.deltaTime : (SnapTweenTimeSec - tweenTimeElapsedSec);
				tweenTimeElapsedSec += num;
				float num2 = num * SnapTweenTimeSecRecip;
				tweenPositionHelper.Set(scrollRect.content.localPosition.x + tweenDirection.x * num2, scrollRect.content.localPosition.y + tweenDirection.y * num2, scrollRect.content.localPosition.z);
				scrollRect.content.localPosition = tweenPositionHelper;
			}
		}

		private Vector2 getSnapPointInScrollRectSpace()
		{
			Vector2 result = default(Vector2);
			if (HorizontalSnapPos == HorizontalSnapPosition.center)
			{
				result.x = 0f;
			}
			else
			{
				result.x = ((HorizontalSnapPos == HorizontalSnapPosition.left) ? ((0f - scrollRect.GetComponent<RectTransform>().rect.width) * 0.5f) : (scrollRect.GetComponent<RectTransform>().rect.width * 0.5f));
			}
			if (VerticalSnapPos == VerticalSnapPosition.center)
			{
				result.y = 0f;
			}
			else
			{
				result.y = ((VerticalSnapPos == VerticalSnapPosition.top) ? ((0f - scrollRect.GetComponent<RectTransform>().rect.height) * 0.5f) : (scrollRect.GetComponent<RectTransform>().rect.height * 0.5f));
			}
			return result;
		}

		private void restrictBounds()
		{
			Vector3 localPosition = scrollRect.content.localPosition;
			float x = localPosition.x;
			float y = localPosition.y;
			bool flag = false;
			float num = scrollRect.content.rect.width * 0.5f;
			float num2 = scrollRect.content.rect.height * 0.5f;
			Vector2 snapPointInScrollRectSpace = getSnapPointInScrollRectSpace();
			if (localPosition.x > snapPointInScrollRectSpace.x + num)
			{
				x = snapPointInScrollRectSpace.x + num;
				flag = true;
			}
			else if (localPosition.x < snapPointInScrollRectSpace.x - num)
			{
				x = snapPointInScrollRectSpace.x - num;
				flag = true;
			}
			if (localPosition.y > snapPointInScrollRectSpace.y + num2)
			{
				y = snapPointInScrollRectSpace.y + num2;
				flag = true;
			}
			else if (localPosition.y < snapPointInScrollRectSpace.y - num2)
			{
				y = snapPointInScrollRectSpace.y - num2;
				flag = true;
			}
			if (flag)
			{
				scrollRect.StopMovement();
				scrollRect.content.localPosition = new Vector3(x, y, localPosition.z);
			}
		}
	}
}
