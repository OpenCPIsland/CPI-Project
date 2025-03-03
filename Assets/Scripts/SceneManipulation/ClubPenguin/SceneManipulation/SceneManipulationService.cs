#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.Utils;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.SceneManipulation
{
	public class SceneManipulationService : MonoBehaviour
	{
		public NumberTracker numberTracker = new NumberTracker();

		[Tooltip("Indicate where to offset new item before the raycast calculation snaps it in world")]
		public float YOffsetNewItem = -5f;

		private DecorationInventoryService decorationInventoryService;

		private EventDispatcher eventDispatcher;

		private Dictionary<int, StructureDefinition> structureDefinitions;

		private Dictionary<int, DecorationDefinition> decorationDefinitions;

		private CPDataEntityCollection dataEntityCollection;

		private DataEntityHandle sceneHandle;

		private ISceneModifier[] sceneModifiers;

		private string selectedObjectStartingId;

		private bool isNewObject = false;

		private Transform sceneLayoutContainer = null;

		private EventChannel awakeEvents;

		private ObjectManipulationInputController _objectManipulationInputController;

		private ObjectManipulationManager _objectManipulationManager;

		private PrefabCacheTracker _prefabCacheTracker;

		public SceneLayoutData SceneLayoutData
		{
			get;
			private set;
		}

		public bool IsObjectSelectedForAdd
		{
			get
			{
				return selectedObjectStartingId == null;
			}
		}

		public ISceneModifier[] SceneModifiers
		{
			get
			{
				return sceneModifiers;
			}
		}

		public ObjectManipulationInputController ObjectManipulationInputController
		{
			get
			{
				if (_objectManipulationInputController == null && SceneRefs.IsSet<ObjectManipulationInputController>())
				{
					_objectManipulationInputController = SceneRefs.Get<ObjectManipulationInputController>();
				}
				return _objectManipulationInputController;
			}
		}

		private ObjectManipulationManager objectManipulationManager
		{
			get
			{
				if (_objectManipulationManager == null)
				{
					_objectManipulationManager = UnityEngine.Object.FindObjectOfType<ObjectManipulationManager>();
				}
				return _objectManipulationManager;
			}
		}

		private PrefabCacheTracker prefabCacheTracker
		{
			get
			{
				if (_prefabCacheTracker == null)
				{
					if (!SceneRefs.IsSet<PrefabCacheTracker>())
					{
						return null;
					}
					_prefabCacheTracker = SceneRefs.Get<PrefabCacheTracker>();
				}
				return _prefabCacheTracker;
			}
		}

		public event Action<ManipulatableObject> ObjectAdded;

		public event Action<ManipulatableObject> ObjectRemoved;

		public event Action<ManipulatableObject> ObjectDeselected;

		public event Action<ManipulatableObject> NewObjectCreated;

		public string GetRelativeGameObjectPath(GameObject obj)
		{
			int num = sceneLayoutContainer.GetPath().Length + 1;
			string path = obj.GetPath();
			if (num < path.Length)
			{
				return path.Substring(num);
			}
			return "";
		}

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventDispatcher = Service.Get<EventDispatcher>();
			decorationInventoryService = Service.Get<DecorationInventoryService>();
			sceneHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			awakeEvents = new EventChannel(eventDispatcher);
			awakeEvents.AddListener<SceneTransitionEvents.LayoutGameObjectsLoaded>(onLayoutGameObjectsLoaded);
			sceneModifiers = new ISceneModifier[1]
			{
				new EditModeModifier(this)
			};
			IGameData gameData = Service.Get<IGameData>();
			decorationDefinitions = gameData.Get<Dictionary<int, DecorationDefinition>>();
			structureDefinitions = gameData.Get<Dictionary<int, StructureDefinition>>();
			SceneRefs.Set(this);
		}

		private void OnEnable()
		{
			if (ObjectManipulationInputController != null)
			{
				ObjectManipulationInputController.ObjectSelected += onObjectSelected;
				ObjectManipulationInputController.ObjectDeselected += onObjectedDeselected;
				ObjectManipulationInputController.ObjectBeforeDelete += onObjectBeforeDelete;
			}
			if (sceneLayoutContainer != null)
			{
				ObjectManipulationInputController.Container = sceneLayoutContainer;
				addObjectManipulators();
			}
		}

		private void OnDisable()
		{
			if (ObjectManipulationInputController != null)
			{
				ObjectManipulationInputController.ObjectSelected -= onObjectSelected;
				ObjectManipulationInputController.ObjectDeselected -= onObjectedDeselected;
				ObjectManipulationInputController.ObjectBeforeDelete -= onObjectBeforeDelete;
			}
			if (sceneLayoutContainer != null)
			{
				removeObjectManipulators();
			}
		}

		private void OnDestroy()
		{
			this.ObjectAdded = null;
			this.ObjectRemoved = null;
			SceneRefs.Remove(this);
			awakeEvents.RemoveAllListeners();
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				sceneModifiers[i].Destroy();
			}
		}

		public bool IsLayoutAtMaxItemLimit()
		{
			if (ObjectManipulationInputController != null && ObjectManipulationInputController.CurrentlySelectedObject != null)
			{
				DecorationLayoutData selectedItem = default(DecorationLayoutData);
				selectedItem.Id.Name = ObjectManipulationInputController.CurrentlySelectedObject.name;
				selectedItem.Id.ParentPath = GetRelativeGameObjectPath(ObjectManipulationInputController.CurrentlySelectedObject.transform.parent.gameObject);
				bool selectedItemIsAPair = ObjectManipulationInputController.CurrentlySelectedObject.GetComponent<SplittableObject>() != null;
				return SceneLayoutData.IsLayoutAtMaxItemLimit(selectedItem, selectedItemIsAPair);
			}
			return SceneLayoutData.IsLayoutAtMaxItemLimit();
		}

		public List<KeyValuePair<DecorationDefinition, int>> GetAvailableDecorations()
		{
			List<KeyValuePair<DecorationDefinition, int>> availableDecorations = decorationInventoryService.GetAvailableDecorations();
			if (IsObjectSelectedForAdd)
			{
				int selectedDefinitionId = getSelectedDefinitionId();
				for (int i = 0; i < availableDecorations.Count; i++)
				{
					KeyValuePair<DecorationDefinition, int> keyValuePair = availableDecorations[i];
					if (keyValuePair.Key.Id == selectedDefinitionId)
					{
						availableDecorations.RemoveAt(i);
						availableDecorations.Insert(i, new KeyValuePair<DecorationDefinition, int>(keyValuePair.Key, keyValuePair.Value - 1));
					}
				}
			}
			return availableDecorations;
		}

		public List<KeyValuePair<StructureDefinition, int>> GetAvailableStructures()
		{
			List<KeyValuePair<StructureDefinition, int>> availableStructures = decorationInventoryService.GetAvailableStructures();
			if (IsObjectSelectedForAdd)
			{
				int selectedDefinitionId = getSelectedDefinitionId();
				for (int i = 0; i < availableStructures.Count; i++)
				{
					KeyValuePair<StructureDefinition, int> keyValuePair = availableStructures[i];
					if (keyValuePair.Key.Id == selectedDefinitionId)
					{
						availableStructures.RemoveAt(i);
						availableStructures.Insert(i, new KeyValuePair<StructureDefinition, int>(keyValuePair.Key, keyValuePair.Value - 1));
					}
				}
			}
			return availableStructures;
		}

		public int GetAvailableDecorationCount(int id)
		{
			int num = decorationInventoryService.GetAvailableDecorationCount(id);
			if (IsObjectSelectedForAdd)
			{
				int selectedDefinitionId = getSelectedDefinitionId();
				if (selectedDefinitionId == id)
				{
					num--;
				}
			}
			return num;
		}

		public int GetAvailableStructureCount(int id)
		{
			int num = decorationInventoryService.GetAvailableStructureCount(id);
			if (IsObjectSelectedForAdd)
			{
				int selectedDefinitionId = getSelectedDefinitionId();
				if (selectedDefinitionId == id)
				{
					num--;
				}
			}
			return num;
		}

		private int getSelectedDefinitionId()
		{
			if (ObjectManipulationInputController.CurrentlySelectedObject != null)
			{
				ManipulatableObject component = ObjectManipulationInputController.CurrentlySelectedObject.GetComponent<ManipulatableObject>();
				if (component != null)
				{
					return component.DefinitionId;
				}
				throw new InvalidOperationException(string.Format("Could not find ManipulatableObject component on {0}", ObjectManipulationInputController.CurrentlySelectedObject.name));
			}
			return -1;
		}

		public void AddNewObject(PrefabContentKey prefabContentKey, Vector2 finalTouchPosition, DecorationLayoutData data, bool setManipulationInputStateToDrag = true)
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(finalTouchPosition.x, finalTouchPosition.y, 0f));
			AddNewObject(prefabContentKey, worldPosition, Quaternion.identity, Vector3.one, data, setManipulationInputStateToDrag);
		}

		public void AddNewObject(PrefabContentKey prefabContentKey, Vector3 worldPosition, Quaternion rotation, Vector3 scale, DecorationLayoutData data, bool setManipulationInputStateToDrag = true)
		{
			if (data.Type == DecorationLayoutData.DefinitionType.Decoration)
			{
				decorationInventoryService.MarkDecorationsDirty();
			}
			else if (data.Type == DecorationLayoutData.DefinitionType.Structure)
			{
				decorationInventoryService.MarkStructuresDirty();
			}
			prefabCacheTracker.Acquire(prefabContentKey, delegate(GameObject prefab, PrefabCacheTracker.PrefabRequest request)
			{
				onNewObjectLoaded(prefab, request, worldPosition, rotation, scale, data, setManipulationInputStateToDrag);
			});
		}

		private void onNewObjectLoaded(GameObject prefab, PrefabCacheTracker.PrefabRequest request, Vector3 worldPosition, Quaternion rotation, Vector3 scale, DecorationLayoutData data, bool setManipulationInputStateToDrag)
		{
			if (prefab == null)
			{
				Log.LogErrorFormatted(this, "Prefab was null");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, sceneLayoutContainer);
			if (gameObject == null)
			{
				Log.LogErrorFormatted(this, "Instantiate returned a null game object for prefab {0}", prefab);
				return;
			}
			isNewObject = true;
			prefabCacheTracker.SetCache(gameObject, request.ContentKey);
			gameObject.transform.position = worldPosition;
			gameObject.transform.rotation = rotation;
			gameObject.transform.localScale = scale;
			ManipulatableObject arg = addObjectManipulator(gameObject, data);
			ObjectManipulationInputController.SelectState setSelectStateTo = (!setManipulationInputStateToDrag) ? ObjectManipulationInputController.SelectState.Active : ObjectManipulationInputController.SelectState.Drag;
			if (ObjectManipulationInputController != null)
			{
				ObjectManipulationInputController.SelectNewObject(gameObject, setSelectStateTo);
			}
			SplittableObject component = gameObject.GetComponent<SplittableObject>();
			if (component != null)
			{
				if (SceneLayoutData.LayoutCount >= SceneLayoutData.MaxLayoutItems - 2)
				{
					UnityEngine.Object.Destroy(gameObject);
					ResetAfterMaxReached();
				}
				else
				{
					component.SetObjectManipulationController(ObjectManipulationInputController);
					component.ChildrenSplit += onSplittableObjectChildrenSplit;
				}
			}
			selectedObjectStartingId = null;
			this.NewObjectCreated.InvokeSafe(arg);
		}

		private void ResetAfterMaxReached()
		{
			ObjectManipulationInputController.Abort();
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.EndDragInventoryItem));
			Service.Get<EventDispatcher>().DispatchEvent(default(SceneLayoutEvents.SceneMaxItemsEvent));
		}

		private void onSplittableObjectChildrenSplit(SplittableObject splittableObject)
		{
			ManipulatableObject component = splittableObject.GetComponent<ManipulatableObject>();
			CollidableObject component2 = splittableObject.GetComponent<CollidableObject>();
			int nextAvailable = numberTracker.GetNextAvailable();
			GameObject[] splitList = splittableObject.SplitList;
			foreach (GameObject gameObject in splitList)
			{
				CollidableObject collidableObject = gameObject.AddComponent<CollidableObject>();
				collidableObject.CollisionRuleSet = component2.CollisionRuleSet;
				ManipulatableObject manipulatableObject = gameObject.AddComponent<ManipulatableObject>();
				gameObject.AddComponent<ManipulatableObjectEffects>();
				if (component != null)
				{
					manipulatableObject.DefinitionId = component.DefinitionId;
				}
				if (this.ObjectAdded != null)
				{
					this.ObjectAdded.InvokeSafe(manipulatableObject);
				}
				PartneredObject component3 = gameObject.GetComponent<PartneredObject>();
				if (component3 != null)
				{
					component3.SetOthers(splittableObject.SplitList);
					component3.SetNumber(nextAvailable);
				}
				UpdateLayoutForManipulatableObject(sceneLayoutContainer, manipulatableObject);
				manipulatableObject.SetParent(sceneLayoutContainer);
				manipulatableObject.OnRemoved += onObjectRemoved;
				manipulatableObject.BeforeParentChanged += onBeforeManipulatableObjectReParented;
				manipulatableObject.AfterParentChanged += onAfterManipulatableObjectReParented;
			}
			component.RemoveObject(false);
			splittableObject.ChildrenSplit -= onSplittableObjectChildrenSplit;
		}

		private bool onLayoutGameObjectsLoaded(SceneTransitionEvents.LayoutGameObjectsLoaded evt)
		{
			SceneLayoutData = dataEntityCollection.GetComponent<SceneLayoutData>(sceneHandle);
			sceneLayoutContainer = evt.Container;
			if (base.enabled)
			{
				ObjectManipulationInputController.Container = sceneLayoutContainer;
				addObjectManipulators();
			}
			return false;
		}

		public void ClearCurrentLayout()
		{
			if (ObjectManipulationInputController != null)
			{
				ObjectManipulationInputController.Reset();
			}
			int childCount = sceneLayoutContainer.childCount;
			for (int num = childCount - 1; num >= 0; num--)
			{
				ManipulatableObject component = sceneLayoutContainer.GetChild(num).GetComponent<ManipulatableObject>();
				if (component != null)
				{
					component.RemoveObject(true);
				}
			}
		}

		private void onObjectRemoved(GameObject obj, bool deleteChildren)
		{
			DecorationLayoutData decorationLayoutData = default(DecorationLayoutData);
			decorationLayoutData.Id = DecorationLayoutData.ID.FromFullPath(GetRelativeGameObjectPath(obj));
			DecorationLayoutData decoration = decorationLayoutData;
			SceneLayoutData.RemoveDecoration(decoration, deleteChildren);
			if (selectedObjectStartingId == decoration.Id.GetFullPath())
			{
				selectedObjectStartingId = null;
				ObjectManipulator component = obj.GetComponent<ObjectManipulator>();
				if (component != null)
				{
					for (int i = 0; i < sceneModifiers.Length; i++)
					{
						sceneModifiers[i].AfterObjectDeselected(component);
					}
				}
			}
			decorationInventoryService.MarkStructuresDirty();
			decorationInventoryService.MarkDecorationsDirty();
			if (this.ObjectRemoved != null)
			{
				ManipulatableObject component2 = obj.GetComponent<ManipulatableObject>();
				this.ObjectRemoved.InvokeSafe(component2);
			}
			UnityEngine.Object.Destroy(obj);
			removePartneredObject(obj);
		}

		private void removePartneredObject(GameObject obj)
		{
			PartneredObject component = obj.GetComponent<PartneredObject>();
			if (!(component != null))
			{
				return;
			}
			numberTracker.UnregisterNumber(component.Number);
			if (component.Other != null)
			{
				ManipulatableObject component2 = component.Other.gameObject.GetComponent<ManipulatableObject>();
				component.Other = null;
				if (component2 != null)
				{
					component2.RemoveObject(false);
				}
			}
		}

		private void onBeforeManipulatableObjectReParented(Transform newParent, GameObject obj)
		{
			ManipulatableObject component = obj.GetComponent<ManipulatableObject>();
			DecorationLayoutData decorationLayoutData = default(DecorationLayoutData);
			decorationLayoutData.Id = DecorationLayoutData.ID.FromFullPath(component.PathId);
			DecorationLayoutData decoration = decorationLayoutData;
			if (SceneLayoutData.ContainsKey(decoration.Id.GetFullPath()))
			{
				SceneLayoutData.RemoveDecoration(decoration, true);
			}
		}

		private void onAfterManipulatableObjectReParented(Transform newParent, GameObject obj)
		{
			ObjectManipulator.SetUniqueGameObjectName(obj, newParent);
			UpdateLayoutForManipulatableObject(newParent, obj.GetComponent<ManipulatableObject>());
			foreach (Transform item in obj.transform)
			{
				ManipulatableObject component = item.GetComponent<ManipulatableObject>();
				if (component != null)
				{
					component.SetParent(component.transform.parent);
				}
			}
		}

		private static void SetCustomPropertiesInDecoration(ref DecorationLayoutData data, PartneredObject po)
		{
			if (po != null)
			{
				Assert.IsNotNull(po.Other, "Other cannot be null");
				data.CustomProperties["partner"] = po.Other.GetGuid();
				data.CustomProperties["guid"] = po.GetGuid();
				Assert.IsTrue(po.Number > 0, "Number not set");
				data.CustomProperties["num"] = po.Number.ToString();
			}
		}

		private DecorationLayoutData UpdateLayoutForManipulatableObject(Transform newParent, ManipulatableObject mo)
		{
			string pathId = mo.PathId;
			if (newParent != mo.transform.parent)
			{
				ObjectManipulator.SetUniqueGameObjectName(mo.gameObject, newParent);
			}
			DecorationLayoutData decorationLayoutData = default(DecorationLayoutData);
			decorationLayoutData.Id.Name = mo.gameObject.name;
			decorationLayoutData.Id.ParentPath = GetRelativeGameObjectPath(newParent.gameObject);
			decorationLayoutData.DefinitionId = mo.DefinitionId;
			decorationLayoutData.Type = mo.Type;
			decorationLayoutData.Position = mo.gameObject.transform.localPosition;
			decorationLayoutData.Rotation = mo.gameObject.transform.localRotation;
			decorationLayoutData.UniformScale = mo.gameObject.transform.localScale.x;
			DecorationLayoutData data = decorationLayoutData;
			if (data.Id.ParentPath == "")
			{
				data.Position = mo.gameObject.transform.position;
				data.Rotation = mo.gameObject.transform.rotation;
				data.UniformScale = mo.gameObject.transform.lossyScale.x;
				data.Id.ParentPath = null;
			}
			SetCustomPropertiesInDecoration(ref data, mo.GetComponent<PartneredObject>());
			if (SceneLayoutData.ContainsKey(pathId))
			{
				SceneLayoutData.UpdateDecoration(pathId, data);
			}
			else
			{
				SceneLayoutData.AddDecoration(data);
			}
			return data;
		}

		private void onObjectBeforeDelete(ManipulatableObject mo)
		{
			ManipulatableObject[] componentsInChildren = mo.GetComponentsInChildren<ManipulatableObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].transform.parent == mo.transform)
				{
					componentsInChildren[i].SetParent(sceneLayoutContainer);
					Assert.IsTrue(componentsInChildren[i].transform.parent == sceneLayoutContainer);
				}
			}
		}

		private void OnObjectManipulatorChanged(ObjectManipulator objectManipulator)
		{
			ManipulatableObject[] componentsInChildren = objectManipulator.GetComponentsInChildren<ManipulatableObject>();
			foreach (ManipulatableObject manipulatableObject in componentsInChildren)
			{
				UpdateLayoutForManipulatableObject(manipulatableObject.transform.parent, manipulatableObject);
			}
		}

		private void onObjectedDeselected(ObjectManipulator obj)
		{
			ManipulatableObject component = obj.GetComponent<ManipulatableObject>();
			ManipulatableObjectEffects component2 = obj.gameObject.GetComponent<ManipulatableObjectEffects>();
			Rigidbody component3 = obj.gameObject.GetComponent<Rigidbody>();
			if (!obj.WasReparented)
			{
				DecorationLayoutData decorationLayoutData = default(DecorationLayoutData);
				decorationLayoutData.Id = DecorationLayoutData.ID.FromFullPath(GetRelativeGameObjectPath(obj.gameObject));
				decorationLayoutData.DefinitionId = component.DefinitionId;
				decorationLayoutData.Type = component.Type;
				decorationLayoutData.Position = obj.transform.localPosition;
				decorationLayoutData.Rotation = obj.transform.localRotation;
				decorationLayoutData.UniformScale = obj.transform.localScale.x;
				DecorationLayoutData data = decorationLayoutData;
				SetCustomPropertiesInDecoration(ref data, obj.GetComponent<PartneredObject>());
				if (selectedObjectStartingId == null)
				{
					SceneLayoutData.AddDecoration(data);
				}
				else
				{
					string relativeGameObjectPath = GetRelativeGameObjectPath(component.gameObject);
					SceneLayoutData.UpdateDecoration(relativeGameObjectPath, data);
				}
			}
			selectedObjectStartingId = null;
			if ((bool)component2)
			{
				component2.ClearObjectManipulator();
			}
			if ((bool)obj)
			{
				UnityEngine.Object.Destroy(obj);
			}
			if ((bool)component3)
			{
				UnityEngine.Object.Destroy(component3);
			}
			component.GetComponent<CollidableObject>().EnableTriggers(false);
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				sceneModifiers[i].AfterObjectDeselected(obj);
			}
			this.ObjectDeselected.InvokeSafe(component);
		}

		private void onObjectSelected(ManipulatableObject obj)
		{
			if (obj == null)
			{
				return;
			}
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				if (!sceneModifiers[i].CanObjectBeSelected(obj))
				{
					return;
				}
			}
			ObjectManipulator objectManipulator = obj.gameObject.AddComponentIfMissing<ObjectManipulator>();
			objectManipulationManager.WatchObject(objectManipulator);
			obj.GetComponent<CollidableObject>().EnableTriggers();
			obj.GetComponent<ManipulatableObjectEffects>().SetObjectManipulator(objectManipulator);
			Rigidbody rigidbody = obj.gameObject.AddComponentIfMissing<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				sceneModifiers[i].AfterObjectSelected(obj, isNewObject);
			}
			isNewObject = false;
			selectedObjectStartingId = GetRelativeGameObjectPath(obj.gameObject);
			ManipulatableObject[] componentsInChildren = obj.GetComponentsInChildren<ManipulatableObject>();
			foreach (ManipulatableObject manipulatableObject in componentsInChildren)
			{
				manipulatableObject.PathId = GetRelativeGameObjectPath(manipulatableObject.gameObject);
			}
		}

		internal void ProcessLayout()
		{
			modifyObjectManipulators(delegate(GameObject go, DecorationLayoutData data)
			{
				for (int j = 0; j < sceneModifiers.Length; j++)
				{
					sceneModifiers[j].ProcessObject(go);
				}
			});
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				sceneModifiers[i].OnLayoutProcessed();
			}
		}

		private void addObjectManipulators()
		{
			modifyObjectManipulators(delegate(GameObject gameObject, DecorationLayoutData decorationLayoutData)
			{
				addObjectManipulator(gameObject, decorationLayoutData);
			});
			ProcessLayout();
		}

		private ManipulatableObject addObjectManipulator(GameObject go, DecorationLayoutData data)
		{
			CollidableObject co = go.AddComponentIfMissing<CollidableObject>();
			ManipulatableObject manipulatableObject = go.AddComponentIfMissing<ManipulatableObject>();
			linkManipulatableObjectWithDefinition(manipulatableObject, co, data);
			manipulatableObject.OnRemoved += onObjectRemoved;
			manipulatableObject.BeforeParentChanged += onBeforeManipulatableObjectReParented;
			manipulatableObject.AfterParentChanged += onAfterManipulatableObjectReParented;
			if (this.ObjectAdded != null)
			{
				this.ObjectAdded.InvokeSafe(manipulatableObject);
			}
			go.AddComponentIfMissing<ManipulatableObjectEffects>();
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				sceneModifiers[i].ObjectAdded(data, go);
			}
			return manipulatableObject;
		}

		private void linkManipulatableObjectWithDefinition(ManipulatableObject mo, CollidableObject co, DecorationLayoutData data)
		{
			CollisionRuleSetDefinitionKey collisionRuleSet = null;
			switch (data.Type)
			{
			case DecorationLayoutData.DefinitionType.Decoration:
			{
				DecorationDefinition value2;
				if (decorationDefinitions.TryGetValue(data.DefinitionId, out value2))
				{
					collisionRuleSet = value2.RuleSet;
					mo.Type = DecorationLayoutData.DefinitionType.Decoration;
					mo.DefinitionId = value2.Id;
				}
				break;
			}
			case DecorationLayoutData.DefinitionType.Structure:
			{
				StructureDefinition value;
				if (structureDefinitions.TryGetValue(data.DefinitionId, out value))
				{
					collisionRuleSet = value.RuleSet;
					mo.Type = DecorationLayoutData.DefinitionType.Structure;
					mo.DefinitionId = value.Id;
				}
				break;
			}
			}
			co.CollisionRuleSet = collisionRuleSet;
		}

		private void removeObjectManipulators()
		{
			modifyObjectManipulators(delegate(GameObject go, DecorationLayoutData data)
			{
				ManipulatableObjectEffects component = go.GetComponent<ManipulatableObjectEffects>();
				if ((bool)component)
				{
					UnityEngine.Object.Destroy(component);
				}
				ManipulatableObject component2 = go.GetComponent<ManipulatableObject>();
				if ((bool)component2)
				{
					component2.OnRemoved -= onObjectRemoved;
					component2.BeforeParentChanged -= onBeforeManipulatableObjectReParented;
					component2.AfterParentChanged -= onAfterManipulatableObjectReParented;
					UnityEngine.Object.Destroy(component2);
				}
				CollidableObject component3 = go.GetComponent<CollidableObject>();
				if ((bool)component3)
				{
					UnityEngine.Object.Destroy(component3);
				}
				for (int i = 0; i < sceneModifiers.Length; i++)
				{
					sceneModifiers[i].ObjectRemoved(data, go);
				}
			});
		}

		private void modifyObjectManipulators(Action<GameObject, DecorationLayoutData> modify)
		{
			foreach (DecorationLayoutData item in SceneLayoutData.GetLayoutEnumerator())
			{
				Transform transform = sceneLayoutContainer;
				DecorationLayoutData.ID id = item.Id;
				Transform transform2 = transform.Find(id.GetFullPath());
				if (!(transform2 == null))
				{
					modify(transform2.gameObject, item);
				}
			}
		}
	}
}
