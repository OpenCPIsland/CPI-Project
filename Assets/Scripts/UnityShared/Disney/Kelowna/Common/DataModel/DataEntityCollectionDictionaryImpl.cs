using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;

namespace Disney.Kelowna.Common.DataModel
{
	public class DataEntityCollectionDictionaryImpl : DataEntityCollection
	{
		private Dictionary<DataEntityHandle, Dictionary<Type, BaseData>> entities;

        private Dictionary<DataEntityHandle2, Dictionary<Type, BaseData>> entities2;

        private DataEventListenerCollection listenerCollection;

		private EventDispatcher eventDispatcher = new EventDispatcher();

		private ReadonlyEventDispatcher readOnlyEventDispatcher;

		public ReadonlyEventDispatcher EventDispatcher
		{
			get
			{
				return readOnlyEventDispatcher;
			}
		}

		public DataEntityCollectionDictionaryImpl()
		{
			readOnlyEventDispatcher = new ReadonlyEventDispatcher(eventDispatcher);
			entities = new Dictionary<DataEntityHandle, Dictionary<Type, BaseData>>();
			listenerCollection = new DataEventListenerCollection(this);
		}

		public T AddComponent<T>(DataEntityHandle handle, Action<T> initFunction = null) where T : BaseData
		{
			T val = (T)Activator.CreateInstance(typeof(T));
			if (initFunction != null)
			{
				initFunction(val);
			}
			AddComponent(handle, val);
			return val;
		}

		public void AddComponent<T>(DataEntityHandle handle, T component) where T : BaseData
		{
			validateHandle(handle, "AddComponent");
			Type type = component.GetType();
			Type[] typeArguments = new Type[1]
			{
				type
			};
			entities[handle][type] = component;
			if (type != null)
			{
				Type typeFromHandle = typeof(DataEntityEvents.ComponentAddedEvent<>);
				Type type2 = typeFromHandle.MakeGenericType(typeArguments);
				object[] parameters = new object[1]
				{
					Activator.CreateInstance(type2, handle, component)
				};
				eventDispatcher.GetType().GetMethod("DispatchEvent").MakeGenericMethod(type2)
					.Invoke(eventDispatcher, parameters);
			}
			else
			{
				eventDispatcher.DispatchEvent(new DataEntityEvents.ComponentAddedEvent<T>(handle, component));
			}
			listenerCollection.ComponentAdded(handle.Id, component);
		}

		public void AddComponentIfMissing<T>(DataEntityHandle handle) where T : BaseData
		{
			if (!HasComponent<T>(handle))
			{
				AddComponent<T>(handle);
			}
		}

		public DataEntityHandle AddEntity(string entityName)
		{
			if (string.IsNullOrEmpty(entityName))
			{
				throw new ArgumentNullException("name", "AddEntity requires a name that is not null or empty");
			}
            UnityEngine.Debug.Log("Entity Is: "+ entityName);
			DataEntityHandle dataEntityHandle = new DataEntityHandle(entityName);
			if (entities.ContainsKey(dataEntityHandle))
			{
				throw new ArgumentException("An entity with the name '" + entityName + "' already exists.");
			}
			entities.Add(dataEntityHandle, new Dictionary<Type, BaseData>());
			eventDispatcher.DispatchEvent(new DataEntityEvents.EntityAddedEvent(dataEntityHandle));
			return dataEntityHandle;
		}

		public bool ContainsEntityByName(string entityName)
		{
			return !FindEntityByName(entityName).IsNull;
		}

		public DataEntityHandle FindEntity<T, F>(F identifier) where T : BaseData, IEntityIdentifierData<F>
		{
			foreach (KeyValuePair<DataEntityHandle, Dictionary<Type, BaseData>> entity in entities)
			{
				if (entity.Value.ContainsKey(typeof(T)))
				{
					T val = (T)entity.Value[typeof(T)];
					if (val.Match(identifier))
					{
						return entity.Key;
					}
				}
			}
			return DataEntityHandle.NullHandle;
		}

		public DataEntityHandle FindEntityByName(string entityName)
		{
			foreach (DataEntityHandle key in entities.Keys)
			{
				if (key.Id == entityName)
				{
					return key;
				}
			}
			return DataEntityHandle.NullHandle;
		}

		public T GetComponent<T>(DataEntityHandle handle) where T : BaseData
		{
			validateHandle(handle, "GetComponent");
			if (entities[handle].ContainsKey(typeof(T)))
			{
				return (T)entities[handle][typeof(T)];
			}
			return null;
		}

        public T GetComponent2<T>(DataEntityHandle2 handle) where T : BaseData
        {
            validateHandle2(handle, "GetComponent");
            if (entities2[handle].ContainsKey(typeof(T)))
            {
                return (T)entities2[handle][typeof(T)];
            }
            return null;
        }

        public DataEntityHandle[] GetEntitiesByType<T>() where T : BaseData
		{
			List<DataEntityHandle> list = new List<DataEntityHandle>();
			foreach (KeyValuePair<DataEntityHandle, Dictionary<Type, BaseData>> entity in entities)
			{
				if (entity.Value.ContainsKey(typeof(T)))
				{
					list.Add(entity.Key);
				}
			}
			return list.ToArray();
		}

		public DataEntityHandle GetEntityByComponent(BaseData component)
		{
			Type type = component.GetType();
			foreach (KeyValuePair<DataEntityHandle, Dictionary<Type, BaseData>> entity in entities)
			{
				if (entity.Value.ContainsKey(type) && object.ReferenceEquals(entity.Value[type], component))
				{
					return entity.Key;
				}
			}
			return DataEntityHandle.NullHandle;
		}

		public DataEntityHandle GetEntityByType<T>() where T : BaseData
		{
			foreach (KeyValuePair<DataEntityHandle, Dictionary<Type, BaseData>> entity in entities)
			{
				if (entity.Value.ContainsKey(typeof(T)))
				{
					return entity.Key;
				}
			}
			return DataEntityHandle.NullHandle;
		}

		public bool HasComponent<T>(DataEntityHandle handle) where T : BaseData
		{
			validateHandle(handle, "HasComponent");
			return entities[handle].ContainsKey(typeof(T));
		}

		public void RemoveAllComponents(DataEntityHandle handle)
		{
			if (!handle.IsNull && entities.ContainsKey(handle))
			{
				foreach (BaseData value in entities[handle].Values)
				{
					value.NotifyWillBeDestroyed();
					eventDispatcher.DispatchEvent(new DataEntityEvents.ComponentRemovedEvent(handle, value));
					listenerCollection.GetType().GetMethod("ComponentRemoved").MakeGenericMethod(value.GetType())
						.Invoke(listenerCollection, new object[2]
						{
							handle.Id,
							value
						});
				}
				eventDispatcher.DispatchEvent(new DataEntityEvents.EntityRemovedEvent(handle));
				entities.Remove(handle);
				handle.Id = null;
			}
		}

		public bool RemoveComponent<T>(DataEntityHandle handle) where T : BaseData
		{
			if (!handle.IsNull && entities.ContainsKey(handle) && entities[handle].ContainsKey(typeof(T)))
			{
				if (typeof(ScopedData).IsAssignableFrom(typeof(T)) && !containsOtherScopedDataComponents(entities[handle], typeof(T)))
				{
					RemoveAllComponents(handle);
				}
				else
				{
					T val = (T)entities[handle][typeof(T)];
					val.NotifyWillBeDestroyed();
					eventDispatcher.DispatchEvent(new DataEntityEvents.ComponentRemovedEvent(handle, val));
					listenerCollection.ComponentRemoved(handle.Id, val);
					entities[handle].Remove(typeof(T));
				}
				return true;
			}
			return false;
		}

		public bool RemoveEntityByName(string entityName)
		{
			DataEntityHandle dataEntityHandle = FindEntityByName(entityName);
			if (dataEntityHandle.IsNull)
			{
				return false;
			}
			RemoveAllComponents(dataEntityHandle);
			return dataEntityHandle.IsNull;
		}

		public void RemoveEntityScopedComponents(DataEntityHandle handle, string[] scopeIDs)
		{
			if (!handle.IsNull && entities.ContainsKey(handle))
			{
				bool flag = false;
				List<Type> list = new List<Type>();
				List<BaseData> list2 = new List<BaseData>();
				foreach (KeyValuePair<Type, BaseData> item in entities[handle])
				{
					if (typeof(ScopedData).IsAssignableFrom(item.Key))
					{
						bool flag2 = false;
						for (int i = 0; i < scopeIDs.Length; i++)
						{
							if (((ScopedData)item.Value).ScopeID == scopeIDs[i])
							{
								list.Add(item.Key);
								list2.Add(item.Value);
								flag2 = true;
							}
						}
						flag = (flag || !flag2);
					}
				}
				int count = list2.Count;
				for (int j = 0; j < count; j++)
				{
					list2[j].NotifyWillBeDestroyed();
					eventDispatcher.DispatchEvent(new DataEntityEvents.ComponentRemovedEvent(handle, list2[j]));
					listenerCollection.GetType().GetMethod("ComponentRemoved").MakeGenericMethod(list2[j].GetType())
						.Invoke(listenerCollection, new object[2]
						{
							handle.Id,
							list2[j]
						});
				}
				count = list.Count;
				for (int j = 0; j < count; j++)
				{
					entities[handle].Remove(list[j]);
				}
				if (!flag)
				{
					RemoveAllComponents(handle);
				}
			}
		}

		public bool TryFindEntity<T, F>(F identifier, out DataEntityHandle dataEntityHandle) where T : BaseData, IEntityIdentifierData<F>
		{
			dataEntityHandle = FindEntity<T, F>(identifier);
			return !dataEntityHandle.IsNull;
		}

		public bool TryGetComponent<T>(DataEntityHandle handle, out T component) where T : BaseData
		{
			if (!DataEntityHandle.IsNullValue(handle) && entities.ContainsKey(handle) && entities[handle].ContainsKey(typeof(T)))
			{
				component = (T)entities[handle][typeof(T)];
				return true;
			}
			component = null;
			return false;
		}

		protected void EndScopes(string[] scopeIDs)
		{
			List<DataEntityHandle> entitiesByInheritedType = getEntitiesByInheritedType<ScopedData>();
			for (int i = 0; i < entitiesByInheritedType.Count; i++)
			{
				RemoveEntityScopedComponents(entitiesByInheritedType[i], scopeIDs);
			}
		}

		private List<DataEntityHandle> getEntitiesByInheritedType<T>()
		{
			List<DataEntityHandle> list = new List<DataEntityHandle>();
			foreach (KeyValuePair<DataEntityHandle, Dictionary<Type, BaseData>> entity in entities)
			{
				foreach (Type key in entity.Value.Keys)
				{
					if (typeof(T).IsAssignableFrom(key))
					{
						list.Add(entity.Key);
						break;
					}
				}
			}
			return list;
		}

		private bool containsOtherScopedDataComponents(Dictionary<Type, BaseData> componentsByType, Type type)
		{
			foreach (Type key in componentsByType.Keys)
			{
				if (typeof(ScopedData).IsAssignableFrom(key) && key != type)
				{
					return true;
				}
			}
			return false;
		}

		private void validateHandle(DataEntityHandle handle, string method)
		{
			if (handle.IsNull)
			{
				throw new ArgumentException("Called " + method + " with a null entity", "handle");
			}
			if (!entities.ContainsKey(handle))
			{
				throw new ArgumentException("Called " + method + " with an unknown entity", "handle");
			}
		}

        private void validateHandle2(DataEntityHandle2 handle, string method)
        {
        }

        public DataEventListener When<T>(string entityName, Action<T> onAdded) where T : BaseData
		{
			return listenerCollection.When(entityName, onAdded);
		}

		public DataEventListener When<T>(DataEntityHandle handle, Action<T> onAdded) where T : BaseData
		{
			return When(handle.Id, onAdded);
		}

		public DataEventListener Whenever<T>(string entityName, Action<T> onAdded, Action<T> onRemoved) where T : BaseData
		{
			return listenerCollection.Whenever(entityName, onAdded, onRemoved);
		}

		public DataEventListener Whenever<T>(DataEntityHandle handle, Action<T> onAdded, Action<T> onRemoved) where T : BaseData
		{
			return Whenever(handle.Id, onAdded, onRemoved);
		}
	}
}
