using NUnit.Framework;
using System.Collections;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class InitManager_Test
	{
		private class TestInitAction : InitAction
		{
			public bool mPerformed = false;

			public bool mCompleted = false;

			public override IEnumerator Perform()
			{
				mPerformed = true;
				OnComplete();
				yield return null;
			}

			public new virtual void OnComplete()
			{
				mCompleted = true;
				base.OnComplete();
			}
		}

		private class TestInitAction1 : TestInitAction
		{
		}

		[InitActionDependency(typeof(TestInitAction1))]
		private class TestInitAction2 : TestInitAction
		{
		}

		[InitActionDependency(typeof(TestInitAction2))]
		private class TestInitAction3 : TestInitAction
		{
		}

		[InitActionDependency(typeof(TestInitAction3))]
		[InitActionDependency(typeof(TestInitAction1))]
		[InitActionDependency(typeof(TestInitAction2))]
		private class TestInitAction4 : TestInitAction
		{
		}

		[InitActionDependency(typeof(TestCircDependencyInitAction2))]
		private class TestCircDependencyInitAction1 : TestInitAction
		{
		}

		[InitActionDependency(typeof(TestCircDependencyInitAction1))]
		private class TestCircDependencyInitAction2 : TestInitAction
		{
		}

		private EventDispatcher mEventDispatcher = null;

		private InitManager mInitManager = null;

		private TestInitAction[] mActions = null;

		private Logger mPreviousLogger = null;

		private Logger mLogger = null;

		[SetUp]
		public void SetUp()
		{
			mPreviousLogger = Logger.Instance;
			mLogger = new Logger();
			mEventDispatcher = new EventDispatcher();
			mInitManager = new InitManager(mEventDispatcher);
			mActions = new TestInitAction[4]
			{
				new TestInitAction1(),
				new TestInitAction2(),
				new TestInitAction3(),
				new TestInitAction4()
			};
			mLogger.GetType();
		}

		[TearDown]
		public void TearDown()
		{
			mEventDispatcher = null;
			mInitManager = null;
			mLogger = null;
			for (int i = 0; i < mActions.Length; i++)
			{
				mActions[i] = null;
			}
			mActions = null;
			Logger.Instance = mPreviousLogger;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mEventDispatcher);
			Assert.NotNull(mInitManager);
			TestInitAction[] array = mActions;
			foreach (TestInitAction anObject in array)
			{
				Assert.NotNull(anObject);
			}
		}

		[Test]
		public void TestMultipleActions()
		{
			TestInitAction[] array = mActions;
			foreach (TestInitAction action in array)
			{
				mInitManager.AddInitAction(action);
			}
			mInitManager.Process(null, false);
			array = mActions;
			foreach (TestInitAction action in array)
			{
				Assert.That(action.mPerformed, Is.EqualTo(true));
				Assert.That(action.mCompleted, Is.EqualTo(true));
			}
		}

		[Test]
		public void TestCircularDependency()
		{
			InitAction action = new TestCircDependencyInitAction1();
			InitAction action2 = new TestCircDependencyInitAction2();
			mInitManager.AddInitAction(action);
			mInitManager.AddInitAction(action2);
			Logger.PushLoggingSupression();
			mInitManager.Process(null, false);
			Logger.PopLoggingSupression();
		}

		[Test]
		public void TestMissingDepenencyAction()
		{
			mInitManager.AddInitAction(mActions[3]);
			Logger.PushLoggingSupression();
			mInitManager.Process(null, false);
			Logger.PopLoggingSupression();
		}

		[Test]
		public void TestMultipleActionsWithDependencies()
		{
			TestInitAction[] array = mActions;
			foreach (TestInitAction action in array)
			{
				mInitManager.AddInitAction(action);
			}
			mInitManager.Process(null, false);
			array = mActions;
			foreach (TestInitAction action in array)
			{
				Assert.That(action.mPerformed, Is.EqualTo(true));
				Assert.That(action.mCompleted, Is.EqualTo(true));
			}
		}
	}
}
