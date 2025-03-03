using NUnit.Framework;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM.Test
{
	[TestFixture]
	public class ManualSignal_Test
	{
		private ManualSignal mSignal = null;

		private GameObject mGameObject = null;

		[SetUp]
		public void SetUp()
		{
			mGameObject = new GameObject();
			mGameObject.AddComponent<ManualSignal>();
			Assert.NotNull(mGameObject);
			mSignal = mGameObject.GetComponent<ManualSignal>();
			Assert.NotNull(mSignal);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(mSignal);
			Object.DestroyImmediate(mGameObject);
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mSignal);
		}

		[Test]
		public void TestSignalNotActive()
		{
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(false));
		}

		[Test]
		public void TestSignalActivated()
		{
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(false));
			mSignal.ActivateSignal();
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(true));
		}

		[Test]
		public void TestSignalActivatedByEvent()
		{
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(false));
			EventDispatcher eventDispatcher = new EventDispatcher();
			eventDispatcher.AddListener<SignalActivateEvent>(mSignal.OnActivateSignal);
			eventDispatcher.DispatchEvent(new SignalActivateEvent());
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(true));
		}

		[Test]
		public void TestSignalReset()
		{
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(false));
			mSignal.ActivateSignal();
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(true));
			mSignal.Reset();
			Assert.That(mSignal.IsSignaled(), Is.EqualTo(false));
		}
	}
}
