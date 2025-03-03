using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.PartyGames
{
	public abstract class AbstractMmoItemObserver<T> : MonoBehaviour where T : CPMMOItem
	{
		private CPDataEntityCollection dataEntityCollection;

		private Dictionary<DataEntityHandle, ServerObjectItemData> handleToServerObject;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			handleToServerObject = new Dictionary<DataEntityHandle, ServerObjectItemData>();
			awake();
		}

		private void Start()
		{
			addListeners();
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<ServerObjectItemData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				onMmoItemAdded(handle, dataEntityCollection.GetComponent<ServerObjectItemData>(handle));
			}
			start();
		}

		private void OnDestroy()
		{
			removeListeners();
			onDestroy();
		}

		protected abstract void onItemRemoved(T item);

		protected abstract void onItemAdded(T item);

		protected abstract void onItemUpdated(T item);

		private void addListeners()
		{
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemDataAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
		}

		private void removeListeners()
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemDataAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onServerObjectRemoved);
		}

		private bool onServerObjectItemDataAdded(DataEntityEvents.ComponentAddedEvent<ServerObjectItemData> evt)
		{
			onMmoItemAdded(evt.Handle, evt.Component);
			return false;
		}

		private void onMmoItemAdded(DataEntityHandle handle, ServerObjectItemData itemData)
		{
			T val = itemData.Item as T;
			if (val != null)
			{
				itemData.ItemChanged += onMmoItemUpdated;
				handleToServerObject.Add(handle, itemData);
				onItemAdded(val);
			}
		}

		private void onMmoItemUpdated(CPMMOItem mmoItem)
		{
			if (mmoItem is T)
			{
				onItemUpdated(mmoItem as T);
			}
		}

		private bool onServerObjectRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (handleToServerObject.ContainsKey(evt.EntityHandle))
			{
				removeMmoItem(evt.EntityHandle);
			}
			return false;
		}

		private void removeMmoItem(DataEntityHandle handle)
		{
			T item = handleToServerObject[handle].Item as T;
			handleToServerObject[handle].ItemChanged -= onMmoItemUpdated;
			handleToServerObject.Remove(handle);
			onItemRemoved(item);
		}

		protected virtual void start()
		{
		}

		protected virtual void awake()
		{
		}

		protected virtual void onDestroy()
		{
		}
	}
}
