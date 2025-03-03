using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class CoroutineGroupTest : MonoBehaviour
	{
		private void Start()
		{
			Service.ResetAll();
			Service.Set(base.gameObject.AddComponent<CoroutineRunner>());
			CoroutineRunner.Start(runTests(), this, "runTests");
		}

		private IEnumerator runTests()
		{
			yield return CoroutineRunner.Start(startAndAdd_VerifyDidComplete(), this, "startAndAdd_VerifyDidComplete");
			yield return CoroutineRunner.Start(stopAll_VerifyDidCancel(), this, "stopAll_VerifyDidCancel");
			yield return CoroutineRunner.Start(clear_VerifyDidNotCancel(), this, "clear_VerifyDidNotCancel");
			yield return CoroutineRunner.Start(add_VerifyDoesContainAndDidFinish(), this, "add_VerifyDoesContainAndDidFinish");
			yield return CoroutineRunner.Start(remove_VerifyDoesNotContain(), this, "remove_VerifyDoesNotContain");
			yield return CoroutineRunner.Start(startAndAddEmptyEnumerator_VerifyNoError(), this, "startAndAddEmptyEnumerator_VerifyNoError");
			IntegrationTest.Pass();
		}

		private IEnumerator startAndAdd_VerifyDidComplete()
		{
			CoroutineGroup group = new CoroutineGroup();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			IntegrationTestEx.FailIf(!group.Contains(coroutine));
			yield return coroutine;
			IntegrationTestEx.FailIf(!testerDidFinish);
			IntegrationTestEx.FailIf(testerDidCancel);
			IntegrationTestEx.FailIf(group.Contains(coroutine));
		}

		private IEnumerator stopAll_VerifyDidCancel()
		{
			CoroutineGroup group = new CoroutineGroup();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			IntegrationTestEx.FailIf(!group.Contains(coroutine));
			group.StopAll();
			yield return coroutine;
			IntegrationTestEx.FailIf(!testerDidCancel);
			IntegrationTestEx.FailIf(testerDidFinish);
			IntegrationTestEx.FailIf(group.Contains(coroutine));
		}

		private IEnumerator clear_VerifyDidNotCancel()
		{
			CoroutineGroup group = new CoroutineGroup();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			group.Clear();
			yield return coroutine;
			IntegrationTestEx.FailIf(testerDidCancel);
			IntegrationTestEx.FailIf(!testerDidFinish);
		}

		private IEnumerator add_VerifyDoesContainAndDidFinish()
		{
			CoroutineGroup group = new CoroutineGroup();
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
			IntegrationTestEx.FailIf(group.Contains(coroutine));
			group.Add(coroutine);
			IntegrationTestEx.FailIf(!group.Contains(coroutine));
			yield return coroutine;
			IntegrationTestEx.FailIf(group.Contains(coroutine));
			IntegrationTestEx.FailIf(!testerDidFinish);
			IntegrationTestEx.FailIf(testerDidCancel);
		}

		private IEnumerator startAndAddMultiple_VerifyDidComplete()
		{
			CoroutineGroup group = new CoroutineGroup();
			bool tester1DidFinish = false;
			bool tester1DidCancel = false;
			bool tester2DidFinish = false;
			bool tester2DidCancel = false;
			ICoroutine coroutine1 = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine1");
			ICoroutine coroutine2 = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine2");
			coroutine1.ECompleted += delegate
			{
				tester1DidFinish = true;
			};
			coroutine1.ECancelled += delegate
			{
				tester1DidCancel = true;
			};
			coroutine2.ECompleted += delegate
			{
				tester2DidFinish = true;
			};
			coroutine2.ECancelled += delegate
			{
				tester2DidCancel = true;
			};
			IntegrationTestEx.FailIf(!group.Contains(coroutine1));
			IntegrationTestEx.FailIf(!group.Contains(coroutine2));
			while (!group.IsFinished)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!tester1DidFinish);
			IntegrationTestEx.FailIf(tester1DidCancel);
			IntegrationTestEx.FailIf(!tester2DidFinish);
			IntegrationTestEx.FailIf(tester2DidCancel);
			IntegrationTestEx.FailIf(group.Contains(coroutine1));
			IntegrationTestEx.FailIf(group.Contains(coroutine2));
		}

		private IEnumerator remove_VerifyDoesNotContain()
		{
			CoroutineGroup group = new CoroutineGroup();
			bool testerDidFinish = false;
			bool testerDidCancel = false;
			ICoroutine coroutine = group.StartAndAdd(testerCoroutine(), this, "testerCoroutine");
			coroutine.ECompleted += delegate
			{
				testerDidFinish = true;
			};
			coroutine.ECancelled += delegate
			{
				testerDidCancel = true;
			};
			IntegrationTestEx.FailIf(!group.Contains(coroutine));
			group.Remove(coroutine);
			IntegrationTestEx.FailIf(group.Contains(coroutine));
			yield return coroutine;
			IntegrationTestEx.FailIf(!testerDidFinish);
			IntegrationTestEx.FailIf(testerDidCancel);
			IntegrationTestEx.FailIf(group.Contains(coroutine));
		}

		private IEnumerator startAndAddEmptyEnumerator_VerifyNoError()
		{
			CoroutineGroup group = new CoroutineGroup();
			yield return group.StartAndAdd(testerCoroutineEmptyEnumator(), this, "testerCoroutineEmptyEnumator");
		}

		private IEnumerator isFinished_VerifyTrue()
		{
			CoroutineGroup group = new CoroutineGroup();
			yield return group.StartAndAdd(testerCoroutine(), this, "testerCoroutineEmptyEnumator");
			IntegrationTestEx.FailIf(!group.IsFinished);
		}

		private IEnumerator isFinished_VerifyFalse()
		{
			CoroutineGroup coroutineGroup = new CoroutineGroup();
			coroutineGroup.StartAndAdd(testerCoroutine(), this, "testerCoroutineEmptyEnumator");
			IntegrationTestEx.FailIf(coroutineGroup.IsFinished);
			yield break;
		}

		private IEnumerator isFinishedEmpty_VerifyTrue()
		{
			CoroutineGroup coroutineGroup = new CoroutineGroup();
			IntegrationTestEx.FailIf(!coroutineGroup.IsFinished);
			yield break;
		}

		private IEnumerator isFinishedStopAll_VerifyTrue()
		{
			CoroutineGroup coroutineGroup = new CoroutineGroup();
			coroutineGroup.StartAndAdd(testerCoroutine(), this, "testerCoroutineEmptyEnumator");
			coroutineGroup.StopAll();
			IntegrationTestEx.FailIf(!coroutineGroup.IsFinished);
			yield break;
		}

		private IEnumerator testerCoroutine()
		{
			yield return null;
			yield return null;
		}

		private IEnumerator testerCoroutineEmptyEnumator()
		{
			yield break;
		}
	}
}
