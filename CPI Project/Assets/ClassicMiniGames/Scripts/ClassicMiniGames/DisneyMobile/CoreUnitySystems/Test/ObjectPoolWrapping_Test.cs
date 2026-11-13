using DisneyMobile.CoreUnitySystems.PoolStrategies;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class ObjectPoolWrapping_Test
	{
		private class PoolableObject
		{
			public static int InstanceCount = 0;

			public PoolableObject()
			{
				InstanceCount++;
			}
		}

		private class ObjectPoolWrapper<T> where T : class, new()
		{
			public bool CalledAllocateObject = false;

			public bool CalledObjectAllocated = false;

			public bool CalledObjectDeallocated = false;

			public bool CalledObjectUnspawned = false;

			public bool CalledReplaceObject = false;

			public ObjectPool<T> m_objectPool = null;

			public ObjectPoolWrapper()
			{
				m_objectPool = new ObjectPool<T>(InitialPoolSize, ScriptableObject.CreateInstance<DropObject>(), OnAllocateObject, OnObjectAllocated, OnObjectDeallocated, OnObjectUnspawned, OnReplaceObject);
			}

			public T OnAllocateObject()
			{
				CalledAllocateObject = true;
				return new T();
			}

			public void OnObjectAllocated(T allocatedObject, int count)
			{
				CalledObjectAllocated = true;
			}

			public void OnObjectDeallocated(T allocatedObject)
			{
				CalledObjectDeallocated = true;
			}

			public void OnObjectUnspawned(T unspawnedObject)
			{
				CalledObjectUnspawned = true;
			}

			public T OnReplaceObject(List<T> activeInstances, Stack<T> inactiveInstances)
			{
				CalledReplaceObject = true;
				return m_objectPool.FullStrategy.GetObjectToReplace(m_objectPool, activeInstances, inactiveInstances);
			}
		}

		public static readonly int InitialPoolSize = 10;

		private ObjectPoolWrapper<PoolableObject> m_poolWrapper;

		private ObjectPool<PoolableObject> m_objectPool;

		[SetUp]
		public void SetUp()
		{
			Logger.PushLoggingSupression();
			PoolableObject.InstanceCount = 0;
			m_poolWrapper = new ObjectPoolWrapper<PoolableObject>();
			m_objectPool = m_poolWrapper.m_objectPool;
		}

		[TearDown]
		public void TearDown()
		{
			m_poolWrapper = null;
			m_objectPool = null;
			Logger.PopLoggingSupression();
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(m_objectPool);
		}

		[Test]
		public void TestCapacity()
		{
			Assert.AreEqual(InitialPoolSize, m_objectPool.Capacity);
		}

		[Test]
		public void TestSettingCapacity()
		{
			int num = InitialPoolSize * 2;
			Assert.AreEqual(InitialPoolSize, m_objectPool.Capacity);
			m_objectPool.Capacity = num;
			Assert.AreEqual(num, m_objectPool.Capacity);
			m_objectPool.Capacity = InitialPoolSize;
			Assert.AreEqual(InitialPoolSize, m_objectPool.Capacity);
		}

		[Test]
		public void TestReducingCapacityAtFullUtilization()
		{
			Assert.AreEqual(InitialPoolSize, m_objectPool.Capacity);
			int initialPoolSize = InitialPoolSize;
			for (int i = 0; i < initialPoolSize; i++)
			{
				Assert.NotNull(m_objectPool.Spawn());
			}
			m_objectPool.Capacity = InitialPoolSize / 2;
			Assert.AreEqual(InitialPoolSize / 2, m_objectPool.Capacity);
		}

		[Test]
		public void TestActiveInstaceCount()
		{
			int num = InitialPoolSize / 2;
			for (int i = 0; i < num; i++)
			{
				Assert.NotNull(m_objectPool.Spawn());
			}
			Assert.AreEqual(num, m_objectPool.ActiveInstanceCount);
		}

		[Test]
		public void TestUtilization()
		{
			int num = InitialPoolSize / 2;
			for (int i = 0; i < num; i++)
			{
				Assert.NotNull(m_objectPool.Spawn());
			}
			Assert.AreEqual(0.5f, m_objectPool.Utilization);
		}

		[Test]
		public void TestFullUtilization()
		{
			Assert.AreEqual(InitialPoolSize, m_objectPool.Capacity);
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.NotNull(m_objectPool.Spawn());
			}
			Assert.AreEqual(1f, m_objectPool.Utilization);
		}

		[Test]
		public void TestFullStrategy()
		{
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceYoungest>();
		}

		[Test]
		public void TestSpawn()
		{
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
			Assert.NotNull(m_objectPool.Spawn());
			Assert.AreEqual(1, m_objectPool.ActiveInstanceCount);
		}

		[Test]
		public void TestUnspawn()
		{
			PoolableObject poolableObject = null;
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
			for (int i = 0; i < 5; i++)
			{
				poolableObject = m_objectPool.Spawn();
				Assert.NotNull(poolableObject);
			}
			Assert.AreEqual(5, m_objectPool.ActiveInstanceCount);
			m_objectPool.Unspawn(poolableObject);
			Assert.AreEqual(4, m_objectPool.ActiveInstanceCount);
		}

		[Test]
		public void TestUnspawningAllObjects()
		{
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
			for (int i = 0; i < 5; i++)
			{
				PoolableObject anObject = m_objectPool.Spawn();
				Assert.NotNull(anObject);
			}
			Assert.AreEqual(5, m_objectPool.ActiveInstanceCount);
			m_objectPool.UnspawnAllObjects();
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
		}

		[Test]
		public void TestDelegates()
		{
			Assert.AreEqual(true, m_poolWrapper.CalledAllocateObject);
			Assert.AreEqual(true, m_poolWrapper.CalledObjectAllocated);
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
			PoolableObject poolableObject = null;
			poolableObject = m_objectPool.Spawn();
			Assert.NotNull(poolableObject);
			Assert.AreEqual(false, m_poolWrapper.CalledObjectUnspawned);
			m_objectPool.Unspawn(poolableObject);
			Assert.AreEqual(true, m_poolWrapper.CalledObjectUnspawned);
			for (int i = 0; i < InitialPoolSize; i++)
			{
				m_objectPool.Spawn();
			}
			Assert.AreEqual(false, m_poolWrapper.CalledReplaceObject);
			m_objectPool.Spawn();
			Assert.AreEqual(true, m_poolWrapper.CalledReplaceObject);
			Assert.AreEqual(false, m_poolWrapper.CalledObjectDeallocated);
			m_objectPool.Capacity--;
			Assert.AreEqual(true, m_poolWrapper.CalledObjectDeallocated);
		}

		[Test]
		public void TestReplaceOldestStrategy()
		{
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceOldest>();
			List<PoolableObject> list = new List<PoolableObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				PoolableObject poolableObject = m_objectPool.Spawn();
				Assert.NotNull(poolableObject);
				list.Add(poolableObject);
			}
			PoolableObject poolableObject2 = m_objectPool.Spawn();
			Assert.NotNull(poolableObject2);
			Assert.AreEqual(poolableObject2, list[0]);
		}

		[Test]
		public void TestReplaceYoungestStrategy()
		{
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceYoungest>();
			List<PoolableObject> list = new List<PoolableObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				PoolableObject poolableObject = m_objectPool.Spawn();
				Assert.NotNull(poolableObject);
				list.Add(poolableObject);
			}
			PoolableObject poolableObject2 = m_objectPool.Spawn();
			Assert.NotNull(poolableObject2);
			Assert.AreEqual(poolableObject2, list[list.Count - 1]);
		}

		[Test]
		public void TestDropObjectStrategy()
		{
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<DropObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.NotNull(m_objectPool.Spawn());
			}
			Assert.IsNull(m_objectPool.Spawn());
		}

		[Test]
		public void TestGrowPoolStrategyExpandByAmount()
		{
			int num = 5;
			GrowPool growPool = ScriptableObject.CreateInstance<GrowPool>();
			ExpandByAmount expandByAmount = ScriptableObject.CreateInstance<ExpandByAmount>();
			expandByAmount.Amount = num;
			growPool.GrowthStrategy = expandByAmount;
			m_objectPool.FullStrategy = growPool;
			List<PoolableObject> list = new List<PoolableObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				PoolableObject poolableObject = m_objectPool.Spawn();
				Assert.NotNull(poolableObject);
				list.Add(poolableObject);
			}
			PoolableObject poolableObject2 = m_objectPool.Spawn();
			Assert.NotNull(poolableObject2);
			Assert.AreEqual(false, list.Contains(poolableObject2));
			Assert.AreEqual(InitialPoolSize + num, m_objectPool.Capacity);
		}

		[Test]
		public void TestGrowPoolStrategyDoubleInSize()
		{
			GrowPool growPool = ScriptableObject.CreateInstance<GrowPool>();
			growPool.GrowthStrategy = ScriptableObject.CreateInstance<DoubleInSize>();
			m_objectPool.FullStrategy = growPool;
			List<PoolableObject> list = new List<PoolableObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				PoolableObject poolableObject = m_objectPool.Spawn();
				Assert.NotNull(poolableObject);
				list.Add(poolableObject);
			}
			PoolableObject poolableObject2 = m_objectPool.Spawn();
			Assert.NotNull(poolableObject2);
			Assert.AreEqual(false, list.Contains(poolableObject2));
			Assert.AreEqual(InitialPoolSize * 2, m_objectPool.Capacity);
		}
	}
}
