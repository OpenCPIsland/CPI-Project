using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Infinite Scroll")]
	public class UI_InfiniteScroll : MonoBehaviour
	{
		[Tooltip("If false, will Init automatically, otherwise you need to call Init() method")]
		public bool InitByUser = false;

		private ScrollRect _scrollRect;

		private ContentSizeFitter _contentSizeFitter;

		private VerticalLayoutGroup _verticalLayoutGroup;

		private HorizontalLayoutGroup _horizontalLayoutGroup;

		private GridLayoutGroup _gridLayoutGroup;

		private bool _isVertical = false;

		private bool _isHorizontal = false;

		private float _disableMarginX = 0f;

		private float _disableMarginY = 0f;

		private bool _hasDisabledGridComponents = false;

		private List<RectTransform> items = new List<RectTransform>();

		private Vector2 _newAnchoredPosition = Vector2.zero;

		private float _treshold = 100f;

		private int _itemCount = 0;

		private float _recordOffsetX = 0f;

		private float _recordOffsetY = 0f;

		private void Awake()
		{
			if (!InitByUser)
			{
				Init();
			}
		}

		public void Init()
		{
			if (GetComponent<ScrollRect>() != null)
			{
				_scrollRect = GetComponent<ScrollRect>();
				_scrollRect.onValueChanged.AddListener(OnScroll);
				_scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
				for (int i = 0; i < _scrollRect.content.childCount; i++)
				{
					items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
				}
				if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
				{
					_verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
				{
					_horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
				{
					_gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
				}
				if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
				{
					_contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
				}
				_isHorizontal = _scrollRect.horizontal;
				_isVertical = _scrollRect.vertical;
				if (_isHorizontal && _isVertical)
				{
					Debug.LogError("UI_InfiniteScroll doesn't support scrolling in both directions, plase choose one direction (horizontal or vertical)");
				}
				_itemCount = _scrollRect.content.childCount;
			}
			else
			{
				Debug.LogError("UI_InfiniteScroll => No ScrollRect component found");
			}
		}

		private void DisableGridComponents()
		{
			if (_isVertical)
			{
				Vector2 anchoredPosition = items[0].GetComponent<RectTransform>().anchoredPosition;
				float y = anchoredPosition.y;
				Vector2 anchoredPosition2 = items[1].GetComponent<RectTransform>().anchoredPosition;
				_recordOffsetY = y - anchoredPosition2.y;
				_disableMarginY = _recordOffsetY * (float)_itemCount / 2f;
			}
			if (_isHorizontal)
			{
				Vector2 anchoredPosition3 = items[1].GetComponent<RectTransform>().anchoredPosition;
				float x = anchoredPosition3.x;
				Vector2 anchoredPosition4 = items[0].GetComponent<RectTransform>().anchoredPosition;
				_recordOffsetX = x - anchoredPosition4.x;
				_disableMarginX = _recordOffsetX * (float)_itemCount / 2f;
			}
			if ((bool)_verticalLayoutGroup)
			{
				_verticalLayoutGroup.enabled = false;
			}
			if ((bool)_horizontalLayoutGroup)
			{
				_horizontalLayoutGroup.enabled = false;
			}
			if ((bool)_contentSizeFitter)
			{
				_contentSizeFitter.enabled = false;
			}
			if ((bool)_gridLayoutGroup)
			{
				_gridLayoutGroup.enabled = false;
			}
			_hasDisabledGridComponents = true;
		}

		public void OnScroll(Vector2 pos)
		{
			if (!_hasDisabledGridComponents)
			{
				DisableGridComponents();
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (_isHorizontal)
				{
					Vector3 vector = _scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position);
					if (vector.x > _disableMarginX + _treshold)
					{
						_newAnchoredPosition = items[i].anchoredPosition;
						_newAnchoredPosition.x -= (float)_itemCount * _recordOffsetX;
						items[i].anchoredPosition = _newAnchoredPosition;
						_scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
					}
					else
					{
						Vector3 vector2 = _scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position);
						if (vector2.x < 0f - _disableMarginX)
						{
							_newAnchoredPosition = items[i].anchoredPosition;
							_newAnchoredPosition.x += (float)_itemCount * _recordOffsetX;
							items[i].anchoredPosition = _newAnchoredPosition;
							_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
						}
					}
				}
				if (!_isVertical)
				{
					continue;
				}
				Vector3 vector3 = _scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position);
				if (vector3.y > _disableMarginY + _treshold)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.y -= (float)_itemCount * _recordOffsetY;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(_itemCount - 1).transform.SetAsFirstSibling();
					continue;
				}
				Vector3 vector4 = _scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position);
				if (vector4.y < 0f - _disableMarginY)
				{
					_newAnchoredPosition = items[i].anchoredPosition;
					_newAnchoredPosition.y += (float)_itemCount * _recordOffsetY;
					items[i].anchoredPosition = _newAnchoredPosition;
					_scrollRect.content.GetChild(0).transform.SetAsLastSibling();
				}
			}
		}
	}
}
