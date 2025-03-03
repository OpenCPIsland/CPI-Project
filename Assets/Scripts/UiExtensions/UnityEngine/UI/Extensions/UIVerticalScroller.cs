using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Layout/Extensions/Vertical Scroller")]
	public class UIVerticalScroller : MonoBehaviour
	{
		[Tooltip("Scrollable area (content of desired ScrollRect)")]
		public RectTransform _scrollingPanel;

		[Tooltip("Elements to populate inside the scroller")]
		public GameObject[] _arrayOfElements;

		[Tooltip("Center display area (position of zoomed content)")]
		public RectTransform _center;

		[Tooltip("Select the item to be in center on start. (optional)")]
		public int StartingIndex = -1;

		[Tooltip("Button to go to the next page. (optional)")]
		public GameObject ScrollUpButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public GameObject ScrollDownButton;

		[Tooltip("Event fired when a specific item is clicked, exposes index number of item. (optional)")]
		public UnityEvent<int> ButtonClicked;

		private float[] distReposition;

		private float[] distance;

		private int minElementsNum;

		private int elementLength;

		private float deltaY;

		private string result;

		public UIVerticalScroller()
		{
		}

		public UIVerticalScroller(RectTransform scrollingPanel, GameObject[] arrayOfElements, RectTransform center)
		{
			_scrollingPanel = scrollingPanel;
			_arrayOfElements = arrayOfElements;
			_center = center;
		}

		public void Awake()
		{
			ScrollRect component = GetComponent<ScrollRect>();
			if (!_scrollingPanel)
			{
				_scrollingPanel = component.content;
			}
			if (!_center)
			{
				Debug.LogError("Please define the RectTransform for the Center viewport of the scrollable area");
			}
			if (_arrayOfElements != null && _arrayOfElements.Length != 0)
			{
				return;
			}
			int childCount = component.content.childCount;
			if (childCount > 0)
			{
				_arrayOfElements = new GameObject[childCount];
				for (int i = 0; i < childCount; i++)
				{
					_arrayOfElements[i] = component.content.GetChild(i).gameObject;
				}
			}
		}

		public void Start()
		{
			if (_arrayOfElements.Length < 1)
			{
				Debug.Log("No child content found, exiting..");
				return;
			}
			elementLength = _arrayOfElements.Length;
			distance = new float[elementLength];
			distReposition = new float[elementLength];
			deltaY = _arrayOfElements[0].GetComponent<RectTransform>().rect.height * (float)elementLength / 3f * 2f;
			Vector2 anchoredPosition = _scrollingPanel.anchoredPosition;
			Vector2 anchoredPosition2 = new Vector2(anchoredPosition.x, 0f - deltaY);
			_scrollingPanel.anchoredPosition = anchoredPosition2;
			for (int i = 0; i < _arrayOfElements.Length; i++)
			{
				AddListener(_arrayOfElements[i], i);
			}
			if ((bool)ScrollUpButton)
			{
				ScrollUpButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					ScrollUp();
				});
			}
			if ((bool)ScrollDownButton)
			{
				ScrollDownButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					ScrollDown();
				});
			}
			if (StartingIndex > -1)
			{
				StartingIndex = ((StartingIndex <= _arrayOfElements.Length) ? StartingIndex : (_arrayOfElements.Length - 1));
				SnapToElement(StartingIndex);
			}
		}

		private void AddListener(GameObject button, int index)
		{
			button.GetComponent<Button>().onClick.AddListener(delegate
			{
				DoSomething(index);
			});
		}

		private void DoSomething(int index)
		{
			if (ButtonClicked != null)
			{
				ButtonClicked.Invoke(index);
			}
		}

		public void Update()
		{
			if (_arrayOfElements.Length < 1)
			{
				return;
			}
			for (int i = 0; i < elementLength; i++)
			{
				float[] array = distReposition;
				int num = i;
				Vector3 position = _center.GetComponent<RectTransform>().position;
				float y = position.y;
				Vector3 position2 = _arrayOfElements[i].GetComponent<RectTransform>().position;
				array[num] = y - position2.y;
				distance[i] = Mathf.Abs(distReposition[i]);
				float num2 = Mathf.Max(0.7f, 1f / (1f + distance[i] / 200f));
				_arrayOfElements[i].GetComponent<RectTransform>().transform.localScale = new Vector3(num2, num2, 1f);
			}
			float num3 = Mathf.Min(distance);
			for (int j = 0; j < elementLength; j++)
			{
				_arrayOfElements[j].GetComponent<CanvasGroup>().interactable = false;
				if (num3 == distance[j])
				{
					minElementsNum = j;
					_arrayOfElements[j].GetComponent<CanvasGroup>().interactable = true;
					result = _arrayOfElements[j].GetComponentInChildren<Text>().text;
				}
			}
			Vector2 anchoredPosition = _arrayOfElements[minElementsNum].GetComponent<RectTransform>().anchoredPosition;
			ScrollingElements(0f - anchoredPosition.y);
		}

		private void ScrollingElements(float position)
		{
			Vector2 anchoredPosition = _scrollingPanel.anchoredPosition;
			float y = Mathf.Lerp(anchoredPosition.y, position, Time.deltaTime * 1f);
			Vector2 anchoredPosition2 = _scrollingPanel.anchoredPosition;
			Vector2 anchoredPosition3 = new Vector2(anchoredPosition2.x, y);
			_scrollingPanel.anchoredPosition = anchoredPosition3;
		}

		public string GetResults()
		{
			return result;
		}

		public void SnapToElement(int element)
		{
			float num = _arrayOfElements[0].GetComponent<RectTransform>().rect.height * (float)element;
			Vector2 anchoredPosition = _scrollingPanel.anchoredPosition;
			Vector2 anchoredPosition2 = new Vector2(anchoredPosition.x, 0f - num);
			_scrollingPanel.anchoredPosition = anchoredPosition2;
		}

		public void ScrollUp()
		{
			float num = _arrayOfElements[0].GetComponent<RectTransform>().rect.height / 1.2f;
			Vector2 anchoredPosition = _scrollingPanel.anchoredPosition;
			float x = anchoredPosition.x;
			Vector2 anchoredPosition2 = _scrollingPanel.anchoredPosition;
			Vector2 b = new Vector2(x, anchoredPosition2.y - num);
			_scrollingPanel.anchoredPosition = Vector2.Lerp(_scrollingPanel.anchoredPosition, b, 1f);
		}

		public void ScrollDown()
		{
			float num = _arrayOfElements[0].GetComponent<RectTransform>().rect.height / 1.2f;
			Vector2 anchoredPosition = _scrollingPanel.anchoredPosition;
			float x = anchoredPosition.x;
			Vector2 anchoredPosition2 = _scrollingPanel.anchoredPosition;
			Vector2 anchoredPosition3 = new Vector2(x, anchoredPosition2.y + num);
			_scrollingPanel.anchoredPosition = anchoredPosition3;
		}
	}
}
