using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Scrollrect Occlusion")]
	public class UI_ScrollRectOcclusion : MonoBehaviour
	{
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

		private bool hasDisabledGridComponents = false;

		private List<RectTransform> items = new List<RectTransform>();

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
				_isHorizontal = _scrollRect.horizontal;
				_isVertical = _scrollRect.vertical;
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
			}
			else
			{
				Debug.LogError("UI_ScrollRectOcclusion => No ScrollRect component found");
			}
		}

		private void DisableGridComponents()
		{
			if (_isVertical)
			{
				float num = _scrollRect.GetComponent<RectTransform>().rect.height / 2f;
				Vector2 sizeDelta = items[0].sizeDelta;
				_disableMarginY = num + sizeDelta.y;
			}
			if (_isHorizontal)
			{
				float num2 = _scrollRect.GetComponent<RectTransform>().rect.width / 2f;
				Vector2 sizeDelta2 = items[0].sizeDelta;
				_disableMarginX = num2 + sizeDelta2.x;
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
			hasDisabledGridComponents = true;
		}

		public void OnScroll(Vector2 pos)
		{
			if (!hasDisabledGridComponents)
			{
				DisableGridComponents();
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (_isVertical && _isHorizontal)
				{
					Vector3 vector = _scrollRect.transform.InverseTransformPoint(items[i].position);
					if (!(vector.y < 0f - _disableMarginY))
					{
						Vector3 vector2 = _scrollRect.transform.InverseTransformPoint(items[i].position);
						if (!(vector2.y > _disableMarginY))
						{
							Vector3 vector3 = _scrollRect.transform.InverseTransformPoint(items[i].position);
							if (!(vector3.x < 0f - _disableMarginX))
							{
								Vector3 vector4 = _scrollRect.transform.InverseTransformPoint(items[i].position);
								if (!(vector4.x > _disableMarginX))
								{
									items[i].gameObject.SetActive(true);
									continue;
								}
							}
						}
					}
					items[i].gameObject.SetActive(false);
					continue;
				}
				if (_isVertical)
				{
					Vector3 vector5 = _scrollRect.transform.InverseTransformPoint(items[i].position);
					if (!(vector5.y < 0f - _disableMarginY))
					{
						Vector3 vector6 = _scrollRect.transform.InverseTransformPoint(items[i].position);
						if (!(vector6.y > _disableMarginY))
						{
							items[i].gameObject.SetActive(true);
							goto IL_01f1;
						}
					}
					items[i].gameObject.SetActive(false);
				}
				goto IL_01f1;
				IL_01f1:
				if (!_isHorizontal)
				{
					continue;
				}
				Vector3 vector7 = _scrollRect.transform.InverseTransformPoint(items[i].position);
				if (!(vector7.x < 0f - _disableMarginX))
				{
					Vector3 vector8 = _scrollRect.transform.InverseTransformPoint(items[i].position);
					if (!(vector8.x > _disableMarginX))
					{
						items[i].gameObject.SetActive(true);
						continue;
					}
				}
				items[i].gameObject.SetActive(false);
			}
		}
	}
}
