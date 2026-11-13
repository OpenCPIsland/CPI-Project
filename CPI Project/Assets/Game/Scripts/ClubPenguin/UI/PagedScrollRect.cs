using Disney.Kelowna.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.UI
{
    public class PagedScrollRect : MonoBehaviour
    {
        public enum ScrollDirection
        {
            Left,
            Right
        }

        private const float MIN_SWIPE_DIST = 0.09f;
        private const float MAX_SWIPE_TIME = 0.4f;
        private const int PAGE_ICON_ON_INDEX = 0;
        private const int PAGE_ICON_OFF_INDEX = 1;

        public RectTransform ContentPanel;
        public RectTransform PageIconContainer;
        public float ItemSpacingPadding = 20f;
        public float ScrollTime = 0.3f;
        public float AutoScrollWaitTime = 4f;
        public bool LoadItemsDynamically = false;
        public PrefabContentKey ScrollPageIconKey;

        protected bool contentLoaded = false;
        protected LinkedList<GameObject> contentItems;
        protected List<TintSelector> pageIcons;
        protected int totalItems;
        protected int currentItemIndex;
        protected int currentPageIcon;
        private float ItemSpacing;
        private bool isScrolling = false;
        private ScrollDirection currentScrollDirection;
        private bool isTouching = false;
        private Vector2 startingTouchPositionInPixels;
        private float touchDist = 0f;
        private float touchTime = 0f;
        private float autoScrollTimer;
        private float prevPointerPos = 0f;

        private void Start()
        {
            pageIcons = new List<TintSelector>();
            contentItems = new LinkedList<GameObject>();
            start();
            if (ContentPanel.childCount > 0)
            {
                loadExistingContent();
                contentLoaded = true;
            }
            else
            {
                loadContent();
            }
        }

        private void OnDestroy()
        {
            onDestroy();
        }

        protected virtual void start()
        {
        }

        protected virtual void onDestroy()
        {
        }

        private void loadExistingContent()
        {
            Transform[] array = new Transform[ContentPanel.childCount];
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < ContentPanel.childCount; i++)
            {
                array[i] = ContentPanel.GetChild(i);
            }
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i].gameObject;
                gameObject.transform.SetParent(null);
                list.Add(gameObject);
            }
            for (int i = 0; i < list.Count - 1; i++)
            {
                addItem(list[i], ScrollDirection.Right, i);
            }
            addItem(list[list.Count - 1], ScrollDirection.Left, 1);
            totalItems = contentItems.Count;
            Content.LoadAsync(onPageIconLoaded, ScrollPageIconKey);
        }

        protected virtual void loadContent()
        {
            contentLoaded = true;
        }

        protected void reloadList()
        {
            if (pageIcons != null)
            {
                for (int i = 0; i < pageIcons.Count; i++)
                {
                    Object.Destroy(pageIcons[i].gameObject);
                }
                pageIcons.Clear();
            }
            clearContentItems();
            pageIcons = new List<TintSelector>();
            contentItems = new LinkedList<GameObject>();
            initList();
        }

        protected virtual void initList()
        {
        }

        protected void onPageIconLoaded(string path, GameObject pageIconPrefab)
        {
            for (int i = 0; i < totalItems; i++)
            {
                pageIcons.Add(Object.Instantiate(pageIconPrefab, PageIconContainer, false).GetComponent<TintSelector>());
            }
            updatePageIcons();
        }

        protected void addItem(GameObject item, ScrollDirection addDirection, int aheadCount)
        {
            ItemSpacing = item.GetComponent<RectTransform>().rect.width + ItemSpacingPadding;
            item.transform.SetParent(ContentPanel);
            if (addDirection == ScrollDirection.Left)
            {
                item.transform.SetAsFirstSibling();
            }
            if (addDirection == ScrollDirection.Left)
            {
                contentItems.AddFirst(item);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)(-aheadCount) * ItemSpacing, 0f);
            }
            else
            {
                contentItems.AddLast(item);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)aheadCount * ItemSpacing, 0f);
            }
        }

        private void Update()
        {
            if (!contentLoaded)
            {
                return;
            }
            if (!isScrolling)
            {
                Vector2 pointerPos = GetPointerPosition();
                bool pointerPressed = IsPointerPressed();
                bool pointerReleased = IsPointerReleased();

                if (pointerPressed)
                {
                    touchDist = 0f;
                    touchTime = 0f;
                    isTouching = true;
                    prevPointerPos = pointerPos.x;
                }
                else if (isTouching)
                {
                    touchDist += pointerPos.x - prevPointerPos;
                    prevPointerPos = pointerPos.x;
                    if (Mathf.Abs(touchDist / (float)Screen.width) > MIN_SWIPE_DIST && touchTime < MAX_SWIPE_TIME)
                    {
                        StartScroll((touchDist > 0f) ? ScrollDirection.Left : ScrollDirection.Right);
                        isTouching = false;
                    }
                    touchTime += Time.deltaTime;
                }
                if (pointerReleased)
                {
                    isTouching = false;
                }
            }
            autoScrollTimer += Time.deltaTime;
            if (autoScrollTimer >= AutoScrollWaitTime)
            {
                StartScroll(ScrollDirection.Right);
            }
        }

        protected virtual void loadNewItem(ScrollDirection direction)
        {
        }

        protected void clearContentItems()
        {
            if (contentItems != null)
            {
                LinkedList<GameObject>.Enumerator enumerator = contentItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Object.Destroy(enumerator.Current.gameObject);
                }
                contentItems.Clear();
            }
        }

        public void OnRightButtonPressed()
        {
            StartScroll(ScrollDirection.Right);
        }

        public void OnLeftButtonPressed()
        {
            StartScroll(ScrollDirection.Left);
        }

        public void StartScroll(ScrollDirection direction)
        {
            if (!isScrolling)
            {
                autoScrollTimer = 0f;
                if (LoadItemsDynamically)
                {
                    loadNewItem(direction);
                }
                else if (direction == ScrollDirection.Left)
                {
                    GameObject value = contentItems.Last.Value;
                    contentItems.RemoveLast();
                    addItem(value, ScrollDirection.Left, 2);
                }
                isScrolling = true;
                currentScrollDirection = direction;
                currentItemIndex = getNextItemIndex(direction, 1);
                iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", ItemSpacing, "time", ScrollTime, "easetype", iTween.EaseType.easeInOutExpo, "onupdatetarget", base.gameObject, "onupdate", "onScrollUpdate"));
            }
        }

        private void onScrollUpdate(float value)
        {
            int num = 0;
            foreach (GameObject contentItem in contentItems)
            {
                if (currentScrollDirection == ScrollDirection.Left)
                {
                    float x = (float)(num - 2) * ItemSpacing + value;
                    contentItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                }
                else
                {
                    float x = (float)(num - 1) * ItemSpacing - value;
                    contentItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                }
                num++;
            }
            if (value != ItemSpacing)
            {
                return;
            }
            isScrolling = false;
            updatePageIcons();
            if (LoadItemsDynamically)
            {
                if (currentScrollDirection == ScrollDirection.Left)
                {
                    Object.Destroy(contentItems.Last.Value.gameObject);
                    contentItems.RemoveLast();
                }
                else
                {
                    Object.Destroy(contentItems.First.Value.gameObject);
                    contentItems.RemoveFirst();
                }
            }
            else if (currentScrollDirection == ScrollDirection.Right)
            {
                GameObject current = contentItems.First.Value;
                contentItems.RemoveFirst();
                addItem(current, ScrollDirection.Right, contentItems.Count - 1);
            }
        }

        protected int getNextItemIndex(ScrollDirection direction, int aheadCount)
        {
            int num = currentItemIndex;
            if (totalItems == 2)
            {
                num = ((num == 0) ? 1 : 0);
            }
            else if (totalItems > 1)
            {
                num = ((direction != ScrollDirection.Right) ? (num - aheadCount) : (num + aheadCount));
                num %= totalItems;
                if (num < 0)
                {
                    num += totalItems;
                }
            }
            return num;
        }

        private void updatePageIcons()
        {
            pageIcons[currentPageIcon].SelectColor(1);
            pageIcons[currentItemIndex].SelectColor(0);
            currentPageIcon = currentItemIndex;
        }

        // --- Input System helpers for both mouse and touch ---
        private Vector2 GetPointerPosition()
        {
            // Touch (if present)
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.isPressed)
                        return touch.position.ReadValue();
                }
            }
            // Mouse fallback
            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();
            return Vector2.zero;
        }

        private bool IsPointerPressed()
        {
            // Touch
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.wasPressedThisFrame)
                        return true;
                }
            }
            // Mouse
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasPressedThisFrame;
            return false;
        }

        private bool IsPointerReleased()
        {
            // Touch
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.press.wasReleasedThisFrame)
                        return true;
                }
            }
            // Mouse
            if (Mouse.current != null)
                return Mouse.current.leftButton.wasReleasedThisFrame;
            return false;
        }
    }
}