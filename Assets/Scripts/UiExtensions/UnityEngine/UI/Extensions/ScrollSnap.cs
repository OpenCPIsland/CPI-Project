using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/Scroll Snap")]
	public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
	{
		public enum ScrollDirection
		{
			Horizontal,
			Vertical
		}

		public delegate void PageSnapChange(int page);

		public ScrollDirection direction = ScrollDirection.Horizontal;

		protected ScrollRect scrollRect;

		protected RectTransform scrollRectTransform;

		protected Transform listContainerTransform;

		protected RectTransform rectTransform;

		private int pages;

		protected int startingPage = 0;

		protected Vector3[] pageAnchorPositions;

		protected Vector3 lerpTarget;

		protected bool lerp;

		protected float listContainerMinPosition;

		protected float listContainerMaxPosition;

		protected float listContainerSize;

		protected RectTransform listContainerRectTransform;

		protected Vector2 listContainerCachedSize;

		protected float itemSize;

		protected int itemsCount = 0;

		[Tooltip("Button to go to the next page. (optional)")]
		public Button nextButton;

		[Tooltip("Button to go to the previous page. (optional)")]
		public Button prevButton;

		[Tooltip("Number of items visible in one page of scroll frame.")]
		[Range(1f, 100f)]
		public int itemsVisibleAtOnce = 1;

		[Tooltip("Sets minimum width of list items to 1/itemsVisibleAtOnce.")]
		public bool autoLayoutItems = true;

		[Tooltip("If you wish to update scrollbar numberOfSteps to number of active children on list.")]
		public bool linkScrolbarSteps = false;

		[Tooltip("If you wish to update scrollrect sensitivity to size of list element.")]
		public bool linkScrolrectScrollSensitivity = false;

		public bool useFastSwipe = true;

		public int fastSwipeThreshold = 100;

		protected bool startDrag = true;

		protected Vector3 positionOnDragStart = default(Vector3);

		protected int pageOnDragStart;

		protected bool fastSwipeTimer = false;

		protected int fastSwipeCounter = 0;

		protected int fastSwipeTarget = 10;

		private bool fastSwipe = false;

		public event PageSnapChange onPageChange;

		private void Awake()
		{
			lerp = false;
			scrollRect = base.gameObject.GetComponent<ScrollRect>();
			scrollRectTransform = base.gameObject.GetComponent<RectTransform>();
			listContainerTransform = scrollRect.content;
			listContainerRectTransform = listContainerTransform.GetComponent<RectTransform>();
			rectTransform = listContainerTransform.gameObject.GetComponent<RectTransform>();
			UpdateListItemsSize();
			UpdateListItemPositions();
			PageChanged(CurrentPage());
			if ((bool)nextButton)
			{
				nextButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					NextScreen();
				});
			}
			if ((bool)prevButton)
			{
				prevButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					PreviousScreen();
				});
			}
		}

		private void Start()
		{
			Awake();
		}

		public void UpdateListItemsSize()
		{
			float num = 0f;
			float num2 = 0f;
			if (direction == ScrollDirection.Horizontal)
			{
				num = scrollRectTransform.rect.width / (float)itemsVisibleAtOnce;
				num2 = listContainerRectTransform.rect.width / (float)itemsCount;
			}
			else
			{
				num = scrollRectTransform.rect.height / (float)itemsVisibleAtOnce;
				num2 = listContainerRectTransform.rect.height / (float)itemsCount;
			}
			itemSize = num;
			if (linkScrolrectScrollSensitivity)
			{
				scrollRect.scrollSensitivity = itemSize;
			}
			if (autoLayoutItems && num2 != num && itemsCount > 0)
			{
				if (direction == ScrollDirection.Horizontal)
				{
					IEnumerator enumerator = listContainerTransform.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object current = enumerator.Current;
							GameObject gameObject = ((Transform)current).gameObject;
							if (gameObject.activeInHierarchy)
							{
								LayoutElement layoutElement = gameObject.GetComponent<LayoutElement>();
								if (layoutElement == null)
								{
									layoutElement = gameObject.AddComponent<LayoutElement>();
								}
								layoutElement.minWidth = itemSize;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
				else
				{
					IEnumerator enumerator2 = listContainerTransform.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object current2 = enumerator2.Current;
							GameObject gameObject2 = ((Transform)current2).gameObject;
							if (gameObject2.activeInHierarchy)
							{
								LayoutElement layoutElement2 = gameObject2.GetComponent<LayoutElement>();
								if (layoutElement2 == null)
								{
									layoutElement2 = gameObject2.AddComponent<LayoutElement>();
								}
								layoutElement2.minHeight = itemSize;
							}
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = (enumerator2 as IDisposable)) != null)
						{
							disposable2.Dispose();
						}
					}
				}
			}
		}

		public void UpdateListItemPositions()
		{
			if (listContainerRectTransform.rect.size.Equals(listContainerCachedSize))
			{
				return;
			}
			int num = 0;
			IEnumerator enumerator = listContainerTransform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (((Transform)current).gameObject.activeInHierarchy)
					{
						num++;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			itemsCount = 0;
			Array.Resize(ref pageAnchorPositions, num);
			if (num > 0)
			{
				pages = Mathf.Max(num - itemsVisibleAtOnce + 1, 1);
				if (direction == ScrollDirection.Horizontal)
				{
					scrollRect.horizontalNormalizedPosition = 0f;
					Vector3 localPosition = listContainerTransform.localPosition;
					listContainerMaxPosition = localPosition.x;
					scrollRect.horizontalNormalizedPosition = 1f;
					Vector3 localPosition2 = listContainerTransform.localPosition;
					listContainerMinPosition = localPosition2.x;
					listContainerSize = listContainerMaxPosition - listContainerMinPosition;
					for (int i = 0; i < pages; i++)
					{
						 Vector3 reference =  pageAnchorPositions[i];
						float x = listContainerMaxPosition - itemSize * (float)i;
						Vector3 localPosition3 = listContainerTransform.localPosition;
						float y = localPosition3.y;
						Vector3 localPosition4 = listContainerTransform.localPosition;
						reference = new Vector3(x, y, localPosition4.z);
					}
				}
				else
				{
					scrollRect.verticalNormalizedPosition = 1f;
					Vector3 localPosition5 = listContainerTransform.localPosition;
					listContainerMinPosition = localPosition5.y;
					scrollRect.verticalNormalizedPosition = 0f;
					Vector3 localPosition6 = listContainerTransform.localPosition;
					listContainerMaxPosition = localPosition6.y;
					listContainerSize = listContainerMaxPosition - listContainerMinPosition;
					for (int j = 0; j < pages; j++)
					{
						 Vector3 reference2 =  pageAnchorPositions[j];
						Vector3 localPosition7 = listContainerTransform.localPosition;
						float x2 = localPosition7.x;
						float y2 = listContainerMinPosition + itemSize * (float)j;
						Vector3 localPosition8 = listContainerTransform.localPosition;
						reference2 = new Vector3(x2, y2, localPosition8.z);
					}
				}
				UpdateScrollbar(linkScrolbarSteps);
				startingPage = Mathf.Min(startingPage, pages);
				ResetPage();
			}
			if (itemsCount != num)
			{
				PageChanged(CurrentPage());
			}
			itemsCount = num;
			 Vector2 reference3 =  listContainerCachedSize;
			Vector2 size = listContainerRectTransform.rect.size;
			float x3 = size.x;
			Vector2 size2 = listContainerRectTransform.rect.size;
			reference3.Set(x3, size2.y);
		}

		public void ResetPage()
		{
			if (direction == ScrollDirection.Horizontal)
			{
				scrollRect.horizontalNormalizedPosition = ((pages <= 1) ? 0f : ((float)startingPage / (float)(pages - 1)));
			}
			else
			{
				scrollRect.verticalNormalizedPosition = ((pages <= 1) ? 0f : ((float)(pages - startingPage - 1) / (float)(pages - 1)));
			}
		}

		protected void UpdateScrollbar(bool linkSteps)
		{
			if (linkSteps)
			{
				if (direction == ScrollDirection.Horizontal)
				{
					if (scrollRect.horizontalScrollbar != null)
					{
						scrollRect.horizontalScrollbar.numberOfSteps = pages;
					}
				}
				else if (scrollRect.verticalScrollbar != null)
				{
					scrollRect.verticalScrollbar.numberOfSteps = pages;
				}
			}
			else if (direction == ScrollDirection.Horizontal)
			{
				if (scrollRect.horizontalScrollbar != null)
				{
					scrollRect.horizontalScrollbar.numberOfSteps = 0;
				}
			}
			else if (scrollRect.verticalScrollbar != null)
			{
				scrollRect.verticalScrollbar.numberOfSteps = 0;
			}
		}

		private void LateUpdate()
		{
			UpdateListItemsSize();
			UpdateListItemPositions();
			if (lerp)
			{
				UpdateScrollbar(false);
				listContainerTransform.localPosition = Vector3.Lerp(listContainerTransform.localPosition, lerpTarget, 7.5f * Time.deltaTime);
				if (Vector3.Distance(listContainerTransform.localPosition, lerpTarget) < 0.001f)
				{
					listContainerTransform.localPosition = lerpTarget;
					lerp = false;
					UpdateScrollbar(linkScrolbarSteps);
				}
				if (Vector3.Distance(listContainerTransform.localPosition, lerpTarget) < 10f)
				{
					PageChanged(CurrentPage());
				}
			}
			if (fastSwipeTimer)
			{
				fastSwipeCounter++;
			}
		}

		public void NextScreen()
		{
			UpdateListItemPositions();
			if (CurrentPage() < pages - 1)
			{
				lerp = true;
				lerpTarget = pageAnchorPositions[CurrentPage() + 1];
				PageChanged(CurrentPage() + 1);
			}
		}

		public void PreviousScreen()
		{
			UpdateListItemPositions();
			if (CurrentPage() > 0)
			{
				lerp = true;
				lerpTarget = pageAnchorPositions[CurrentPage() - 1];
				PageChanged(CurrentPage() - 1);
			}
		}

		private void NextScreenCommand()
		{
			if (pageOnDragStart < pages - 1)
			{
				int num = Mathf.Min(pages - 1, pageOnDragStart + itemsVisibleAtOnce);
				lerp = true;
				lerpTarget = pageAnchorPositions[num];
				PageChanged(num);
			}
		}

		private void PrevScreenCommand()
		{
			if (pageOnDragStart > 0)
			{
				int num = Mathf.Max(0, pageOnDragStart - itemsVisibleAtOnce);
				lerp = true;
				lerpTarget = pageAnchorPositions[num];
				PageChanged(num);
			}
		}

		public int CurrentPage()
		{
			float value;
			if (direction == ScrollDirection.Horizontal)
			{
				float num = listContainerMaxPosition;
				Vector3 localPosition = listContainerTransform.localPosition;
				value = num - localPosition.x;
				value = Mathf.Clamp(value, 0f, listContainerSize);
			}
			else
			{
				Vector3 localPosition2 = listContainerTransform.localPosition;
				value = localPosition2.y - listContainerMinPosition;
				value = Mathf.Clamp(value, 0f, listContainerSize);
			}
			float f = value / itemSize;
			return Mathf.Clamp(Mathf.RoundToInt(f), 0, pages);
		}

		public void ChangePage(int page)
		{
			if (0 <= page && page < pages)
			{
				lerp = true;
				lerpTarget = pageAnchorPositions[page];
				PageChanged(page);
			}
		}

		private void PageChanged(int currentPage)
		{
			startingPage = currentPage;
			if ((bool)nextButton)
			{
				nextButton.interactable = (currentPage < pages - 1);
			}
			if ((bool)prevButton)
			{
				prevButton.interactable = (currentPage > 0);
			}
			if (this.onPageChange != null)
			{
				this.onPageChange(currentPage);
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			UpdateScrollbar(false);
			fastSwipeCounter = 0;
			fastSwipeTimer = true;
			positionOnDragStart = eventData.position;
			pageOnDragStart = CurrentPage();
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			startDrag = true;
			float num = 0f;
			if (direction == ScrollDirection.Horizontal)
			{
				float x = positionOnDragStart.x;
				Vector2 position = eventData.position;
				num = x - position.x;
			}
			else
			{
				float num2 = 0f - positionOnDragStart.y;
				Vector2 position2 = eventData.position;
				num = num2 + position2.y;
			}
			if (useFastSwipe)
			{
				fastSwipe = false;
				fastSwipeTimer = false;
				if (fastSwipeCounter <= fastSwipeTarget && Math.Abs(num) > (float)fastSwipeThreshold)
				{
					fastSwipe = true;
				}
				if (fastSwipe)
				{
					if (num > 0f)
					{
						NextScreenCommand();
					}
					else
					{
						PrevScreenCommand();
					}
				}
				else
				{
					lerp = true;
					lerpTarget = pageAnchorPositions[CurrentPage()];
				}
			}
			else
			{
				lerp = true;
				lerpTarget = pageAnchorPositions[CurrentPage()];
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			lerp = false;
			if (startDrag)
			{
				OnBeginDrag(eventData);
				startDrag = false;
			}
		}
	}
}
