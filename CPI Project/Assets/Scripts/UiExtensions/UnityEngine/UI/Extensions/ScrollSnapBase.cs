using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	public class ScrollSnapBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEventSystemHandler
	{
		[Serializable]
		public class SelectionChangeStartEvent : UnityEvent
		{
		}

		[Serializable]
		public class SelectionPageChangedEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class SelectionChangeEndEvent : UnityEvent<int>
		{
		}

		internal RectTransform _screensContainer;

		internal bool _isVertical;

		internal int _screens = 1;

		internal float _scrollStartPosition;

		internal float _childSize;

		private float _childPos;

		private float _maskSize;

		internal Vector2 _childAnchorPoint;

		internal ScrollRect _scroll_rect;

		internal Vector3 _lerp_target;

		internal bool _lerp;

		internal bool _pointerDown = false;

		internal bool _settled = true;

		internal Vector3 _startPosition = default(Vector3);

		[Tooltip("The currently active page")]
		internal int _currentPage;

		internal int _previousPage;

		internal int _halfNoVisibleItems;

		private int _bottomItem;

		private int _topItem;

		[Tooltip("The screen / page to start the control on\n*Note, this is a 0 indexed array")]
		[SerializeField]
		public int StartingScreen = 0;

		[Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
		[SerializeField]
		[Range(1f, 8f)]
		public float PageStep = 1f;

		[Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
		public GameObject Pagination;

		[Tooltip("Button to go to the next page. (optional)")]
		public GameObject NextButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public GameObject PrevButton;

		[Tooltip("Transition speed between pages. (optional)")]
		public float transitionSpeed = 7.5f;

		[Tooltip("Fast Swipe makes swiping page next / previous (optional)")]
		public bool UseFastSwipe = false;

		[Tooltip("How far swipe has to travel to initiate a page change (optional)")]
		public int FastSwipeThreshold = 100;

		[Tooltip("Speed at which the ScrollRect will keep scrolling before slowing down and stopping (optional)")]
		public int SwipeVelocityThreshold = 200;

		[Tooltip("The visible bounds area, controls which items are visible/enabled. *Note Should use a RectMask. (optional)")]
		public RectTransform MaskArea;

		[Tooltip("Pixel size to buffer arround Mask Area. (optional)")]
		public float MaskBuffer = 1f;

		[Tooltip("(Experimental)\nBy default, child array objects will use the parent transform\nHowever you can disable this for some interesting effects")]
		public bool UseParentTransform = true;

		[Tooltip("Scroll Snap children. (optional)\nEither place objects in the scene as children OR\nPrefabs in this array, NOT BOTH")]
		public GameObject[] ChildObjects;

		[SerializeField]
		[Tooltip("Event fires when a user starts to change the selection")]
		private SelectionChangeStartEvent m_OnSelectionChangeStartEvent = new SelectionChangeStartEvent();

		[SerializeField]
		[Tooltip("Event fires as the page changes, while dragging or jumping")]
		private SelectionPageChangedEvent m_OnSelectionPageChangedEvent = new SelectionPageChangedEvent();

		[SerializeField]
		[Tooltip("Event fires when the page settles after a user has dragged")]
		private SelectionChangeEndEvent m_OnSelectionChangeEndEvent = new SelectionChangeEndEvent();

		public int CurrentPage
		{
			get
			{
				return _currentPage;
			}
			internal set
			{
				if ((value != _currentPage && value >= 0 && value < _screensContainer.childCount) || (value == 0 && _screensContainer.childCount == 0))
				{
					_previousPage = _currentPage;
					_currentPage = value;
					if ((bool)MaskArea)
					{
						UpdateVisible();
					}
					if (!_lerp)
					{
						ScreenChange();
					}
					ChangeBulletsInfo(_currentPage);
				}
			}
		}

		public SelectionChangeStartEvent OnSelectionChangeStartEvent
		{
			get
			{
				return m_OnSelectionChangeStartEvent;
			}
			set
			{
				m_OnSelectionChangeStartEvent = value;
			}
		}

		public SelectionPageChangedEvent OnSelectionPageChangedEvent
		{
			get
			{
				return m_OnSelectionPageChangedEvent;
			}
			set
			{
				m_OnSelectionPageChangedEvent = value;
			}
		}

		public SelectionChangeEndEvent OnSelectionChangeEndEvent
		{
			get
			{
				return m_OnSelectionChangeEndEvent;
			}
			set
			{
				m_OnSelectionChangeEndEvent = value;
			}
		}

		private void Awake()
		{
			_scroll_rect = base.gameObject.GetComponent<ScrollRect>();
			if ((bool)_scroll_rect.horizontalScrollbar || (bool)_scroll_rect.verticalScrollbar)
			{
				Debug.LogWarning("Warning, using scrollbars with the Scroll Snap controls is not advised as it causes unpredictable results");
			}
			if (StartingScreen < 0)
			{
				StartingScreen = 0;
			}
			_screensContainer = _scroll_rect.content;
			if (ChildObjects != null && ChildObjects.Length > 0)
			{
				if (_screensContainer.transform.childCount > 0)
				{
					Debug.LogError("ScrollRect Content has children, this is not supported when using managed Child Objects\n Either remove the ScrollRect Content children or clear the ChildObjects array");
					return;
				}
				InitialiseChildObjectsFromArray();
			}
			else
			{
				InitialiseChildObjectsFromScene();
			}
			if ((bool)NextButton)
			{
				NextButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					NextScreen();
				});
			}
			if ((bool)PrevButton)
			{
				PrevButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					PreviousScreen();
				});
			}
		}

		internal void InitialiseChildObjectsFromScene()
		{
			int childCount = _screensContainer.childCount;
			ChildObjects = new GameObject[childCount];
			for (int i = 0; i < childCount; i++)
			{
				ChildObjects[i] = _screensContainer.transform.GetChild(i).gameObject;
				if ((bool)MaskArea && ChildObjects[i].activeSelf)
				{
					ChildObjects[i].SetActive(false);
				}
			}
		}

		internal void InitialiseChildObjectsFromArray()
		{
			int num = ChildObjects.Length;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Object.Instantiate(ChildObjects[i]);
				if (UseParentTransform)
				{
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.rotation = _screensContainer.rotation;
					component.localScale = _screensContainer.localScale;
					component.position = _screensContainer.position;
				}
				gameObject.transform.SetParent(_screensContainer.transform);
				ChildObjects[i] = gameObject;
				if ((bool)MaskArea && ChildObjects[i].activeSelf)
				{
					ChildObjects[i].SetActive(false);
				}
			}
		}

		internal void UpdateVisible()
		{
			if (!MaskArea || ChildObjects == null || ChildObjects.Length < 1 || _screensContainer.childCount < 1)
			{
				return;
			}
			_maskSize = ((!_isVertical) ? MaskArea.rect.width : MaskArea.rect.height);
			_halfNoVisibleItems = (int)Math.Round(_maskSize / (_childSize * MaskBuffer), MidpointRounding.AwayFromZero) / 2;
			_bottomItem = (_topItem = 0);
			for (int num = _halfNoVisibleItems + 1; num > 0; num--)
			{
				_bottomItem = ((_currentPage - num >= 0) ? num : 0);
				if (_bottomItem > 0)
				{
					break;
				}
			}
			for (int num2 = _halfNoVisibleItems + 1; num2 > 0; num2--)
			{
				_topItem = ((_screensContainer.childCount - _currentPage - num2 >= 0) ? num2 : 0);
				if (_topItem > 0)
				{
					break;
				}
			}
			for (int i = CurrentPage - _bottomItem; i < CurrentPage + _topItem; i++)
			{
				try
				{
					ChildObjects[i].SetActive(true);
				}
				catch
				{
					Debug.Log("Failed to setactive child [" + i + "]");
				}
			}
			if (_currentPage > _halfNoVisibleItems)
			{
				ChildObjects[CurrentPage - _bottomItem].SetActive(false);
			}
			if (_screensContainer.childCount - _currentPage > _topItem)
			{
				ChildObjects[CurrentPage + _topItem].SetActive(false);
			}
		}

		public void NextScreen()
		{
			if (_currentPage < _screens - 1)
			{
				if (!_lerp)
				{
					StartScreenChange();
				}
				_lerp = true;
				CurrentPage = _currentPage + 1;
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		public void PreviousScreen()
		{
			if (_currentPage > 0)
			{
				if (!_lerp)
				{
					StartScreenChange();
				}
				_lerp = true;
				CurrentPage = _currentPage - 1;
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		public void GoToScreen(int screenIndex)
		{
			if (screenIndex <= _screens - 1 && screenIndex >= 0)
			{
				if (!_lerp)
				{
					StartScreenChange();
				}
				_lerp = true;
				CurrentPage = screenIndex;
				GetPositionforPage(_currentPage, ref _lerp_target);
				ScreenChange();
			}
		}

		internal int GetPageforPosition(Vector3 pos)
		{
			return (!_isVertical) ? (-(int)Math.Round((pos.x - _scrollStartPosition) / _childSize)) : (-(int)Math.Round((pos.y - _scrollStartPosition) / _childSize));
		}

		internal bool IsRectSettledOnaPage(Vector3 pos)
		{
			return (!_isVertical) ? (0f - (pos.x - _scrollStartPosition) / _childSize == (float)(-(int)Math.Round((pos.x - _scrollStartPosition) / _childSize))) : (0f - (pos.y - _scrollStartPosition) / _childSize == (float)(-(int)Math.Round((pos.y - _scrollStartPosition) / _childSize)));
		}

		internal void GetPositionforPage(int page, ref Vector3 target)
		{
			_childPos = (0f - _childSize) * (float)page;
			if (_isVertical)
			{
				target.y = _childPos + _scrollStartPosition;
			}
			else
			{
				target.x = _childPos + _scrollStartPosition;
			}
		}

		internal void ScrollToClosestElement()
		{
			_lerp = true;
			CurrentPage = GetPageforPosition(_screensContainer.localPosition);
			GetPositionforPage(_currentPage, ref _lerp_target);
			ChangeBulletsInfo(_currentPage);
		}

		internal void ChangeBulletsInfo(int targetScreen)
		{
			if ((bool)Pagination)
			{
				for (int i = 0; i < Pagination.transform.childCount; i++)
				{
					Pagination.transform.GetChild(i).GetComponent<Toggle>().isOn = ((targetScreen == i) ? true : false);
				}
			}
		}

		private void OnValidate()
		{
			int childCount = base.gameObject.GetComponent<ScrollRect>().content.childCount;
			if (childCount != 0 || ChildObjects != null)
			{
				int num = (ChildObjects != null && ChildObjects.Length != 0) ? ChildObjects.Length : childCount;
				if (StartingScreen > num - 1)
				{
					StartingScreen = num - 1;
				}
				if (StartingScreen < 0)
				{
					StartingScreen = 0;
				}
			}
			if (MaskBuffer <= 0f)
			{
				MaskBuffer = 1f;
			}
			if (PageStep < 0f)
			{
				PageStep = 0f;
			}
			if (PageStep > 8f)
			{
				PageStep = 9f;
			}
		}

		internal void StartScreenChange()
		{
			OnSelectionChangeStartEvent.Invoke();
		}

		internal void ScreenChange()
		{
			OnSelectionPageChangedEvent.Invoke(_currentPage);
		}

		internal void EndScreenChange()
		{
			OnSelectionChangeEndEvent.Invoke(_currentPage);
			_settled = true;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			_pointerDown = true;
			_settled = false;
			StartScreenChange();
			_startPosition = _screensContainer.localPosition;
		}

		public void OnDrag(PointerEventData eventData)
		{
			_lerp = false;
		}
	}
}
