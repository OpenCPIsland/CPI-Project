using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Layout/Extensions/Vertical Scroll Snap")]
	public class VerticalScrollSnap : ScrollSnapBase, IEndDragHandler, IEventSystemHandler
	{
		private void Start()
		{
			_isVertical = true;
			_childAnchorPoint = new Vector2(0.5f, 0f);
			_currentPage = StartingScreen;
			UpdateLayout();
		}

		private void Update()
		{
			if (!_lerp && _scroll_rect.velocity == Vector2.zero)
			{
				if (!_settled && !_pointerDown && !IsRectSettledOnaPage(_screensContainer.localPosition))
				{
					ScrollToClosestElement();
				}
				return;
			}
			if (_lerp)
			{
				_screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerp_target, transitionSpeed * Time.deltaTime);
				if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 0.1f)
				{
					_lerp = false;
					EndScreenChange();
				}
			}
			base.CurrentPage = GetPageforPosition(_screensContainer.localPosition);
			if (_pointerDown)
			{
				return;
			}
			Vector2 velocity = _scroll_rect.velocity;
			if (!((double)velocity.y > 0.01))
			{
				Vector2 velocity2 = _scroll_rect.velocity;
				if (!((double)velocity2.y < -0.01))
				{
					return;
				}
			}
			if (IsRectMovingSlowerThanThreshold(0f))
			{
				ScrollToClosestElement();
			}
		}

		private bool IsRectMovingSlowerThanThreshold(float startingSpeed)
		{
			Vector2 velocity = _scroll_rect.velocity;
			int result;
			if (velocity.y > startingSpeed)
			{
				Vector2 velocity2 = _scroll_rect.velocity;
				if (velocity2.y < (float)SwipeVelocityThreshold)
				{
					result = 1;
					goto IL_0075;
				}
			}
			Vector2 velocity3 = _scroll_rect.velocity;
			if (velocity3.y < startingSpeed)
			{
				Vector2 velocity4 = _scroll_rect.velocity;
				result = ((velocity4.y > (float)(-SwipeVelocityThreshold)) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			goto IL_0075;
			IL_0075:
			return (byte)result != 0;
		}

		public void DistributePages()
		{
			_screens = _screensContainer.childCount;
			_scroll_rect.verticalNormalizedPosition = 0f;
			float num = 0f;
			float num2 = 0f;
			Rect rect = base.gameObject.GetComponent<RectTransform>().rect;
			float num3 = 0f;
			float num4 = _childSize = (float)(int)rect.height * ((PageStep != 0f) ? PageStep : 3f);
			for (int i = 0; i < _screensContainer.transform.childCount; i++)
			{
				RectTransform component = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
				num3 = num + (float)i * num4;
				component.sizeDelta = new Vector2(rect.width, rect.height);
				component.anchoredPosition = new Vector2(0f, num3);
				Vector2 vector = component.pivot = _childAnchorPoint;
				vector = (component.anchorMin = (component.anchorMax = vector));
			}
			num2 = num3 + num * -1f;
			_screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(0f, num2);
		}

		public void AddChild(GameObject GO)
		{
			AddChild(GO, false);
		}

		public void AddChild(GameObject GO, bool WorldPositionStays)
		{
			_scroll_rect.verticalNormalizedPosition = 0f;
			GO.transform.SetParent(_screensContainer, WorldPositionStays);
			InitialiseChildObjectsFromScene();
			DistributePages();
			if ((bool)MaskArea)
			{
				UpdateVisible();
			}
			SetScrollContainerPosition();
		}

		public void RemoveChild(int index, out GameObject ChildRemoved)
		{
			ChildRemoved = null;
			if (index >= 0 && index <= _screensContainer.childCount)
			{
				_scroll_rect.verticalNormalizedPosition = 0f;
				Transform child = _screensContainer.transform.GetChild(index);
				child.SetParent(null);
				ChildRemoved = child.gameObject;
				InitialiseChildObjectsFromScene();
				DistributePages();
				if ((bool)MaskArea)
				{
					UpdateVisible();
				}
				if (_currentPage > _screens - 1)
				{
					base.CurrentPage = _screens - 1;
				}
				SetScrollContainerPosition();
			}
		}

		public void RemoveAllChildren(out GameObject[] ChildrenRemoved)
		{
			int childCount = _screensContainer.childCount;
			ChildrenRemoved = new GameObject[childCount];
			for (int num = childCount - 1; num >= 0; num--)
			{
				ChildrenRemoved[num] = _screensContainer.GetChild(num).gameObject;
				ChildrenRemoved[num].transform.SetParent(null);
			}
			_scroll_rect.verticalNormalizedPosition = 0f;
			base.CurrentPage = 0;
			InitialiseChildObjectsFromScene();
			DistributePages();
			if ((bool)MaskArea)
			{
				UpdateVisible();
			}
		}

		private void SetScrollContainerPosition()
		{
			Vector3 localPosition = _screensContainer.localPosition;
			_scrollStartPosition = localPosition.y;
			_scroll_rect.verticalNormalizedPosition = (float)_currentPage / (float)(_screens - 1);
		}

		public void UpdateLayout()
		{
			_lerp = false;
			DistributePages();
			if ((bool)MaskArea)
			{
				UpdateVisible();
			}
			SetScrollContainerPosition();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (_childAnchorPoint != Vector2.zero)
			{
				UpdateLayout();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_pointerDown = false;
			if (!_scroll_rect.vertical || !UseFastSwipe)
			{
				return;
			}
			Vector2 velocity = _scroll_rect.velocity;
			if (velocity.y > 0f)
			{
				Vector2 velocity2 = _scroll_rect.velocity;
				if (velocity2.y > (float)FastSwipeThreshold)
				{
					goto IL_009e;
				}
			}
			Vector2 velocity3 = _scroll_rect.velocity;
			if (velocity3.y < 0f)
			{
				Vector2 velocity4 = _scroll_rect.velocity;
				if (velocity4.y < (float)(-FastSwipeThreshold))
				{
					goto IL_009e;
				}
			}
			ScrollToClosestElement();
			return;
			IL_009e:
			_scroll_rect.velocity = Vector3.zero;
			float y = _startPosition.y;
			Vector3 localPosition = _screensContainer.localPosition;
			if (y - localPosition.y > 0f)
			{
				NextScreen();
			}
			else
			{
				PreviousScreen();
			}
		}
	}
}
