using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	public abstract class AbstractIglooScreenController<DecorationDefinitionType, DecorationIdType> : MonoBehaviour where DecorationDefinitionType : IglooAssetDefinition<DecorationIdType>
	{
		[Header("Pooled Scroll Rect Content")]
		public PooledCellsScrollRect PooledScrollRect;

		[Tooltip("PrefabContentKey for the button used to display items on this screen")]
		public PrefabContentKey ButtonPrefabKey;

		private GameObjectPoolOverride goPoolOverride;

		private GameObject buttonPrefab;

		[Tooltip("Number of buttons that always get spawned (e.g. Catalog button)")]
		[Header("Screen Specific Settings")]
		public int numberOfStaticButtons;

		[Tooltip("FSM Event to be fired when this is the active screen")]
		[SerializeField]
		private string trayButtonPressedId;

		[Tooltip("FSM Event to be fired when this screen is disabled")]
		[SerializeField]
		private string trayButtonDisabledId;

		public bool showItemCountsWithZeroCount;

		public bool tintItemsWithZeroCount;

		[Tooltip("Breadcrumb definition for top-level breadcrumbs (i.e. those that appear on the IglooMenu)")]
		[Header("Breadcrumbs")]
		public StaticBreadcrumbDefinitionKey MenuBreadcrumbType;

		[Tooltip("Breadcrumb definition for those that appear on the item buttons")]
		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		protected CPDataEntityCollection dataEntityCollection;

		protected DataEntityHandle sceneDataHandle;

		protected SceneStateData sceneStateData;

		private DataEventListener sceneStateDataListener;

		protected SceneLayoutData sceneLayoutData;

		private DataEventListener sceneLayoutDataListener;

		protected EventDispatcher eventDispatcher;

		private PersistentIglooUIPositionData persistentPositionData;

		protected List<KeyValuePair<DecorationDefinitionType, int>> inventoryCountPairs;

		protected Dictionary<DecorationIdType, ProgressionUtils.ParsedProgression<DecorationDefinitionType>> inventoryProgressionStatus;

		protected bool displayListRetrieved;

		protected bool isDestroyed;

		private ScrollRect _scrollRect;

		protected ScrollRect scrollRect
		{
			get
			{
				if (_scrollRect == null)
				{
					_scrollRect = PooledScrollRect.GetComponent<ScrollRect>();
				}
				return _scrollRect;
			}
		}

		protected virtual void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneDataHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			sceneStateDataListener = dataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneDataAdded);
			sceneLayoutDataListener = dataEntityCollection.Whenever<SceneLayoutData>("ActiveSceneData", onSceneLayoutDataAdded, onSceneLayoutDataRemoved);
		}

		protected virtual void Start()
		{
			if (PooledScrollRect.ElementPoolOverride == null)
			{
				goPoolOverride = ClubPenguin.Core.SceneRefs.Get<GameObjectPoolOverride>();
			}
			displayListRetrieved = false;
			if (ClubPenguin.Core.SceneRefs.Get<IScreenContainerStateHandler>().IsOpen)
			{
				loadContentPrefabs();
			}
			else
			{
				eventDispatcher.AddListener<TrayEvents.TrayOpened>(onTrayOpened);
			}
		}

		protected virtual void OnEnable()
		{
			if (MenuBreadcrumbType != null)
			{
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(MenuBreadcrumbType);
			}
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement(trayButtonPressedId));
		}

		protected virtual void OnDestroy()
		{
			if (PooledScrollRect != null)
			{
				if (goPoolOverride != null)
				{
					PooledScrollRect.ResetContent();
				}
				PooledScrollRect.ObjectAdded -= onObjectAdded;
				PooledScrollRect.ObjectRemoved -= onObjectRemoved;
			}
			if (eventDispatcher != null)
			{
				eventDispatcher.RemoveListener<TrayEvents.TrayOpened>(onTrayOpened);
			}
			isDestroyed = true;
			if (sceneStateDataListener != null)
			{
				sceneStateDataListener.StopListening();
			}
			if (sceneLayoutDataListener != null)
			{
				sceneLayoutDataListener.StopListening();
			}
			CoroutineRunner.StopAllForOwner(this);
		}

		protected virtual void OnDisable()
		{
			if (persistentPositionData != null)
			{
				persistentPositionData.Position = scrollRect.normalizedPosition;
			}
			eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement(trayButtonPressedId));
			if (sceneStateData.State == SceneStateData.SceneState.Preview)
			{
				eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement(trayButtonDisabledId));
			}
		}

		private bool onTrayOpened(TrayEvents.TrayOpened evt)
		{
			eventDispatcher.RemoveListener<TrayEvents.TrayOpened>(onTrayOpened);
			loadContentPrefabs();
			return false;
		}

		protected virtual void loadContentPrefabs()
		{
			Content.LoadAsync(onButtonPrefabLoaded, ButtonPrefabKey);
		}

		private void onButtonPrefabLoaded(string path, GameObject loadedItemPrefab)
		{
			buttonPrefab = loadedItemPrefab;
			GetDisplayedDefinitionsList();
			displayListRetrieved = true;
			if (isAllContentLoaded())
			{
				setPoolOverride();
			}
		}

		protected virtual bool isAllContentLoaded()
		{
			return displayListRetrieved;
		}

		protected void setPoolOverride()
		{
			if (goPoolOverride == null)
			{
				setupPooledScrollRect(GetDisplayCount());
			}
			else if (!goPoolOverride.IsReady)
			{
				goPoolOverride.ObjectPoolReady += onPoolOverrideReady;
			}
			else
			{
				setupPooledScrollRect(GetDisplayCount(), true);
			}
		}

		private void onPoolOverrideReady()
		{
			goPoolOverride.ObjectPoolReady -= onPoolOverrideReady;
			setupPooledScrollRect(GetDisplayCount(), true);
		}

		private void setupPooledScrollRect(int count, bool overridePool = false)
		{
			if (overridePool)
			{
				PooledScrollRect.ElementPoolOverride = goPoolOverride.ObjectPool;
			}
			PooledScrollRect.gameObject.SetActive(true);
			PooledScrollRect.ObjectAdded += onObjectAdded;
			PooledScrollRect.ObjectRemoved += onObjectRemoved;
			PooledScrollRect.Init(count, buttonPrefab);
			CoroutineRunner.Start(setScrollRectPersistentPosition(), this, "Waiting for Pooled ScrollRect");
		}

		private IEnumerator setScrollRectPersistentPosition()
		{
			if (!dataEntityCollection.TryGetComponent(sceneDataHandle, out persistentPositionData))
			{
				persistentPositionData = dataEntityCollection.AddComponent<PersistentIglooUIPositionData>(sceneDataHandle);
			}
			if (persistentPositionData.ScreenName != base.name)
			{
				persistentPositionData.ScreenName = base.name;
				yield break;
			}
			while (!PooledScrollRect.IsInitialized)
			{
				yield return null;
			}
			scrollRect.normalizedPosition = persistentPositionData.Position;
		}

		protected void resetScrollRectPersistentPosition()
		{
			if (!dataEntityCollection.TryGetComponent(sceneDataHandle, out persistentPositionData))
			{
				persistentPositionData = dataEntityCollection.AddComponent<PersistentIglooUIPositionData>(sceneDataHandle);
			}
			persistentPositionData.ScreenName = "";
		}

		protected virtual void onObjectAdded(RectTransform item, int index)
		{
			item.anchoredPosition = Vector2.zero;
			item.anchorMin = Vector2.zero;
			item.anchorMax = Vector2.one;
			item.sizeDelta = Vector2.zero;
			item.localScale = Vector3.one;
			IglooCustomizationButton component = item.GetComponent<IglooCustomizationButton>();
			if (component != null)
			{
				SetupIglooCustomizationButton(component, index);
			}
		}

		protected virtual void onObjectRemoved(RectTransform item, int index)
		{
		}

		private void onSceneDataAdded(SceneStateData stateData)
		{
			sceneStateData = stateData;
		}

		private void onSceneLayoutDataAdded(SceneLayoutData layoutData)
		{
			sceneLayoutData = layoutData;
		}

		private void onSceneLayoutDataRemoved(SceneLayoutData layoutData)
		{
			sceneLayoutData = null;
		}

		public void Reload()
		{
			GetDisplayedDefinitionsList();
			PooledScrollRect.RefreshList(GetDisplayCount());
		}

		protected virtual bool SetLockableButtonLockedStatus(ILockableButton button, DecorationDefinitionType definition, ProgressionUtils.ParsedProgression<DecorationDefinitionType> progressData)
		{
			bool result = true;
			if (progressData != null)
			{
				if (progressData.MemberLocked && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
				{
					button.SetMemberLocked();
				}
				else if (progressData.LevelLocked)
				{
					button.SetLevelLocked(progressData.Level);
				}
				else if (progressData.ProgressionLocked)
				{
					button.SetProgressionLocked(progressData.MascotName);
				}
				else
				{
					button.SetUnlocked();
					result = false;
				}
			}
			else if (definition.IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				button.SetMemberLocked();
			}
			else
			{
				button.SetUnlocked();
				result = false;
			}
			return result;
		}

		protected virtual int GetDisplayCount()
		{
			return inventoryCountPairs.Count;
		}

		protected virtual void GetDisplayedDefinitionsList()
		{
			List<ProgressionUtils.ParsedProgression<DecorationDefinitionType>> progressionList = GetProgressionList();
			inventoryProgressionStatus = new Dictionary<DecorationIdType, ProgressionUtils.ParsedProgression<DecorationDefinitionType>>(progressionList.Count);
			foreach (ProgressionUtils.ParsedProgression<DecorationDefinitionType> item in progressionList)
			{
				Dictionary<DecorationIdType, ProgressionUtils.ParsedProgression<DecorationDefinitionType>> dictionary = inventoryProgressionStatus;
				DecorationDefinitionType definition = item.Definition;
				dictionary.Add(definition.GetId(), item);
			}
			inventoryCountPairs = GetAvailableItems();
		}

		protected virtual void SetupIglooCustomizationButton(IglooCustomizationButton button, int index)
		{
			if (index < numberOfStaticButtons)
			{
				button.SetupCatalogButton();
			}
			else
			{
				button.SetupItemButton();
				int index2 = index - numberOfStaticButtons;
				DecorationDefinitionType key = inventoryCountPairs[index2].Key;
				button.gameObject.name = key.Name + "_button";
				button.Init(key.Icon, BreadcrumbType, GetIntegerDefinitionId(key), true);
				ProgressionUtils.ParsedProgression<DecorationDefinitionType> value;
				inventoryProgressionStatus.TryGetValue(key.GetId(), out value);
				if (!SetLockableButtonLockedStatus(button, key, value))
				{
					int value2 = inventoryCountPairs[index2].Value;
					button.SetItemCount(value2, showItemCountsWithZeroCount, tintItemsWithZeroCount);
					if (value2 <= 0)
					{
						button.SetCanDrag(false);
					}
				}
				AccessibilitySettings component = button.GetComponent<AccessibilitySettings>();
				if (component != null)
				{
					component.CustomToken = key.Name;
				}
			}
			button.SetDragReferences(scrollRect, index, 0f);
		}

		protected abstract List<KeyValuePair<DecorationDefinitionType, int>> GetAvailableItems();

		protected abstract List<ProgressionUtils.ParsedProgression<DecorationDefinitionType>> GetProgressionList();

		protected virtual int GetIntegerDefinitionId(DecorationDefinitionType definition)
		{
			return -1;
		}
	}
}
