using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/ComboBox")]
	public class ComboBox : MonoBehaviour
	{
		public Color disabledTextColor;

		public List<string> AvailableOptions;

		public Action<int> OnSelectionChanged;

		private bool _isPanelActive = false;

		private bool _hasDrawnOnce = false;

		private InputField _mainInput;

		private RectTransform _inputRT;

		private RectTransform _rectTransform;

		private RectTransform _overlayRT;

		private RectTransform _scrollPanelRT;

		private RectTransform _scrollBarRT;

		private RectTransform _slidingAreaRT;

		private RectTransform _itemsPanelRT;

		private Canvas _canvas;

		private RectTransform _canvasRT;

		private ScrollRect _scrollRect;

		private List<string> _panelItems;

		private Dictionary<string, GameObject> panelObjects;

		private GameObject itemTemplate;

		[SerializeField]
		private float _scrollBarWidth = 20f;

		[SerializeField]
		private int _itemsToDisplay;

		public DropDownListItem SelectedItem
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public float ScrollBarWidth
		{
			get
			{
				return _scrollBarWidth;
			}
			set
			{
				_scrollBarWidth = value;
				RedrawPanel();
			}
		}

		public int ItemsToDisplay
		{
			get
			{
				return _itemsToDisplay;
			}
			set
			{
				_itemsToDisplay = value;
				RedrawPanel();
			}
		}

		public void Awake()
		{
			Initialize();
		}

		private bool Initialize()
		{
			bool result = true;
			try
			{
				_rectTransform = GetComponent<RectTransform>();
				_inputRT = _rectTransform.Find("InputField").GetComponent<RectTransform>();
				_mainInput = _inputRT.GetComponent<InputField>();
				_overlayRT = _rectTransform.Find("Overlay").GetComponent<RectTransform>();
				_overlayRT.gameObject.SetActive(false);
				_scrollPanelRT = _overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
				_scrollBarRT = _scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
				_slidingAreaRT = _scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
				_itemsPanelRT = _scrollPanelRT.Find("Items").GetComponent<RectTransform>();
				_canvas = GetComponentInParent<Canvas>();
				_canvasRT = _canvas.GetComponent<RectTransform>();
				_scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
				ScrollRect scrollRect = _scrollRect;
				Vector2 sizeDelta = _rectTransform.sizeDelta;
				scrollRect.scrollSensitivity = sizeDelta.y / 2f;
				_scrollRect.movementType = ScrollRect.MovementType.Clamped;
				_scrollRect.content = _itemsPanelRT;
				itemTemplate = _rectTransform.Find("ItemTemplate").gameObject;
				itemTemplate.SetActive(false);
			}
			catch (NullReferenceException exception)
			{
				Debug.LogException(exception);
				Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Refernece Exception");
				result = false;
			}
			panelObjects = new Dictionary<string, GameObject>();
			_panelItems = AvailableOptions.ToList();
			RebuildPanel();
			return result;
		}

		private void RebuildPanel()
		{
			_panelItems.Clear();
			foreach (string availableOption in AvailableOptions)
			{
				_panelItems.Add(availableOption.ToLower());
			}
			_panelItems.Sort();
			List<GameObject> list = new List<GameObject>(panelObjects.Values);
			panelObjects.Clear();
			int num = 0;
			while (list.Count < AvailableOptions.Count)
			{
				GameObject gameObject = Object.Instantiate(itemTemplate);
				gameObject.name = "Item " + num;
				gameObject.transform.SetParent(_itemsPanelRT, false);
				list.Add(gameObject);
				num++;
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].SetActive(i <= AvailableOptions.Count);
				if (i < AvailableOptions.Count)
				{
					list[i].name = "Item " + i + " " + _panelItems[i];
					list[i].transform.Find("Text").GetComponent<Text>().text = _panelItems[i];
					Button component = list[i].GetComponent<Button>();
					component.onClick.RemoveAllListeners();
					string textOfItem = _panelItems[i];
					component.onClick.AddListener(delegate
					{
						OnItemClicked(textOfItem);
					});
					panelObjects[_panelItems[i]] = list[i];
				}
			}
		}

		private void OnItemClicked(string item)
		{
			Text = item;
			_mainInput.text = Text;
			ToggleDropdownPanel(true);
		}

		private void RedrawPanel()
		{
			float num = (_panelItems.Count <= ItemsToDisplay) ? 0f : _scrollBarWidth;
			_scrollBarRT.gameObject.SetActive(_panelItems.Count > ItemsToDisplay);
			if (!_hasDrawnOnce || _rectTransform.sizeDelta != _inputRT.sizeDelta)
			{
				_hasDrawnOnce = true;
				RectTransform inputRT = _inputRT;
				Vector2 sizeDelta = _rectTransform.sizeDelta;
				inputRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta.x);
				RectTransform inputRT2 = _inputRT;
				Vector2 sizeDelta2 = _rectTransform.sizeDelta;
				inputRT2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta2.y);
				_scrollPanelRT.SetParent(base.transform, true);
				RectTransform scrollPanelRT = _scrollPanelRT;
				Vector2 sizeDelta3 = _rectTransform.sizeDelta;
				scrollPanelRT.anchoredPosition = new Vector2(0f, 0f - sizeDelta3.y);
				_overlayRT.SetParent(_canvas.transform, false);
				RectTransform overlayRT = _overlayRT;
				Vector2 sizeDelta4 = _canvasRT.sizeDelta;
				overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta4.x);
				RectTransform overlayRT2 = _overlayRT;
				Vector2 sizeDelta5 = _canvasRT.sizeDelta;
				overlayRT2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDelta5.y);
				_overlayRT.SetParent(base.transform, true);
				_scrollPanelRT.SetParent(_overlayRT, true);
			}
			if (_panelItems.Count >= 1)
			{
				Vector2 sizeDelta6 = _rectTransform.sizeDelta;
				float num2 = sizeDelta6.y * (float)Mathf.Min(_itemsToDisplay, _panelItems.Count);
				_scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
				RectTransform scrollPanelRT2 = _scrollPanelRT;
				Vector2 sizeDelta7 = _rectTransform.sizeDelta;
				scrollPanelRT2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta7.x);
				RectTransform itemsPanelRT = _itemsPanelRT;
				Vector2 sizeDelta8 = _scrollPanelRT.sizeDelta;
				itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeDelta8.x - num - 5f);
				_itemsPanelRT.anchoredPosition = new Vector2(5f, 0f);
				_scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
				_scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
				_slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
				RectTransform slidingAreaRT = _slidingAreaRT;
				Vector2 sizeDelta9 = _scrollBarRT.sizeDelta;
				slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2 - sizeDelta9.x);
			}
		}

		public void OnValueChanged(string currText)
		{
			Text = currText;
			RedrawPanel();
			if (_panelItems.Count == 0)
			{
				_isPanelActive = true;
				ToggleDropdownPanel(false);
			}
			else if (!_isPanelActive)
			{
				ToggleDropdownPanel(false);
			}
		}

		public void ToggleDropdownPanel(bool directClick)
		{
			_isPanelActive = !_isPanelActive;
			_overlayRT.gameObject.SetActive(_isPanelActive);
			if (_isPanelActive)
			{
				base.transform.SetAsLastSibling();
			}
			else if (!directClick)
			{
			}
		}
	}
}
