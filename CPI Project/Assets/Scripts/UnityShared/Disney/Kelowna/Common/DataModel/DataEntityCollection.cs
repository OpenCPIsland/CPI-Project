using Disney.LaunchPadFramework;
using System;

namespace Disney.Kelowna.Common.DataModel
{
	public interface DataEntityCollection
	{
		ReadonlyEventDispatcher EventDispatcher
		{
			get;
		}

		DataEventListener When<T>(string entityName, Action<T> onAdded) where T : BaseData;

		DataEventListener When<T>(DataEntityHandle handle, Action<T> onAdded) where T : BaseData;

		DataEventListener Whenever<T>(string entityName, Action<T> onAdded, Action<T> onRemoved) where T : BaseData;

		DataEventListener Whenever<T>(DataEntityHandle handle, Action<T> onAdded, Action<T> onRemoved) where T : BaseData;

		DataEntityHandle AddEntity(string entityName);

		DataEntityHandle FindEntity<T, F>(F identifier) where T : BaseData, IEntityIdentifierData<F>;

		bool TryFindEntity<T, F>(F identifier, out DataEntityHandle dataEntityHandle) where T : BaseData, IEntityIdentifierData<F>;

		DataEntityHandle FindEntityByName(string entityName);

		bool ContainsEntityByName(string entityName);

		DataEntityHandle[] GetEntitiesByType<T>() where T : BaseData;

		DataEntityHandle GetEntityByType<T>() where T : BaseData;

		DataEntityHandle GetEntityByComponent(BaseData component);

		bool RemoveEntityByName(string entityName);

		T AddComponent<T>(DataEntityHandle handle, Action<T> initFunction = null) where T : BaseData;

		void AddComponent<T>(DataEntityHandle handle, T component) where T : BaseData;

		void AddComponentIfMissing<T>(DataEntityHandle handle) where T : BaseData;

		bool HasComponent<T>(DataEntityHandle handle) where T : BaseData;

		T GetComponent<T>(DataEntityHandle handle) where T : BaseData;

		bool TryGetComponent<T>(DataEntityHandle handle, out T component) where T : BaseData;

		bool RemoveComponent<T>(DataEntityHandle handle) where T : BaseData;

		void RemoveEntityScopedComponents(DataEntityHandle handle, string[] scopeIDs);

		void RemoveAllComponents(DataEntityHandle handle);

        T GetComponent2<T>(DataEntityHandle2 handle) where T : BaseData;
    }
}
