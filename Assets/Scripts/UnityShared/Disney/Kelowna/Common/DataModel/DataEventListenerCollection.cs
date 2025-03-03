using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common.DataModel
{
	internal class DataEventListenerCollection
	{
		private class DataEventListenerImpl : DataEventListener
		{
			public DataEventListenerCollection ListenerCollection;

			public ListenerKey Key;

			public ComponentAddedHandler AddedHandler;

			public ComponentRemovedHandler RemovedHandler;

			public void StopListening()
			{
				if (AddedHandler != null)
				{
					ListenerCollection.deleteComponentAddedHandler(Key, AddedHandler);
				}
				if (RemovedHandler != null)
				{
					ListenerCollection.deleteComponentRemovedHandler(Key, RemovedHandler);
				}
			}
		}

		private struct ListenerKey
		{
			public string EntityName;

			public Type Type;

			public override int GetHashCode()
			{
				int num = -2128831035;
				num = ((num * 16777619) ^ EntityName.GetHashCode());
				return (num * 16777619) ^ Type.GetHashCode();
			}
		}

		private class ComponentAddedHandler
		{
			public bool onlyOnce;
		}

		private class ComponentRemovedHandler
		{
		}

		private class ComponentAddedHandler<T> : ComponentAddedHandler where T : BaseData
		{
			public Action<T> Added;
		}

		private class ComponentRemovedHandler<T> : ComponentRemovedHandler where T : BaseData
		{
			public Action<T> Removed;
		}

		private DataEntityCollectionDictionaryImpl dataEntityCollection;

		private Dictionary<ListenerKey, List<ComponentAddedHandler>> componentAddedHandlers;

		private Dictionary<ListenerKey, List<ComponentRemovedHandler>> componentRemovedHandlers;

		internal DataEventListenerCollection(DataEntityCollectionDictionaryImpl dataEntityCollection)
		{
			this.dataEntityCollection = dataEntityCollection;
			componentAddedHandlers = new Dictionary<ListenerKey, List<ComponentAddedHandler>>();
			componentRemovedHandlers = new Dictionary<ListenerKey, List<ComponentRemovedHandler>>();
		}

		internal DataEventListener When<T>(string entityName, Action<T> onAdded) where T : BaseData
		{
			DataEventListenerImpl dataEventListenerImpl = new DataEventListenerImpl();
			dataEventListenerImpl.ListenerCollection = this;
			dataEventListenerImpl.Key.EntityName = entityName;
			dataEventListenerImpl.Key.Type = typeof(T);
			DataEventListenerImpl dataEventListenerImpl2 = dataEventListenerImpl;
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName(entityName);
			if (!dataEntityHandle.IsNull && dataEntityCollection.HasComponent<T>(dataEntityHandle))
			{
				onAdded(dataEntityCollection.GetComponent<T>(dataEntityHandle));
			}
			else
			{
				ListenerKey listenerKey = default(ListenerKey);
				listenerKey.EntityName = entityName;
				listenerKey.Type = typeof(T);
				ListenerKey key = listenerKey;
				dataEventListenerImpl2.AddedHandler = createComponentAddedHandler(key, onAdded, true);
			}
			return dataEventListenerImpl2;
		}

		internal DataEventListener Whenever<T>(string entityName, Action<T> onAdded, Action<T> onRemoved) where T : BaseData
		{
			DataEventListenerImpl dataEventListenerImpl = (DataEventListenerImpl)When(entityName, onAdded);
			ListenerKey listenerKey = default(ListenerKey);
			listenerKey.EntityName = entityName;
			listenerKey.Type = typeof(T);
			ListenerKey key = listenerKey;
			if (dataEventListenerImpl.AddedHandler == null)
			{
				dataEventListenerImpl.AddedHandler = createComponentAddedHandler(key, onAdded, false);
			}
			else
			{
				dataEventListenerImpl.AddedHandler.onlyOnce = false;
			}
			dataEventListenerImpl.RemovedHandler = createComponentRemovedHandler(key, onRemoved);
			return dataEventListenerImpl;
		}

		internal void ComponentAdded<T>(string entityName, T component) where T : BaseData
		{
			ListenerKey listenerKey = default(ListenerKey);
			listenerKey.EntityName = entityName;
			listenerKey.Type = typeof(T);
			ListenerKey key = listenerKey;
			if (!componentAddedHandlers.ContainsKey(key))
			{
				return;
			}
			List<ComponentAddedHandler> list = componentAddedHandlers[key];
			Stack<int> stack = new Stack<int>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				ComponentAddedHandler<T> componentAddedHandler = (ComponentAddedHandler<T>)list[i];
				componentAddedHandler.Added.InvokeSafe(component);
				if (componentAddedHandler.onlyOnce)
				{
					stack.Push(i);
				}
			}
			if (stack.Count > 0)
			{
				foreach (int item in stack)
				{
					list.RemoveAt(item);
				}
				if (list.Count == 0)
				{
					componentAddedHandlers.Remove(key);
				}
			}
		}

		public void ComponentRemoved<T>(string entityName, T component) where T : BaseData
		{
			ListenerKey listenerKey = default(ListenerKey);
			listenerKey.EntityName = entityName;
			listenerKey.Type = typeof(T);
			ListenerKey key = listenerKey;
			if (componentRemovedHandlers.ContainsKey(key))
			{
				List<ComponentRemovedHandler> list = componentRemovedHandlers[key];
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					ComponentRemovedHandler<T> componentRemovedHandler = (ComponentRemovedHandler<T>)list[i];
					componentRemovedHandler.Removed.InvokeSafe(component);
				}
			}
		}

		private ComponentAddedHandler createComponentAddedHandler<T>(ListenerKey key, Action<T> onAdded, bool onlyOnce) where T : BaseData
		{
			if (!componentAddedHandlers.ContainsKey(key))
			{
				componentAddedHandlers.Add(key, new List<ComponentAddedHandler>());
			}
			ComponentAddedHandler<T> componentAddedHandler = new ComponentAddedHandler<T>();
			componentAddedHandler.Added = onAdded;
			componentAddedHandler.onlyOnce = onlyOnce;
			ComponentAddedHandler<T> componentAddedHandler2 = componentAddedHandler;
			componentAddedHandlers[key].Add(componentAddedHandler2);
			return componentAddedHandler2;
		}

		private void deleteComponentAddedHandler(ListenerKey key, ComponentAddedHandler handler)
		{
			if (componentAddedHandlers.ContainsKey(key))
			{
				componentAddedHandlers[key].Remove(handler);
				if (componentAddedHandlers[key].Count == 0)
				{
					componentAddedHandlers.Remove(key);
				}
			}
		}

		private ComponentRemovedHandler createComponentRemovedHandler<T>(ListenerKey key, Action<T> onRemoved) where T : BaseData
		{
			if (!componentRemovedHandlers.ContainsKey(key))
			{
				componentRemovedHandlers.Add(key, new List<ComponentRemovedHandler>());
			}
			ComponentRemovedHandler<T> componentRemovedHandler = new ComponentRemovedHandler<T>();
			componentRemovedHandler.Removed = onRemoved;
			ComponentRemovedHandler<T> componentRemovedHandler2 = componentRemovedHandler;
			componentRemovedHandlers[key].Add(componentRemovedHandler2);
			return componentRemovedHandler2;
		}

		private void deleteComponentRemovedHandler(ListenerKey key, ComponentRemovedHandler handler)
		{
			if (componentRemovedHandlers.ContainsKey(key))
			{
				componentRemovedHandlers[key].Remove(handler);
				if (componentRemovedHandlers[key].Count == 0)
				{
					componentRemovedHandlers.Remove(key);
				}
			}
		}
	}
}
