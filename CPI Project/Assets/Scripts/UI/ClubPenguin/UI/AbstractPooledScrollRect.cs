using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public abstract class AbstractPooledScrollRect : MonoBehaviour
	{
		private enum ScrollDirection
		{
			beginning,
			end
		}

		protected const float CONTENT_BOUNDS_FACTOR = 1.1f;

		private bool isScrollingAllowed = true;

		[Header("Keeping cells speeds up scrolling but increases memory usage.")]
		public bool KeepInvisibleCells = false;

		private RectTransform _rectTransform;

		[Header("If this is not null no pool is created.")]
		public GameObjectPool ElementPoolOverride;

		protected ScrollRect scrollRect;

		protected GameObjectPool elementPool;

		private GameObject elementItemPrefab;

		private float contentStartBound;

		private float contentEndBound;

		private int previousPosition;

		private bool isInitialized;

		protected float customStartPosition;

		private float movingTimeoutSeconds = 0.5f;

		private ICoroutine movingCooldownTimer;

		private ICoroutine waitForLayoutCoroutine;

		private ICoroutine resizePoolCoroutine;

		public bool IsScrollingAllowed
		{
			get
			{
				return isScrollingAllowed;
			}
			set
			{
				if (scrollRect != null)
				{
					if (value)
					{
						scrollRect.onValueChanged.AddListener(onScrollRectUpdate);
					}
					else
					{
						scrollRect.onValueChanged.RemoveListener(onScrollRectUpdate);
					}
				}
				isScrollingAllowed = value;
			}
		}

		public bool IsMoving
		{
			get;
			private set;
		}

		protected RectTransform rectTransform
		{
			get
			{
				if (_rectTransform == null)
				{
					_rectTransform = (RectTransform)base.transform;
				}
				return _rectTransform;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return isInitialized;
			}
		}

		protected abstract float rectSize
		{
			get;
		}

		protected abstract float position
		{
			get;
		}

		protected abstract float sizeDelta
		{
			get;
		}

		public event Action<RectTransform, int> ObjectAdded;

		public event Action<RectTransform, int> ObjectRemoved;

		public event Action<bool> OnIsMovingChanged;

		public event System.Action OnRefreshCompleted;

		protected virtual void awake()
		{
		}

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			awake();
		}

		private void OnRectTransformDimensionsChange()
		{
			contentStartBound = 0f - rectSize * 1.1f;
			contentEndBound = rectSize * 1.1f;
		}

		private void OnEnable()
		{
			CoroutineRunner.Start(ReloadCells(), this, "AbstractPooledScrollRect.ReloadCells");
		}

		private IEnumerator ReloadCells()
		{
			yield return new WaitForEndOfFrame();
			checkAllCells(ScrollDirection.beginning);
		}

		public void Init(int cellCount, GameObject elementItemPrefab, bool isAsync = false)
		{
			if (!isInitialized)
			{
				waitForLayoutCoroutine = CoroutineRunner.Start(waitForLayoutGroupAdjustments(elementItemPrefab, cellCount, isAsync), this, "waitForLayoutGroupAdjustments");
			}
			else
			{
				Log.LogError(this, "Attempted to initialize a PooledScrollRect that was already initialized");
			}
		}

		private IEnumerator waitForLayoutGroupAdjustments(GameObject elementItemPrefab, int cellCount, bool isAsync)
		{
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			contentStartBound = 0f - rectSize * 1.1f;
			contentEndBound = rectSize * 1.1f;
			this.elementItemPrefab = elementItemPrefab;
			if (ElementPoolOverride == null)
			{
				setUpElementPool(cellCount, isAsync);
			}
			else
			{
				elementPool = ElementPoolOverride;
				resizePoolForItemCount(cellCount);
			}
			scrollRect.onValueChanged.AddListener(onScrollRectUpdate);
			if (!isAsync)
			{
				for (int i = 0; i < cellCount; i++)
				{
					RectTransform rectTransform = createEmptyCell();
					rectTransform.SetParent(scrollRect.content, false);
				}
				LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
				yield return new WaitForEndOfFrame();
				checkAllCells(ScrollDirection.beginning);
			}
			scrollRect.horizontalNormalizedPosition = Mathf.Clamp(customStartPosition / (0f - sizeDelta), 0f, 1f);
			scrollRect.verticalNormalizedPosition = 1f;
			isInitialized = true;
			waitForLayoutCoroutine = null;
		}

		private void setUpElementPool(int itemCount, bool isAsync)
		{
			elementPool = base.gameObject.AddComponent<GameObjectPool>();
			elementPool.PrefabToInstance = elementItemPrefab;
			if (isAsync)
			{
				resizePoolCoroutine = CoroutineRunner.Start(resizePoolForItemCountAsync(itemCount), this, "resizePoolForItemCountAsync");
			}
			else
			{
				resizePoolForItemCount(itemCount);
			}
			elementPool.enabled = true;
		}

		private void resizePoolForItemCount(int itemCount)
		{
			if (!(ElementPoolOverride != null) || elementPool.Capacity < itemCount)
			{
				elementPool.Capacity = Math.Min(itemCount, getPoolSize(elementItemPrefab));
			}
		}

		private IEnumerator resizePoolForItemCountAsync(int itemCount)
		{
			int cellCount = 0;
			while (cellCount < itemCount)
			{
				RectTransform newBlankCell = createEmptyCell();
				newBlankCell.transform.SetParent(scrollRect.content, false);
				updateCell(newBlankCell);
				cellCount++;
				yield return null;
			}
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			resizePoolCoroutine = null;
		}

		public void ResetContent()
		{
			for (int num = scrollRect.content.childCount - 1; num >= 0; num--)
			{
				RectTransform rectTransform = (RectTransform)scrollRect.content.GetChild(num);
				if (rectTransform.childCount > 0)
				{
					removeElementFromCell(num);
				}
			}
		}

		public void RefreshList(int newSize)
		{
			if (waitForLayoutCoroutine != null && !waitForLayoutCoroutine.Disposed)
			{
				waitForLayoutCoroutine.Stop();
			}
			if (resizePoolCoroutine != null && !resizePoolCoroutine.Disposed)
			{
				resizePoolCoroutine.Stop();
			}
			int childCount = scrollRect.content.childCount;
			for (int i = 0; i < childCount; i++)
			{
				RectTransform rectTransform = (RectTransform)scrollRect.content.GetChild(i);
				if (rectTransform.childCount > 0 && this.ObjectRemoved != null)
				{
					this.ObjectRemoved.InvokeSafe((RectTransform)rectTransform.GetChild(0), i);
				}
			}
			if (newSize > childCount)
			{
				if (elementPool == null)
				{
					setUpElementPool(newSize, false);
				}
				else
				{
					resizePoolForItemCount(newSize);
				}
				int num = newSize - childCount;
				for (int i = 0; i < num; i++)
				{
					RectTransform rectTransform2 = createEmptyCell();
					rectTransform2.SetParent(scrollRect.content, false);
				}
			}
			else if (newSize < childCount)
			{
				for (int i = childCount - 1; i > newSize - 1; i--)
				{
					RectTransform rectTransform3 = (RectTransform)scrollRect.content.GetChild(i);
					if (rectTransform3.childCount > 0)
					{
						removeElementFromCell(i, false);
					}
					rectTransform3.transform.SetParent(null);
					UnityEngine.Object.Destroy(rectTransform3.gameObject);
				}
			}
			scrollRect.horizontalNormalizedPosition = Mathf.Clamp(customStartPosition / (0f - sizeDelta), 0f, 1f);
			scrollRect.verticalNormalizedPosition = 1f;
			if (this.OnRefreshCompleted != null)
			{
				this.OnRefreshCompleted.InvokeSafe();
			}
			for (int i = 0; i < newSize; i++)
			{
				RectTransform rectTransform = (RectTransform)scrollRect.content.GetChild(i);
				if (rectTransform.childCount > 0 && this.ObjectAdded != null)
				{
					this.ObjectAdded.InvokeSafe((RectTransform)rectTransform.GetChild(0), i);
				}
			}
			CoroutineRunner.Start(forceLayoutRebuildThenCheckCells(), this, "forceLayoutRebuildThenCheckCells");
		}

		public bool IsIndexCellVisible(int index)
		{
			if (!base.gameObject.IsDestroyed() && scrollRect != null && scrollRect.content != null)
			{
				return scrollRect.content.childCount > index && scrollRect.content.GetChild(index).childCount > 0;
			}
			return false;
		}

		public RectTransform GetCellAtIndex(int index)
		{
			return (RectTransform)scrollRect.content.GetChild(index).GetChild(0);
		}

		private IEnumerator forceLayoutRebuildThenCheckCells()
		{
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
			yield return new WaitForEndOfFrame();
			checkAllCells(ScrollDirection.beginning);
			CoroutineRunner.Start(checkContentPosition(), this, "checkContentPosition");
		}

		private IEnumerator checkContentPosition()
		{
			yield return new WaitForEndOfFrame();
			if (position < 0f - sizeDelta)
			{
				scrollRect.horizontalNormalizedPosition = 1f;
				scrollRect.verticalNormalizedPosition = 0f;
			}
		}

		private RectTransform createEmptyCell()
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<RectTransform>();
			gameObject.name = "Cell";
			setUpEmptyCell(gameObject);
			return gameObject.transform as RectTransform;
		}

		private void onScrollRectUpdate(Vector2 position)
		{
			if (!IsScrollingAllowed)
			{
				return;
			}
			int scrollPosition = getScrollPosition(position);
			if (previousPosition != scrollPosition)
			{
				startMoving();
				if (previousPosition > scrollPosition)
				{
					ScrollDirection dir = ScrollDirection.beginning;
					checkAllCells(dir);
				}
				else
				{
					ScrollDirection dir = ScrollDirection.end;
					checkAllCells(dir);
				}
				previousPosition = scrollPosition;
			}
		}

		private void startMoving()
		{
			setIsMoving(true);
			if (movingCooldownTimer != null && !movingCooldownTimer.Disposed)
			{
				movingCooldownTimer.Stop();
			}
			movingCooldownTimer = CoroutineRunner.Start(waitForMovingTimeout(), this, "waitForMovingTimeout");
		}

		private IEnumerator waitForMovingTimeout()
		{
			yield return new WaitForSeconds(movingTimeoutSeconds);
			setIsMoving(false);
		}

		private void setIsMoving(bool isMoving)
		{
			if (IsMoving != isMoving)
			{
				IsMoving = isMoving;
				if (this.OnIsMovingChanged != null)
				{
					this.OnIsMovingChanged.InvokeSafe(isMoving);
				}
			}
		}

		private void checkAllCells(ScrollDirection dir)
		{
			if (!IsScrollingAllowed)
			{
				return;
			}
			RectTransform content = scrollRect.content;
			int childCount = content.childCount;
			if (KeepInvisibleCells)
			{
				for (int i = 0; i < childCount; i++)
				{
					updateCellPersistContent(content.GetChild(i) as RectTransform);
				}
				return;
			}
			if (dir == ScrollDirection.end)
			{
				for (int i = 0; i < childCount; i++)
				{
					updateCell(content.GetChild(i) as RectTransform);
				}
				return;
			}
			for (int i = childCount - 1; i >= 0; i--)
			{
				updateCell(content.GetChild(i) as RectTransform);
			}
		}

		private void updateCell(RectTransform cell)
		{
			if (isCellVisible(cell))
			{
				if (cell.childCount == 0)
				{
					addElementToCell(cell);
				}
			}
			else if (cell.childCount > 0)
			{
				removeElementFromCell(cell.GetSiblingIndex());
			}
		}

		private void updateCellPersistContent(RectTransform cell)
		{
			if (isCellVisible(cell))
			{
				if (cell.childCount == 0)
				{
					addElementToCell(cell);
				}
				if (cell.childCount > 0)
				{
					cell.GetChild(0).gameObject.SetActive(true);
				}
			}
			else if (cell.childCount > 0)
			{
				cell.GetChild(0).gameObject.SetActive(false);
			}
		}

		private bool isCellVisible(RectTransform cell)
		{
			if (getPosition(cell) < contentStartBound)
			{
				return false;
			}
			if (getPosition(cell) > contentEndBound)
			{
				return false;
			}
			return true;
		}

		private void addElementToCell(RectTransform cell)
		{
			RectTransform rectTransform = elementPool.Spawn().transform as RectTransform;
			rectTransform.SetParent(cell, false);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			if (this.ObjectAdded != null)
			{
				this.ObjectAdded.InvokeSafe(rectTransform, cell.GetSiblingIndex());
			}
		}

		private void removeElementFromCell(int index, bool fireEvent = true)
		{
			RectTransform rectTransform = scrollRect.content.GetChild(index) as RectTransform;
			if (rectTransform.childCount > 0)
			{
				RectTransform rectTransform2 = rectTransform.GetChild(0) as RectTransform;
				elementPool.Unspawn(rectTransform2.gameObject);
				if (fireEvent && this.ObjectRemoved != null)
				{
					this.ObjectRemoved.InvokeSafe(rectTransform2, index);
				}
			}
		}

		protected abstract int getPoolSize(GameObject item);

		protected abstract int getScrollPosition(Vector2 position);

		protected abstract float getPosition(RectTransform item);

		protected virtual void setUpEmptyCell(GameObject cell)
		{
		}

		public virtual void CenterOnElement(int elementIndex)
		{
			throw new NotImplementedException();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			scrollRect.onValueChanged.RemoveListener(onScrollRectUpdate);
			this.ObjectAdded = null;
			this.ObjectRemoved = null;
			this.OnIsMovingChanged = null;
			this.OnRefreshCompleted = null;
		}
	}
}
