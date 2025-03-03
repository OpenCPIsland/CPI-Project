using NUnit.Framework;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM.Test
{
	[TestFixture]
	public class State_Test
	{
		private class StateWorker
		{
			public int mOnPreEnterCalledCount = 0;

			public int mOnEnterCalledCount = 0;

			public int mOnPostEnterCalledCount = 0;

			public int mOnPreUpdateCalledCount = 0;

			public int mOnUpdateCalledCount = 0;

			public int mOnPostUpdateCalledCount = 0;

			public int mOnPreExitCalledCount = 0;

			public int mOnExitCalledCount = 0;

			public int mOnPostExitCalledCount = 0;

			public bool OnPreEnter(StatePreEnterEvent evnt)
			{
				mOnPreEnterCalledCount++;
				return false;
			}

			public bool OnEnter(StateEnterEvent evnt)
			{
				mOnEnterCalledCount++;
				return false;
			}

			public bool OnPostEnter(StatePostEnterEvent evnt)
			{
				mOnPostEnterCalledCount++;
				return false;
			}

			public bool OnPreUpdate(StatePreUpdateEvent evnt)
			{
				mOnPreUpdateCalledCount++;
				return false;
			}

			public bool OnUpdate(StateUpdateEvent evnt)
			{
				mOnUpdateCalledCount++;
				return false;
			}

			public bool OnPostUpdate(StatePostUpdateEvent evnt)
			{
				mOnPostUpdateCalledCount++;
				return false;
			}

			public bool OnPreExit(StatePreExitEvent evnt)
			{
				mOnPreExitCalledCount++;
				return false;
			}

			public bool OnExit(StateExitEvent evnt)
			{
				mOnExitCalledCount++;
				return false;
			}

			public bool OnPostExit(StatePostExitEvent evnt)
			{
				mOnPostExitCalledCount++;
				return false;
			}
		}

		private State mState = null;

		private GameObject mGameObject = null;

		[SetUp]
		public void SetUp()
		{
			mGameObject = new GameObject();
			mGameObject.AddComponent<State>();
			Assert.NotNull(mGameObject);
			mState = mGameObject.GetComponent<State>();
			Assert.NotNull(mState);
		}

		[TearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(mState);
			Object.DestroyImmediate(mGameObject);
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mState);
		}

		[Test]
		public void TestStateOutsideSetup()
		{
			StateWorker stateWorker = new StateWorker();
			mState.EventDispatcher.AddListener<StatePreEnterEvent>(stateWorker.OnPreEnter);
			mState.EventDispatcher.AddListener<StateEnterEvent>(stateWorker.OnEnter);
			mState.EventDispatcher.AddListener<StatePostEnterEvent>(stateWorker.OnPostEnter);
			mState.EventDispatcher.AddListener<StatePreUpdateEvent>(stateWorker.OnPreUpdate);
			mState.EventDispatcher.AddListener<StateUpdateEvent>(stateWorker.OnUpdate);
			mState.EventDispatcher.AddListener<StatePostUpdateEvent>(stateWorker.OnPostUpdate);
			mState.EventDispatcher.AddListener<StatePreExitEvent>(stateWorker.OnPreExit);
			mState.EventDispatcher.AddListener<StateExitEvent>(stateWorker.OnExit);
			mState.EventDispatcher.AddListener<StatePostExitEvent>(stateWorker.OnPostExit);
			Assert.That(stateWorker.mOnPreEnterCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnEnterCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnPostEnterCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnPreUpdateCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnUpdateCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnPostUpdateCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnPreExitCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnExitCalledCount, Is.EqualTo(0));
			Assert.That(stateWorker.mOnPostExitCalledCount, Is.EqualTo(0));
			StateChangeArgs stateChangeDetails = new StateChangeArgs(null, null, null, null, null);
			mState.RaisePreEnterEvent(stateChangeDetails);
			mState.RaiseEnterEvent(stateChangeDetails);
			mState.RaisePostEnterEvent(stateChangeDetails);
			mState.RaisePreUpdateEvent();
			mState.RaiseUpdateEvent();
			mState.RaisePostUpdateEvent();
			mState.RaisePreExitEvent(stateChangeDetails);
			mState.RaiseExitEvent(stateChangeDetails);
			mState.RaisePostExitEvent(stateChangeDetails);
			Assert.That(stateWorker.mOnPreEnterCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnEnterCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnPostEnterCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnPreUpdateCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnUpdateCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnPostUpdateCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnPreExitCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnExitCalledCount, Is.EqualTo(1));
			Assert.That(stateWorker.mOnPostExitCalledCount, Is.EqualTo(1));
			for (int i = 0; i < 10; i++)
			{
				mState.RaiseUpdateEvent();
			}
			Assert.That(stateWorker.mOnUpdateCalledCount, Is.EqualTo(11));
		}
	}
}
