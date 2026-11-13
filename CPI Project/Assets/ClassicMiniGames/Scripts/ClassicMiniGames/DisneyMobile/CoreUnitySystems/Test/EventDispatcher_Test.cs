using NUnit.Framework;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class EventDispatcher_Test
	{
		public class TestEvent : BaseEvent
		{
		}

		private EventDispatcher mEventDispatcher = null;

		private List<int> mVisitOrder = null;

		[SetUp]
		public void SetUp()
		{
			mEventDispatcher = new EventDispatcher();
			mVisitOrder = new List<int>();
		}

		[TearDown]
		public void TearDown()
		{
			mEventDispatcher = null;
			mVisitOrder = null;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mEventDispatcher);
		}

		[Test]
		public void TestAdd()
		{
			mEventDispatcher.AddListener<TestEvent>(EventListener1);
		}

		[Test]
		public void TestRemove()
		{
			mEventDispatcher.AddListener<TestEvent>(EventListener1);
			bool condition = mEventDispatcher.RemoveListener<TestEvent>(EventListener1);
			Assert.True(condition);
		}

		[Test]
		public void TestMultiAdd()
		{
			mEventDispatcher.AddListener<TestEvent>(EventListener1);
			mEventDispatcher.AddListener<TestEvent>(EventListener2);
			mEventDispatcher.AddListener<TestEvent>(EventListener3);
		}

		[Test]
		public void TestMultiAddAndMultiRemoveInReverseOrder()
		{
			mEventDispatcher.AddListener<TestEvent>(EventListener1);
			mEventDispatcher.AddListener<TestEvent>(EventListener2);
			mEventDispatcher.AddListener<TestEvent>(EventListener3);
			bool flag = false;
			flag = mEventDispatcher.RemoveListener<TestEvent>(EventListener3);
			Assert.True(flag);
			flag = mEventDispatcher.RemoveListener<TestEvent>(EventListener2);
			Assert.True(flag);
			flag = mEventDispatcher.RemoveListener<TestEvent>(EventListener1);
			Assert.True(flag);
		}

		[Test]
		public void TestPriorityOrder()
		{
			mEventDispatcher.AddListener<TestEvent>(EventListener1, EventDispatcher.Priority.LAST);
			mEventDispatcher.AddListener<TestEvent>(EventListener2, EventDispatcher.Priority.FIRST);
			mEventDispatcher.AddListener<TestEvent>(EventListener3);
			mEventDispatcher.DispatchEvent(new TestEvent());
			Assert.AreEqual(mVisitOrder[0], 2);
			Assert.AreEqual(mVisitOrder[1], 3);
			Assert.AreEqual(mVisitOrder[2], 1);
			mVisitOrder.Clear();
			mEventDispatcher.RemoveListener<TestEvent>(EventListener3);
			mEventDispatcher.DispatchEvent(new TestEvent());
			Assert.AreEqual(mVisitOrder[0], 2);
			Assert.AreEqual(mVisitOrder[1], 1);
		}

		public bool EventListener1(TestEvent evnt)
		{
			mVisitOrder.Add(1);
			return false;
		}

		public bool EventListener2(TestEvent evnt)
		{
			mVisitOrder.Add(2);
			return false;
		}

		public bool EventListener3(TestEvent evnt)
		{
			mVisitOrder.Add(3);
			return false;
		}
	}
}
