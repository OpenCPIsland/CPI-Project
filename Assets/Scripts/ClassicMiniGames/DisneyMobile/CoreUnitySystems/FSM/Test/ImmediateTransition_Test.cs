using NUnit.Framework;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM.Test
{
	[TestFixture]
	public class ImmediateTransition_Test
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

		private ImmediateTransition mTransition = null;

		private GameObject mGameObject = null;

		private State[] mState = null;

		private StateWorker[] mStateWorker = null;

		private GameObject[] mStateGameObject = null;

		[SetUp]
		public void SetUp()
		{
			mGameObject = new GameObject();
			mGameObject.AddComponent<ImmediateTransition>();
			Assert.NotNull(mGameObject);
			mTransition = mGameObject.GetComponent<ImmediateTransition>();
			Assert.NotNull(mTransition);
			mState = new State[2];
			mStateWorker = new StateWorker[2];
			mStateGameObject = new GameObject[2];
			for (int i = 0; i < 2; i++)
			{
				mStateGameObject[i] = new GameObject();
				Assert.NotNull(mStateGameObject[i]);
				mStateGameObject[i].AddComponent<State>();
				mState[i] = mStateGameObject[i].GetComponent<State>();
				Assert.NotNull(mState[i]);
				mStateWorker[i] = new StateWorker();
				Assert.NotNull(mStateWorker[i]);
			}
		}

		[TearDown]
		public void TearDown()
		{
			for (int i = 0; i < 2; i++)
			{
				Object.DestroyImmediate(mStateGameObject[i]);
				Object.DestroyImmediate(mState[i]);
				mStateWorker[i] = null;
			}
			Object.DestroyImmediate(mGameObject);
			Object.DestroyImmediate(mTransition);
		}

		[Test]
		public void TestCreation()
		{
			Assert.NotNull(mGameObject);
			Assert.NotNull(mTransition);
			for (int i = 0; i < 2; i++)
			{
				Assert.NotNull(mStateGameObject[i]);
				Assert.NotNull(mState[i]);
				Assert.NotNull(mStateWorker[i]);
			}
		}

		[Test]
		public void TestTransition()
		{
			for (int i = 0; i < 2; i++)
			{
				mState[i].EventDispatcher.AddListener<StatePreEnterEvent>(mStateWorker[i].OnPreEnter);
				mState[i].EventDispatcher.AddListener<StateEnterEvent>(mStateWorker[i].OnEnter);
				mState[i].EventDispatcher.AddListener<StatePostEnterEvent>(mStateWorker[i].OnPostEnter);
				mState[i].EventDispatcher.AddListener<StatePreUpdateEvent>(mStateWorker[i].OnPreUpdate);
				mState[i].EventDispatcher.AddListener<StateUpdateEvent>(mStateWorker[i].OnUpdate);
				mState[i].EventDispatcher.AddListener<StatePostUpdateEvent>(mStateWorker[i].OnPostUpdate);
				mState[i].EventDispatcher.AddListener<StatePreExitEvent>(mStateWorker[i].OnPreExit);
				mState[i].EventDispatcher.AddListener<StateExitEvent>(mStateWorker[i].OnExit);
				mState[i].EventDispatcher.AddListener<StatePostExitEvent>(mStateWorker[i].OnPostExit);
			}
			for (int i = 0; i < 2; i++)
			{
				Assert.That(mStateWorker[i].mOnPreEnterCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnEnterCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPostEnterCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPreUpdateCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnUpdateCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPostUpdateCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPreExitCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnExitCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPostExitCalledCount, Is.EqualTo(0));
			}
			mTransition.Perform(new StateChangeArgs(null, mState[0], mState[1], null, mTransition));
			Assert.That(mStateWorker[0].mOnPreExitCalledCount, Is.EqualTo(1));
			Assert.That(mStateWorker[0].mOnExitCalledCount, Is.EqualTo(1));
			Assert.That(mStateWorker[0].mOnPostExitCalledCount, Is.EqualTo(1));
			Assert.That(mStateWorker[1].mOnPreEnterCalledCount, Is.EqualTo(1));
			Assert.That(mStateWorker[1].mOnEnterCalledCount, Is.EqualTo(1));
			Assert.That(mStateWorker[1].mOnPostEnterCalledCount, Is.EqualTo(1));
			for (int i = 0; i < 2; i++)
			{
				Assert.That(mStateWorker[i].mOnPreUpdateCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnUpdateCalledCount, Is.EqualTo(0));
				Assert.That(mStateWorker[i].mOnPostUpdateCalledCount, Is.EqualTo(0));
			}
		}
	}
}
