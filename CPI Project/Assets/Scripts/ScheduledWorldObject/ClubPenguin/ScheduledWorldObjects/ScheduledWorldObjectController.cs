using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ScheduledWorldObjects
{
	internal class ScheduledWorldObjectController : MonoBehaviour
	{
		private HashSet<string> childrenPaths = new HashSet<string>();

		private Dictionary<DataEntityHandle, ServerObjectItemData> dataModelHandleListenersMap;

		private Dictionary<long, Transform> gameObjectIdToTransformMap;

		private DataEntityCollection dataEntityCollection;

		public void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			childrenPaths = new HashSet<string>(base.gameObject.GetChildrenPaths());
			dataModelHandleListenersMap = new Dictionary<DataEntityHandle, ServerObjectItemData>();
			gameObjectIdToTransformMap = new Dictionary<long, Transform>();
		}

		public void Start()
		{
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<ServerObjectItemData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				onServerObjectItemAdded(handle, dataEntityCollection.GetComponent<ServerObjectItemData>(handle));
			}
		}

		public void OnDestroy()
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
		}

		private bool onServerObjectItemAdded(DataEntityEvents.ComponentAddedEvent<ServerObjectItemData> evt)
		{
			onServerObjectItemAdded(evt.Handle, evt.Component);
			return false;
		}

		private void onServerObjectItemAdded(DataEntityHandle handle, ServerObjectItemData obj)
		{
			StatefulWorldObject statefulWorldObject = obj.Item as StatefulWorldObject;
			if (statefulWorldObject == null)
			{
				return;
			}
			if (!dataModelHandleListenersMap.ContainsKey(handle) && childrenPaths.Contains(statefulWorldObject.Path))
			{
				Transform transform = base.gameObject.transform.Find(statefulWorldObject.Path);
				if (transform != null)
				{
					dataModelHandleListenersMap[handle] = obj;
					obj.ItemChanged += onServerObjectChanged;
					gameObjectIdToTransformMap[statefulWorldObject.Id.Id] = transform;
				}
			}
			if (dataModelHandleListenersMap.ContainsKey(handle))
			{
				UpdateWorldObjectState(statefulWorldObject);
			}
		}

		private bool onServerObjectRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (dataModelHandleListenersMap.ContainsKey(evt.EntityHandle))
			{
				ServerObjectItemData serverObjectItemData = dataModelHandleListenersMap[evt.EntityHandle];
				serverObjectItemData.ItemChanged -= onServerObjectChanged;
				gameObjectIdToTransformMap.Remove(serverObjectItemData.Item.Id.Id);
				dataModelHandleListenersMap.Remove(evt.EntityHandle);
			}
			return false;
		}

		private void onServerObjectChanged(CPMMOItem obj)
		{
			UpdateWorldObjectState(obj as StatefulWorldObject);
		}

		private void UpdateWorldObjectState(StatefulWorldObject statefulWorldObject)
		{
			if (statefulWorldObject == null || !gameObjectIdToTransformMap.ContainsKey(statefulWorldObject.Id.Id))
			{
				return;
			}
			Transform transform = gameObjectIdToTransformMap[statefulWorldObject.Id.Id];
			if (!(transform != null))
			{
				return;
			}
			switch (statefulWorldObject.State)
			{
			case ScheduledWorldObjectState.Active:
			{
				StatefulWorldObjectMonobehaviour statefulWorldObjectMonobehaviour = transform.gameObject.GetComponent<StatefulWorldObjectMonobehaviour>();
				if (statefulWorldObjectMonobehaviour == null)
				{
					statefulWorldObjectMonobehaviour = transform.gameObject.AddComponent<StatefulWorldObjectMonobehaviour>();
				}
				statefulWorldObjectMonobehaviour.StatefulWorldObject = statefulWorldObject;
				transform.gameObject.SetActive(true);
				break;
			}
			case ScheduledWorldObjectState.Inactive:
				transform.gameObject.SetActive(false);
				break;
			}
		}
	}
}
