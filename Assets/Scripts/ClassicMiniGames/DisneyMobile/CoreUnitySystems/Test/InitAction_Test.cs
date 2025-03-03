using DisneyMobile.CoreUnitySystems.Utility;
using NUnit.Framework;
using System.Collections;

namespace DisneyMobile.CoreUnitySystems.Test
{
	[TestFixture]
	public class InitAction_Test
	{
		private class TestInitAction : InitAction
		{
			public bool mPerformed = false;

			public bool mCompleted = false;

			public TestInitAction(EventDispatcher eventDispatcher, Configurator configurator)
			{
				Initialize(null, eventDispatcher, configurator);
			}

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

		[InitActionDependency(typeof(TestInitAction))]
		private class AntecedentInitAction : TestInitAction
		{
			public AntecedentInitAction(EventDispatcher eventDispatcher, Configurator configurator)
				: base(eventDispatcher, configurator)
			{
			}
		}

		private EventDispatcher mEventDispatcher = null;

		private TestInitAction mAction = null;

		[SetUp]
		public void SetUp()
		{
			mEventDispatcher = new EventDispatcher();
			mAction = new TestInitAction(mEventDispatcher, null);
		}

		[TearDown]
		public void TearDown()
		{
			mAction = null;
			mEventDispatcher = null;
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mEventDispatcher);
			Assert.NotNull(mAction);
		}

		[Test]
		public void TestPerform()
		{
			Assert.That(mAction.CanStart(), Is.EqualTo(true));
			mAction.Start();
			CoroutineHelper.RunCoroutineToCompletion(mAction.Perform());
			Assert.That(mAction.mPerformed, Is.EqualTo(true));
			Assert.That(mAction.mCompleted, Is.EqualTo(true));
		}

		[Test]
		public void TestPerformWithDependency()
		{
			TestInitAction testInitAction = new TestInitAction(mEventDispatcher, null);
			Assert.That(testInitAction.CanStart(), Is.EqualTo(true));
			testInitAction.Start();
			CoroutineHelper.RunCoroutineToCompletion(testInitAction.Perform());
			Assert.That(testInitAction.mPerformed, Is.EqualTo(true));
			Assert.That(testInitAction.mCompleted, Is.EqualTo(true));
			Assert.That(mAction.CanStart(), Is.EqualTo(true));
			mAction.Start();
			CoroutineHelper.RunCoroutineToCompletion(mAction.Perform());
			Assert.That(mAction.mPerformed, Is.EqualTo(true));
			Assert.That(mAction.mCompleted, Is.EqualTo(true));
		}
	}
}
