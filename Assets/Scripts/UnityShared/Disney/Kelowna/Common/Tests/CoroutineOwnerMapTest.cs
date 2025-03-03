using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class CoroutineOwnerMapTest : MonoBehaviour
	{
		private void Start()
		{
			Service.ResetAll();
			Service.Set(base.gameObject.AddComponent<CoroutineRunner>());
			CoroutineRunner.Start(runTests(), this, "runTests");
		}

		private IEnumerator runTests()
		{
			yield return CoroutineRunner.Start(add_VerifyDoesContain(), this, "add_VerifyDoesContain");
			yield return CoroutineRunner.Start(stopAllForOwner_VerifyDidStop(), this, "stopAllForOwner_VerifyDidStop");
			yield return CoroutineRunner.Start(stopAll_VerifyDidStop(), this, "stopAll_VerifyDidStop");
			yield return CoroutineRunner.Start(stopAll_VerifyMultipleDidStop(), this, "stopAll_VerifyMultipleDidStop");
			IntegrationTest.Pass();
		}

		private IEnumerator add_VerifyDoesContain()
		{
			CoroutineOwnerMap<RadicalCoroutine> map = new CoroutineOwnerMap<RadicalCoroutine>();
			ICoroutine coroutine = CoroutineRunner.Start(testerCoroutine(), this, "testerCoroutine");
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 0);
			map.Add(this, (RadicalCoroutine)coroutine);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 1);
			map.Add(this, (RadicalCoroutine)coroutine);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 1);
			yield return coroutine;
		}

		private IEnumerator stopAllForOwner_VerifyDidStop()
		{
			CoroutineOwnerMap<RadicalCoroutine> map = new CoroutineOwnerMap<RadicalCoroutine>();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = CoroutineRunner.Start(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			map.Add(this, (RadicalCoroutine)coroutine);
			map.StopAllForOwner(this);
			yield return coroutine;
			IntegrationTestEx.FailIf(testerDidCancel);
			IntegrationTestEx.FailIf(testerDidFinish);
			IntegrationTestEx.FailIf(!coroutine.Disposed);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 0);
		}

		private IEnumerator stopAll_VerifyDidStop()
		{
			CoroutineOwnerMap<RadicalCoroutine> map = new CoroutineOwnerMap<RadicalCoroutine>();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = CoroutineRunner.Start(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			map.Add(this, (RadicalCoroutine)coroutine);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 1);
			map.StopAll();
			yield return coroutine;
			IntegrationTestEx.FailIf(testerDidCancel);
			IntegrationTestEx.FailIf(testerDidFinish);
			IntegrationTestEx.FailIf(!coroutine.Disposed);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 0);
		}

		private IEnumerator stopAll_VerifyMultipleDidStop()
		{
			CoroutineOwnerMap<RadicalCoroutine> map = new CoroutineOwnerMap<RadicalCoroutine>();
			bool testerDidFinish1 = false;
			bool testerDidCancel1 = false;
			bool testerDidFinish2 = false;
			bool testerDidCancel2 = false;
			ICoroutine coroutine1 = CoroutineRunner.Start(testerCoroutine(), this, "testerCoroutine_1");
			coroutine1.ECompleted += delegate
			{
				testerDidFinish1 = true;
			};
			coroutine1.ECancelled += delegate
			{
				testerDidCancel1 = true;
			};
			map.Add(this, (RadicalCoroutine)coroutine1);
			ICoroutine coroutine2 = CoroutineRunner.Start(testerCoroutine(), this, "testerCoroutine_2");
			coroutine2.ECompleted += delegate
			{
				testerDidFinish2 = true;
			};
			coroutine2.ECancelled += delegate
			{
				testerDidCancel2 = true;
			};
			map.Add(this, (RadicalCoroutine)coroutine2);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 2);
			map.StopAll();
			yield return coroutine2;
			IntegrationTestEx.FailIf(testerDidCancel1);
			IntegrationTestEx.FailIf(testerDidFinish1);
			IntegrationTestEx.FailIf(!coroutine1.Disposed);
			IntegrationTestEx.FailIf(testerDidCancel2);
			IntegrationTestEx.FailIf(testerDidFinish2);
			IntegrationTestEx.FailIf(!coroutine2.Disposed);
			IntegrationTestEx.FailIf(map.GetCountForOwner(this) != 0);
		}

		private IEnumerator testerCoroutine()
		{
			yield return null;
			yield return null;
		}
	}
}
