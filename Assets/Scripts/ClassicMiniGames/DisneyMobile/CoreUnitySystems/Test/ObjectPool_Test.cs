using NUnit.Framework;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class ObjectPool_Test
	{
		private class PoolableObject
		{
			public static int InstanceCount = 0;

			public PoolableObject()
			{
				InstanceCount++;
			}
		}

		public static readonly int InitialPoolSize = 10;

		private ObjectPool<PoolableObject> m_objectPool = null;

		[SetUp]
		public void SetUp()
		{
			Logger.PushLoggingSupression();
			PoolableObject.InstanceCount = 0;
			m_objectPool = new ObjectPool<PoolableObject>(10, null);
		}

		[TearDown]
		public void TearDown()
		{
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
	}
}
