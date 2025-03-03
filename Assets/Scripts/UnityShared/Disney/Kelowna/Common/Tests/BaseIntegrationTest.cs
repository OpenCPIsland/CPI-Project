using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.Utility.Assert;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BaseIntegrationTest : MonoBehaviour
	{
		public GameObject[] EnableAfterSetup;

		public bool EnableAllChildenAfterSetup;

		public bool WaitAfterSetup;

		private bool isStarted = false;

		private bool isWaiting = false;

		private ICoroutine testCoroutine;

		private TestCoroutineRunner testCoroutineRunner;

		protected virtual IEnumerator setup()
		{
			yield break;
		}

		protected virtual IEnumerator runTest()
		{
			yield break;
		}

		protected virtual void tearDown()
		{
		}

		public void OnEnable()
		{
			Assert.DidAssert = false;
			Service.ResetAll();
			Service.Set(new EventDispatcher());
			Service.Set((JsonService)new LitJsonService());
			Service.Set((ICommonGameSettings)new MockGameSettings());
			Configurator configurator = new Configurator();
			configurator.Init(false);
			Service.Set(configurator);
			GameObject gameObject = GameObject.Find("TestCoroutineRunner");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
			gameObject = new GameObject("TestCoroutineRunner");
			testCoroutineRunner = gameObject.AddComponent<TestCoroutineRunner>();
			GameObject gameObject2 = GameObject.Find("CoroutineRunner");
			if (gameObject2 != null)
			{
				Object.Destroy(gameObject2);
			}
			gameObject2 = new GameObject("CoroutineRunner");
			CoroutineRunner instance = gameObject2.AddComponent<CoroutineRunner>();
			Service.Set(instance);
			Log.Instance.LogAllTypesPriorities = Log.PriorityFlags.ALL;
		}

		public void Update()
		{
			if (!isStarted)
			{
				isStarted = true;
				ICoroutine coroutine = testCoroutineRunner.StartTestCoroutine(setup(), this, "setup");
				if (coroutine.Disposed)
				{
					onSetupCompleted();
					return;
				}
				coroutine.ECompleted += onSetupCompleted;
				coroutine.ECancelled += delegate
				{
					testFinished(true);
				};
			}
		}

		protected void wait()
		{
			isWaiting = true;
		}

		protected void doneWaiting()
		{
			isWaiting = false;
		}

		private void onSetupCompleted()
		{
			if (EnableAfterSetup != null)
			{
				GameObject[] enableAfterSetup = EnableAfterSetup;
				foreach (GameObject gameObject in enableAfterSetup)
				{
					gameObject.SetActive(true);
				}
			}
			if (EnableAllChildenAfterSetup)
			{
				foreach (Transform item in base.gameObject.transform)
				{
					item.gameObject.SetActive(true);
				}
			}
			if (WaitAfterSetup)
			{
				wait();
			}
			ICoroutine coroutine = testCoroutineRunner.StartTestCoroutine(runTestWrapper(), this, "runTestWrapper");
			if (coroutine.Disposed)
			{
				testFinished(coroutine.Cancelled);
				return;
			}
			coroutine.ECompleted += delegate
			{
				testFinished(false);
			};
			coroutine.ECancelled += delegate
			{
				testFinished(true);
			};
		}

		private IEnumerator runTestWrapper()
		{
			testCoroutine = testCoroutineRunner.StartTestCoroutine(runTest(), this, "runTest");
			yield return testCoroutine;
			while (isWaiting)
			{
				yield return null;
			}
		}

		private void testFinished(bool wasCancelled)
		{
			if (Assert.DidAssert)
			{
				IntegrationTest.Fail("Integration test threw an assert. See assert message above.");
			}
			else if (wasCancelled)
			{
				IntegrationTest.Fail("Integration test coroutine was cancelled");
			}
			else
			{
				IntegrationTest.Pass();
			}
		}

		protected ICoroutine StartTestCoroutine(IEnumerator enumerator, object owner, string debugName)
		{
			return testCoroutineRunner.StartTestCoroutine(enumerator, owner, debugName);
		}

		public void OnDisable()
		{
			if (isStarted)
			{
				if (!testCoroutine.Cancelled && !testCoroutine.Disposed)
				{
					testCoroutine.Stop();
				}
				tearDown();
				Service.ResetAll();
			}
		}
	}
}
