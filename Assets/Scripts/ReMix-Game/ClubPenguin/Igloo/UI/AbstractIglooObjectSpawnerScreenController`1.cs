#define UNITY_ASSERTIONS
using ClubPenguin.ClothingDesigner;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.Progression;
using ClubPenguin.SceneManipulation;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
    public abstract class AbstractIglooObjectSpawnerScreenController<DecorationDefinitionType> : AbstractIglooScreenController<DecorationDefinitionType, int> where DecorationDefinitionType : IglooAssetDefinition<int>
    {
        [Header("Drag Container")]
        [Tooltip("The prefab that should be spawned under the user's input when they begin draggin an item")]
        public PrefabContentKey DragContainerContentKey;

        private DragContainer dragContainerInstance;

        private float totalDragHeight;

        public DecorationType decorationType;

        public DecorationLayoutData.DefinitionType decorationDefinitionType;

        private ReturnToFSMStateOnInteractionState returnToPreviousState;

        private ObjectManipulationInputController objectManipulationInputController;

        private RecentDecorationsService recentDecorationsService;

        protected SceneManipulationService sceneManipulationService;

        protected DecorationInventoryService decorationInventoryService;

        protected DecorationCategoryManager decorationCategoryManager;

        protected override void Start()
        {
            base.Start();
            if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
            {
                sceneManipulationService = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>();
                sceneManipulationService.ObjectRemoved += onObjectAddedOrRemoved;
                sceneManipulationService.ObjectAdded += onObjectAddedOrRemoved;
                sceneManipulationService.NewObjectCreated += onObjectAddedOrRemoved;
            }
            if (ClubPenguin.Core.SceneRefs.IsSet<ObjectManipulationInputController>())
            {
                objectManipulationInputController = ClubPenguin.Core.SceneRefs.Get<ObjectManipulationInputController>();
                objectManipulationInputController.InteractionStateChanged += onInteractionStateChanged;
            }
            recentDecorationsService = Service.Get<RecentDecorationsService>();
            decorationInventoryService = Service.Get<DecorationInventoryService>();
            decorationCategoryManager = GetComponentInChildren<DecorationCategoryManager>();
            if (decorationCategoryManager != null)
            {
                DecorationCategoryManager obj = decorationCategoryManager;
                obj.CategoryRefreshedEvent = (CategoryRefreshedEvent)Delegate.Combine(obj.CategoryRefreshedEvent, new CategoryRefreshedEvent(onCategoryUpdated));
            }
            totalDragHeight = (base.transform as RectTransform).rect.height;
            GameObject gameObject = GameObject.Find("IglooMenuLoader");
            if (gameObject != null)
            {
                totalDragHeight += (gameObject.transform as RectTransform).rect.height;
            }
            Canvas componentInParent = GetComponentInParent<Canvas>();
            if (componentInParent != null)
            {
                Assert.AreEqual(componentInParent.transform.localScale.x, componentInParent.transform.localScale.y);
                totalDragHeight *= componentInParent.transform.localScale.x;
            }
            returnToPreviousState = GetComponentInParent<ReturnToFSMStateOnInteractionState>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (sceneManipulationService != null)
            {
                sceneManipulationService.ObjectRemoved -= onObjectAddedOrRemoved;
                sceneManipulationService.ObjectAdded -= onObjectAddedOrRemoved;
                sceneManipulationService.NewObjectCreated -= onObjectAddedOrRemoved;
            }
            if (decorationCategoryManager != null)
            {
                DecorationCategoryManager obj = decorationCategoryManager;
                obj.CategoryRefreshedEvent = (CategoryRefreshedEvent)Delegate.Remove(obj.CategoryRefreshedEvent, new CategoryRefreshedEvent(onCategoryUpdated));
            }
            if (objectManipulationInputController != null)
            {
                objectManipulationInputController.InteractionStateChanged -= onInteractionStateChanged;
            }
            if (dragContainerInstance != null)
            {
                UnityEngine.Object.Destroy(dragContainerInstance);
            }
        }

        protected override void loadContentPrefabs()
        {
            base.loadContentPrefabs();
            Content.LoadAsync(onDragContainerLoaded, DragContainerContentKey);
        }

        protected void onDragContainerLoaded(string path, GameObject asset)
        {
            if (!isDestroyed)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(asset, base.transform);
                dragContainerInstance = gameObject.GetComponent<DragContainer>();
                dragContainerInstance.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                gameObject.SetActive(false);
                if (isAllContentLoaded())
                {
                    setPoolOverride();
                }
            }
        }

        protected override bool isAllContentLoaded()
        {
            return displayListRetrieved && dragContainerInstance != null;
        }

        protected override void onObjectAdded(RectTransform item, int index)
        {
            base.onObjectAdded(item, index);
            IglooCustomizationButton component = item.GetComponent<IglooCustomizationButton>();
            component.SetDragReferences(base.scrollRect, index, totalDragHeight, dragContainerInstance);
            component.DraggedOffDragArea += onDraggedOffDragArea;
        }

        protected override void onObjectRemoved(RectTransform item, int index)
        {
            IglooCustomizationButton component = item.GetComponent<IglooCustomizationButton>();
            component.DraggedOffDragArea -= onDraggedOffDragArea;
            base.onObjectRemoved(item, index);
        }

        protected void onDraggedOffDragArea(IglooCustomizationButton iglooCustomizationButton, Vector2 finalTouchPoint, int index)
        {
            transitionToInteraction(index, finalTouchPoint);
        }

        protected void transitionToInteraction(int index, Vector2 finalTouchPoint)
        {
            EventSystem.current.SetSelectedGameObject(null);
            setDataAndTemporarilyClose();
            int index2 = index - numberOfStaticButtons;
            int value = inventoryCountPairs[index2].Value;
            if (value > 0)
            {
                dispatchAddNewObjectEvent(inventoryCountPairs[index2].Key, finalTouchPoint);
            }
        }

        protected abstract void dispatchAddNewObjectEvent(DecorationDefinitionType definition, Vector2 finalTouchPoint);

        protected void onInteractionStateChanged(InteractionState interactionState)
        {
            if (interactionState == InteractionState.DragItem)
            {
                setDataAndTemporarilyClose();
            }
        }

        protected void setDataAndTemporarilyClose()
        {
            if (returnToPreviousState != null)
            {
                returnToPreviousState.SetCurrentTargetAndState("IglooScreenContainer", "max");
                returnToPreviousState.SetPreviousState(base.gameObject);
            }
            base.gameObject.SetActive(false);
            StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
            componentInParent.SendEvent(new ExternalEvent("IglooScreenContainer", "min"));
        }

        protected void onObjectAddedOrRemoved(ManipulatableObject manipulatableObject)
        {
            inventoryCountPairs = GetAvailableItems();
            if (manipulatableObject.Type != decorationDefinitionType || inventoryCountPairs == null)
            {
                return;
            }
            inventoryCountPairs = sortInventoryList(inventoryCountPairs);
            bool flag = false;
            int count = inventoryCountPairs.Count;
            for (int i = 0; i < count; i++)
            {
                DecorationDefinitionType key = inventoryCountPairs[i].Key;
                if (key.GetId() == manipulatableObject.DefinitionId)
                {
                    if (PooledScrollRect.IsIndexCellVisible(i + numberOfStaticButtons))
                    {
                        IglooCustomizationButton component = PooledScrollRect.GetCellAtIndex(i + numberOfStaticButtons).GetComponent<IglooCustomizationButton>();
                        component.SetItemCount(inventoryCountPairs[i].Value, showItemCountsWithZeroCount, tintItemsWithZeroCount);
                    }
                    objectAddedOrRemoved(i, manipulatableObject);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                manipulatedObjectNotFound(manipulatableObject);
            }
        }

        protected virtual void objectAddedOrRemoved(int itemIndex, ManipulatableObject manipulatableObject)
        {
        }

        protected virtual void manipulatedObjectNotFound(ManipulatableObject manipulatableObject)
        {
        }

        protected virtual void onCategoryUpdated(int newCategory)
        {
            Reload();
        }

        protected override int GetDisplayCount()
        {
            return inventoryCountPairs.Count + numberOfStaticButtons;
        }

        protected override int GetIntegerDefinitionId(DecorationDefinitionType definition)
        {
            return definition.GetId();
        }

        protected override void GetDisplayedDefinitionsList()
        {
            base.GetDisplayedDefinitionsList();
            inventoryCountPairs = sortInventoryList(inventoryCountPairs);
            scrollToLatestPurchase();
        }

        protected void scrollToLatestPurchase()
        {
            if (recentDecorationsService == null || !recentDecorationsService.ShouldShowMostRecentPurchase || recentDecorationsService.MostRecentPurchaseType != decorationType)
            {
                return;
            }
            int elementIndex = 0;
            for (int i = 0; i < inventoryCountPairs.Count; i++)
            {
                DecorationDefinitionType key = inventoryCountPairs[i].Key;
                if (key.GetId() == recentDecorationsService.MostRecentPurchaseId)
                {
                    elementIndex = i;
                    break;
                }
            }
            PooledScrollRect.CenterOnElement(elementIndex);
            resetScrollRectPersistentPosition();
            recentDecorationsService.ShouldShowMostRecentPurchase = false;
        }

        protected List<KeyValuePair<DecorationDefinitionType, int>> sortInventoryList(List<KeyValuePair<DecorationDefinitionType, int>> listToSort)
        {
            List<KeyValuePair<DecorationDefinitionType, int>> list = new List<KeyValuePair<DecorationDefinitionType, int>>();
            List<KeyValuePair<DecorationDefinitionType, int>> list2 = new List<KeyValuePair<DecorationDefinitionType, int>>();
            SortedDictionary<int, List<KeyValuePair<DecorationDefinitionType, int>>> sortedDictionary = new SortedDictionary<int, List<KeyValuePair<DecorationDefinitionType, int>>>();
            SortedDictionary<string, List<KeyValuePair<DecorationDefinitionType, int>>> sortedDictionary2 = new SortedDictionary<string, List<KeyValuePair<DecorationDefinitionType, int>>>();
            bool flag = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
            for (int i = 0; i < listToSort.Count; i++)
            {
                KeyValuePair<DecorationDefinitionType, int> item = listToSort[i];
                bool flag2 = false;
                Dictionary<int, ProgressionUtils.ParsedProgression<DecorationDefinitionType>> inventoryProgressionStatus = base.inventoryProgressionStatus;
                DecorationDefinitionType key = item.Key;
                ProgressionUtils.ParsedProgression<DecorationDefinitionType> value;
                if (inventoryProgressionStatus.TryGetValue(key.GetId(), out value))
                {
                    if (value.MemberLocked && !flag)
                    {
                        list2.Add(item);
                        flag2 = true;
                    }
                    else if (value.LevelLocked)
                    {
                        if (!sortedDictionary.ContainsKey(value.Level))
                        {
                            sortedDictionary.Add(value.Level, new List<KeyValuePair<DecorationDefinitionType, int>>());
                        }
                        sortedDictionary[value.Level].Add(item);
                        flag2 = true;
                    }
                    else if (value.ProgressionLocked)
                    {
                        if (!sortedDictionary2.ContainsKey(value.MascotName))
                        {
                            sortedDictionary2.Add(value.MascotName, new List<KeyValuePair<DecorationDefinitionType, int>>());
                        }
                        sortedDictionary2[value.MascotName].Add(item);
                        flag2 = true;
                    }
                }
                else
                {
                    key = item.Key;
                    if (key.IsMemberOnly && !flag)
                    {
                        list2.Add(item);
                        flag2 = true;
                    }
                }
                if (!flag2)
                {
                    list.Add(item);
                }
            }
            foreach (KeyValuePair<int, List<KeyValuePair<DecorationDefinitionType, int>>> item2 in sortedDictionary)
            {
                list.AddRange(item2.Value);
            }
            foreach (KeyValuePair<string, List<KeyValuePair<DecorationDefinitionType, int>>> item3 in sortedDictionary2)
            {
                list.AddRange(item3.Value);
            }
            list.AddRange(list2);
            return list;
        }
    }
}
