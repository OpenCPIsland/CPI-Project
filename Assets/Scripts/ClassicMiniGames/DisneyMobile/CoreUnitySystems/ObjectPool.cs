using DisneyMobile.CoreUnitySystems.PoolStrategies;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class ObjectPool<T> where T : class, new()
	{
		public delegate T AllocateObject();

		public delegate void ObjectAllocated(T allocatedObject, int count);

		public delegate void ObjectDeallocated(T allocatedObject);

		public delegate void ObjectUnspawned(T unspawnedObject);

		public delegate T ReplaceObject(List<T> activeInstances, Stack<T> inactiveInstances);

		protected int m_capacity = 8;

		private ObjectPoolFullStrategy m_fullStrategy;

		private Stack<T> m_inactiveInstances;

		private List<T> m_activeInstances;

		protected AllocateObject AllocateObjectDelegate;

		protected ObjectAllocated ObjectAllocatedDelegate;

		protected ObjectDeallocated ObjectDeallocatedDelegate;

		protected ObjectUnspawned ObjectUnspawnedDelegate;

		protected ReplaceObject ReplaceObjectDelegate;

		public int Capacity
		{
			get
			{
				return m_capacity;
			}
			set
			{
				m_capacity = value;
				int num = ActiveInstanceCount + m_inactiveInstances.Count;
				if (m_capacity > num)
				{
					AllocateObjects(m_capacity - num);
				}
				else if (m_capacity < num)
				{
					DeallocateObjects(num - m_capacity);
				}
			}
		}

		public int ActiveInstanceCount
		{
			get
			{
				int result = 0;
				if (m_activeInstances != null)
				{
					result = m_activeInstances.Count;
				}
				return result;
			}
		}

		public float Utilization
		{
			get
			{
				return (float)ActiveInstanceCount / (float)Capacity;
			}
		}

		public ObjectPoolFullStrategy FullStrategy
		{
			get
			{
				return m_fullStrategy;
			}
			set
			{
				m_fullStrategy = value;
			}
		}

		public ObjectPool(int initalCapacity, ObjectPoolFullStrategy fullStrategy, AllocateObject customAllocateObjectDelegate, ObjectAllocated customObjectAllocatedDelegate, ObjectDeallocated customObjectDeallocatedDelegate, ObjectUnspawned customObjectUnspawnedDelegate, ReplaceObject customReplaceObjectDelegate)
		{
			AllocateObjectDelegate = OnAllocateObject;
			if (customAllocateObjectDelegate != null)
			{
				AllocateObjectDelegate = customAllocateObjectDelegate;
			}
			ObjectAllocatedDelegate = null;
			if (customObjectAllocatedDelegate != null)
			{
				ObjectAllocatedDelegate = customObjectAllocatedDelegate;
			}
			ObjectDeallocatedDelegate = null;
			if (customObjectDeallocatedDelegate != null)
			{
				ObjectDeallocatedDelegate = customObjectDeallocatedDelegate;
			}
			ObjectUnspawnedDelegate = null;
			if (customObjectUnspawnedDelegate != null)
			{
				ObjectUnspawnedDelegate = customObjectUnspawnedDelegate;
			}
			ReplaceObjectDelegate = OnReplaceObject;
			if (customReplaceObjectDelegate != null)
			{
				ReplaceObjectDelegate = customReplaceObjectDelegate;
			}
			FullStrategy = fullStrategy;
			m_inactiveInstances = new Stack<T>(initalCapacity);
			m_activeInstances = new List<T>(initalCapacity);
			AllocateObjects(initalCapacity);
			if (FullStrategy == null)
			{
				FullStrategy = ScriptableObject.CreateInstance<ReplaceOldest>();
			}
		}

		public ObjectPool(int initalCapacity, ObjectPoolFullStrategy fullStrategy)
			: this(initalCapacity, fullStrategy, (AllocateObject)null, (ObjectAllocated)null, (ObjectDeallocated)null, (ObjectUnspawned)null, (ReplaceObject)null)
		{
		}

		private ObjectPool()
		{
		}

		public T Spawn()
		{
			T val = null;
			if (m_inactiveInstances.Count > 0)
			{
				val = m_inactiveInstances.Pop();
				m_activeInstances.Add(val);
			}
			else if (m_fullStrategy != null)
			{
				if (ReplaceObjectDelegate != null)
				{
					val = ReplaceObjectDelegate(m_activeInstances, m_inactiveInstances);
				}
				else
				{
					Logger.LogFatal(this, "Pool has exceeded its active instances and there is no replacement object delegate.", Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
				}
			}
			else
			{
				Logger.LogFatal(this, "Pool has exceeded its active instances and there is no full strategy specified.", Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
			}
			return val;
		}

		public void Unspawn(T spawnedObject)
		{
			m_activeInstances.Remove(spawnedObject);
			m_inactiveInstances.Push(spawnedObject);
			if (ObjectUnspawnedDelegate != null)
			{
				ObjectUnspawnedDelegate(spawnedObject);
			}
		}

		public void UnspawnAllObjects()
		{
			for (int i = 0; i < m_activeInstances.Count; i++)
			{
				T val = m_activeInstances[i];
				m_inactiveInstances.Push(val);
				if (ObjectUnspawnedDelegate != null)
				{
					ObjectUnspawnedDelegate(val);
				}
			}
			m_activeInstances.Clear();
		}

		protected void AllocateObjects(int amountOfObjectsToAllocate)
		{
			for (int i = 0; i < amountOfObjectsToAllocate; i++)
			{
				T val = AllocateObjectDelegate();
				if (val != null)
				{
					m_inactiveInstances.Push(val);
					if (ObjectAllocatedDelegate != null)
					{
						ObjectAllocatedDelegate(val, m_inactiveInstances.Count);
					}
				}
				else
				{
					Logger.LogFatal(this, "Pool could not allocate object for pooling.", Logger.TagFlags.CORE | Logger.TagFlags.MEMORY);
				}
			}
			Capacity = ActiveInstanceCount + m_inactiveInstances.Count;
		}

		private T OnAllocateObject()
		{
			return new T();
		}

		private T OnReplaceObject(List<T> activeInstances, Stack<T> inactiveInstances)
		{
			return m_fullStrategy.GetObjectToReplace(this, activeInstances, inactiveInstances);
		}

		protected void DeallocateObjects(int amountOfObjectsToDeallocate)
		{
			int i = 0;
			int val = ActiveInstanceCount + m_inactiveInstances.Count;
			for (int num = Math.Min(amountOfObjectsToDeallocate, val); i < num; i++)
			{
				DeallocateObject();
			}
		}

		protected void DeallocateObject()
		{
			T val = null;
			if (m_inactiveInstances.Count > 0)
			{
				val = m_inactiveInstances.Pop();
			}
			if (val == null && m_activeInstances.Count > 0)
			{
				int index = ActiveInstanceCount - 1;
				val = m_activeInstances[index];
				m_activeInstances.RemoveAt(index);
			}
			if (val != null && ObjectDeallocatedDelegate != null)
			{
				ObjectDeallocatedDelegate(val);
			}
		}
	}
}
