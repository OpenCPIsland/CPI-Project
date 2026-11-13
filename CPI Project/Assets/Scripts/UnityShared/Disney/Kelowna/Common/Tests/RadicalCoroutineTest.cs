using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class RadicalCoroutineTest : MonoBehaviour
	{
		private int frameCount;

		private bool waitForSecondsTestComplete;

		private bool waitForFrameTestComplete;

		private bool waitForCompositeReturnComplete;

		private bool waitForFrameCancelled;

		private void Start()
		{
			StartCoroutine(runTests());
		}

		private IEnumerator runTests()
		{
			frameCount = 0;
			yield return RadicalCoroutine.Run(this, waitForSecondsTest(), "waitForSecondsTest");
			frameCount = 0;
			yield return RadicalCoroutine.Run(this, waitForNextFrameTest(), "waitForNextFrameTest");
			frameCount = 0;
			yield return RadicalCoroutine.Run(this, waitForFrameTest(), "waitForFrameTest");
			frameCount = 0;
			yield return RadicalCoroutine.Run(this, waitForCompositeReturnTest(), "waitForCompositeReturn");
			yield return RadicalCoroutine.Run(this, waitForFrameCancelledTest(), "waitForFrameCancelledTest");
			yield return RadicalCoroutine.Run(this, waitForDisposedEventTest(), "WaitForDisposedEventTest");
			yield return RadicalCoroutine.Run(this, waitForCompletedEventTest(), "waitForCompletedEventTest");
			yield return RadicalCoroutine.Run(this, waitForPausedEventTest(), "waitForPausedEventTest");
			yield return RadicalCoroutine.Run(this, waitForResumedEventTest(), "waitForResumedEventTest");
			yield return RadicalCoroutine.Run(this, cannotAddListenerToEmptyEnumerator(), "cannotAddListenerToEmptyEnumerator");
			yield return RadicalCoroutine.Run(this, waitForNestedIEnumeratorTest(), "waitForNestedIEnumeratorTest");
			IntegrationTest.Pass();
		}

		private IEnumerator waitForNextFrameTest()
		{
			yield return null;
			IntegrationTestEx.FailIf(frameCount != 1, "Coroutine did not wait exactly one frame.");
		}

		private IEnumerator waitForSecondsTest()
		{
			yield return new WaitForSeconds(0.01f);
			IntegrationTestEx.FailIf(frameCount == 0, "Coroutine returned immediately");
		}

		private IEnumerator waitForFrameTest()
		{
			yield return new WaitForFrame(2);
			IntegrationTestEx.FailIf(frameCount != 2, "WaitForFrame waited for the incorrect number of frames");
		}

		private IEnumerator waitForCompositeReturnTest()
		{
			yield return new CompositeCoroutineReturn(new WaitForFrame(2), new WaitForFrame(3));
			if (frameCount != 3)
			{
				IntegrationTest.Fail("WaitForFrame (x2) waited for the incorrect number of frames.");
			}
			IntegrationTestEx.FailIf(frameCount != 3, "WaitForFrame (x2) waited for the incorrect number of frames.");
		}

		private IEnumerator waitForFrameCancelledTest()
		{
			RadicalCoroutine coroutine = RadicalCoroutine.Create(testCoroutine(), "testCoroutine");
			StartCoroutine(coroutine.Enumerator);
			coroutine.ECancelled += delegate
			{
				waitForFrameCancelled = true;
			};
			coroutine.Cancel();
			while (!coroutine.Disposed)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!waitForFrameCancelled, "ECancelled was not dispatched for cancelled coroutine.");
		}

		private IEnumerator waitForDisposedEventTest()
		{
			bool disposed = false;
			RadicalCoroutine coroutine = RadicalCoroutine.Create(testCoroutine(), "testCoroutine");
			coroutine.EDisposed += delegate
			{
				disposed = true;
			};
			StartCoroutine(coroutine.Enumerator);
			yield return coroutine;
			IntegrationTestEx.FailIf(coroutine.Cancelled);
			IntegrationTestEx.FailIf(!disposed, "coroutine did not dispatch EDisposed after completion.");
		}

		private IEnumerator waitForCompletedEventTest()
		{
			bool completed = false;
			RadicalCoroutine coroutine = RadicalCoroutine.Create(testCoroutine(), "testCoroutine");
			coroutine.ECompleted += delegate
			{
				completed = true;
			};
			StartCoroutine(coroutine.Enumerator);
			yield return coroutine;
			IntegrationTestEx.FailIf(!completed, "coroutine did not dispatch ECompleted after completion.");
			IntegrationTestEx.FailIf(coroutine.Cancelled);
		}

		private IEnumerator waitForPausedEventTest()
		{
			bool paused = false;
			RadicalCoroutine coroutine = RadicalCoroutine.Create(testCoroutine(), "testCoroutine");
			coroutine.EPaused += delegate
			{
				paused = true;
			};
			coroutine.Pause();
			StartCoroutine(coroutine.Enumerator);
			yield return new WaitForFrame(5);
			IntegrationTestEx.FailIf(!paused, "coroutine did not dispatch EPaused after within 5 frames.");
			IntegrationTestEx.FailIf(!coroutine.Paused);
			IntegrationTestEx.FailIf(coroutine.Disposed);
			IntegrationTestEx.FailIf(coroutine.Completed);
			IntegrationTestEx.FailIf(coroutine.Cancelled);
		}

		private IEnumerator waitForResumedEventTest()
		{
			bool resumed = false;
			RadicalCoroutine coroutine = RadicalCoroutine.Create(testCoroutine(), "testCoroutine");
			coroutine.EResumed += delegate
			{
				resumed = true;
			};
			coroutine.Pause();
			StartCoroutine(coroutine.Enumerator);
			yield return null;
			IntegrationTestEx.FailIf(resumed);
			coroutine.Resume();
			yield return null;
			IntegrationTestEx.FailIf(!resumed, "coroutine did not dispatch EResumed one frame after resumed");
		}

		private IEnumerator cannotAddListenerToEmptyEnumerator()
		{
			RadicalCoroutine radicalCoroutine = RadicalCoroutine.Create(testEmptyCoroutine(), "testEmptyCoroutine");
			StartCoroutine(radicalCoroutine.Enumerator);
			bool flag = false;
			try
			{
				radicalCoroutine.ECompleted += delegate
				{
				};
			}
			catch (InvalidOperationException)
			{
				flag = true;
			}
			IntegrationTestEx.FailIf(!flag);
			IntegrationTestEx.FailIf(!radicalCoroutine.Disposed);
			yield break;
		}

		private IEnumerator waitForNestedIEnumeratorTest()
		{
			frameCount = 0;
			yield return RadicalCoroutine.Run(this, parentCoroutine(parentCoroutine(new CompositeCoroutineReturn(new WaitForFrame(4)))), "testCoroutine");
			IntegrationTestEx.FailIf(frameCount != 4, "waitForNestedIEnumeratorTest waited for the incorrect number of frames");
		}

		private IEnumerator testCoroutine()
		{
			yield return null;
			yield return null;
			yield return null;
		}

		private IEnumerator testEmptyCoroutine()
		{
			yield break;
		}

		private IEnumerator parentCoroutine(IEnumerator childCoroutine)
		{
			yield return childCoroutine;
		}

		private IEnumerator parentCoroutine(CoroutineReturn childCoroutine)
		{
			yield return childCoroutine;
		}

		private void Update()
		{
			frameCount++;
		}
	}
}
