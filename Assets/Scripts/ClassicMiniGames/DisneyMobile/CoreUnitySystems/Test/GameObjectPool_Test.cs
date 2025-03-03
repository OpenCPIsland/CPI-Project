using DisneyMobile.CoreUnitySystems.PoolStrategies;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class GameObjectPool_Test
	{
		public static readonly int InitialPoolSize = 10;

		private GameObjectPool m_objectPool = null;

		private GameObject m_poolGameObject = null;

		[SetUp]
		public void SetUp()
		{
			Logger.PushLoggingSupression();
			m_poolGameObject = new GameObject();
			m_poolGameObject.AddComponent<GameObjectPool>();
			m_objectPool = m_poolGameObject.GetComponent<GameObjectPool>();
			m_poolGameObject.name = "GameObjectPool";
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<DropObject>();
			m_objectPool.Capacity = InitialPoolSize;
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(m_poolGameObject);
			m_poolGameObject = null;
			m_objectPool = null;
			Logger.PopLoggingSupression();
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(m_objectPool);
		}

		[Test]
		public void TestSpawn()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "object";
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn());
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithLifetime()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(1f));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithPriority()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component);
			component.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(5));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithTransform()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(m_objectPool.transform));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithTransformAndLifetime()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(m_objectPool.transform, 1f));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithTransformAndPriority()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component);
			component.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(m_objectPool.transform, 5));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithTrandformAndPriorityAndLifetime()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component2 = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component2);
			component2.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(m_objectPool.transform, 4, 1f));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithPositionAndRotation()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component2 = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component2);
			component2.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			Vector3 position = new Vector3(0f, 0f, 0f);
			Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(position, rotation));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithPositionAndRotationAndPriority()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component2 = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component2);
			component2.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			Vector3 position = new Vector3(0f, 0f, 0f);
			Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(position, rotation, 4));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithPositionAndRotationAndLifetime()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component2 = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component2);
			component2.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			Vector3 position = new Vector3(0f, 0f, 0f);
			Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(position, rotation, 1f));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestSpawnWithPositionAndRotationAndPriorityAndLifetime()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "LifetimeObject";
			gameObject.AddComponent<LifetimeComponent>();
			LifetimeComponent component = gameObject.GetComponent<LifetimeComponent>();
			Assert.IsNotNull(component);
			component.SetLifetime(1f);
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component2 = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component2);
			component2.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			Vector3 position = new Vector3(0f, 0f, 0f);
			Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Assert.IsNotNull(m_objectPool.Spawn(position, rotation, 4, 1f));
			}
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestUnspawn()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "Object";
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			GameObject gameObject2 = null;
			for (int i = 0; i < InitialPoolSize; i++)
			{
				gameObject2 = m_objectPool.Spawn(m_objectPool.transform);
				Assert.IsNotNull(gameObject2);
			}
			m_objectPool.Unspawn(gameObject2);
			Assert.AreEqual(InitialPoolSize - 1, m_objectPool.ActiveInstanceCount);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestUnspawnAll()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "Object";
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.OnEnable();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				GameObject anObject = m_objectPool.Spawn(m_objectPool.transform);
				Assert.IsNotNull(anObject);
			}
			m_objectPool.UnspawnAllObjects();
			Assert.AreEqual(0, m_objectPool.ActiveInstanceCount);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestReplacementStrategyHighestPriority()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component);
			component.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceHighestPriority>();
			m_objectPool.OnEnable();
			GameObject gameObject2 = null;
			for (int i = 0; i < InitialPoolSize; i++)
			{
				if (i == InitialPoolSize / 2)
				{
					gameObject2 = m_objectPool.Spawn(10);
					Assert.IsNotNull(gameObject2);
				}
				else
				{
					Assert.IsNotNull(m_objectPool.Spawn(5));
				}
			}
			GameObject actual = m_objectPool.Spawn(5);
			Assert.AreEqual(gameObject2, actual);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestReplacementStrategyLowestPriority()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			gameObject.AddComponent<PriorityComponent>();
			PriorityComponent component = gameObject.GetComponent<PriorityComponent>();
			Assert.IsNotNull(component);
			component.Priority = 1;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceLowestPriority>();
			m_objectPool.OnEnable();
			GameObject gameObject2 = null;
			for (int i = 0; i < InitialPoolSize; i++)
			{
				if (i == InitialPoolSize / 2)
				{
					gameObject2 = m_objectPool.Spawn(1);
					Assert.IsNotNull(gameObject2);
				}
				else
				{
					Assert.IsNotNull(m_objectPool.Spawn(5));
				}
			}
			GameObject actual = m_objectPool.Spawn(5);
			Assert.AreEqual(gameObject2, actual);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		private GameObject CreateGameObjectWithTransformAtPosition(Vector3 position)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "ObjectWithTransformAtPosition";
			gameObject.transform.position = position;
			return gameObject;
		}

		[Test]
		public void TestReplacementStrategyClosestToTransform()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			GameObject gameObject2 = new GameObject();
			Assert.IsNotNull(gameObject2);
			gameObject2.name = "Target";
			gameObject2.transform.position = new Vector3(0f, 0f, 0f);
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceClosestToTransform>();
			ReplaceClosestToTransform replaceClosestToTransform = m_objectPool.FullStrategy as ReplaceClosestToTransform;
			replaceClosestToTransform.TargetTransform = gameObject2.transform;
			m_objectPool.OnEnable();
			GameObject gameObject3 = null;
			Transform transform = CreateGameObjectWithTransformAtPosition(new Vector3(0f, 10f, 0f)).transform;
			Transform transform2 = CreateGameObjectWithTransformAtPosition(new Vector3(0f, 5f, 0f)).transform;
			for (int i = 0; i < InitialPoolSize; i++)
			{
				if (i == InitialPoolSize / 2)
				{
					gameObject3 = m_objectPool.Spawn(transform2);
					Assert.IsNotNull(gameObject3);
				}
				else
				{
					Assert.IsNotNull(m_objectPool.Spawn(transform));
				}
			}
			GameObject actual = m_objectPool.Spawn(transform2);
			Assert.AreEqual(gameObject3, actual);
			Object.DestroyImmediate(transform.gameObject);
			Object.DestroyImmediate(transform2.gameObject);
			Object.DestroyImmediate(gameObject2);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestReplacementStrategyFarthestFromTransform()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			GameObject gameObject2 = new GameObject();
			Assert.IsNotNull(gameObject2);
			gameObject2.name = "Target";
			gameObject2.transform.position = new Vector3(0f, 0f, 0f);
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = ScriptableObject.CreateInstance<ReplaceFarthestFromTransform>();
			ReplaceFarthestFromTransform replaceFarthestFromTransform = m_objectPool.FullStrategy as ReplaceFarthestFromTransform;
			replaceFarthestFromTransform.TargetTransform = gameObject2.transform;
			m_objectPool.OnEnable();
			GameObject gameObject3 = null;
			Transform transform = CreateGameObjectWithTransformAtPosition(new Vector3(0f, 10f, 0f)).transform;
			Transform transform2 = CreateGameObjectWithTransformAtPosition(new Vector3(0f, 5f, 0f)).transform;
			for (int i = 0; i < InitialPoolSize; i++)
			{
				if (i == InitialPoolSize / 2)
				{
					gameObject3 = m_objectPool.Spawn(transform);
					Assert.IsNotNull(gameObject3);
				}
				else
				{
					Assert.IsNotNull(m_objectPool.Spawn(transform2));
				}
			}
			GameObject actual = m_objectPool.Spawn(transform);
			Assert.AreEqual(gameObject3, actual);
			Object.DestroyImmediate(transform.gameObject);
			Object.DestroyImmediate(transform2.gameObject);
			Object.DestroyImmediate(gameObject2);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestGrowthStrategyExpandByAmount()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			int num = 5;
			GrowPool growPool = ScriptableObject.CreateInstance<GrowPool>();
			ExpandByAmount expandByAmount = ScriptableObject.CreateInstance<ExpandByAmount>();
			expandByAmount.Amount = num;
			growPool.GrowthStrategy = expandByAmount;
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = growPool;
			m_objectPool.OnEnable();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				GameObject gameObject2 = m_objectPool.Spawn();
				Assert.NotNull(gameObject2);
				list.Add(gameObject2);
			}
			GameObject gameObject3 = m_objectPool.Spawn();
			Assert.NotNull(gameObject3);
			Assert.AreEqual(false, list.Contains(gameObject3));
			Assert.AreEqual(InitialPoolSize + num, m_objectPool.Capacity);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}

		[Test]
		public void TestGrowthStrategyDoubleInSize()
		{
			GameObject gameObject = new GameObject();
			Assert.IsNotNull(gameObject);
			gameObject.name = "PriorityObject";
			GrowPool growPool = ScriptableObject.CreateInstance<GrowPool>();
			growPool.GrowthStrategy = ScriptableObject.CreateInstance<DoubleInSize>();
			m_objectPool.PrefabToInstance = Object.Instantiate(gameObject);
			Assert.IsNotNull(m_objectPool.PrefabToInstance);
			m_objectPool.FullStrategy = growPool;
			m_objectPool.OnEnable();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < InitialPoolSize; i++)
			{
				GameObject gameObject2 = m_objectPool.Spawn();
				Assert.NotNull(gameObject2);
				list.Add(gameObject2);
			}
			GameObject gameObject3 = m_objectPool.Spawn();
			Assert.NotNull(gameObject3);
			Assert.AreEqual(false, list.Contains(gameObject3));
			Assert.AreEqual(InitialPoolSize * 2, m_objectPool.Capacity);
			Object.DestroyImmediate(m_objectPool.PrefabToInstance);
			Object.DestroyImmediate(gameObject);
		}
	}
}
