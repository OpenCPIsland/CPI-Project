#define ENABLE_PROFILER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Disney.Kelowna.Common
{
	public class RadicalCoroutine : ICoroutine
	{
		private Stopwatch Timer;

		private RadicalCoroutine nestedRoutine;

		public IEnumerator Enumerator;

		private Action eCancelled;

		private Action eCompleted;

		private Action ePaused;

		private Action eResumed;

		private Action eDisposed;

		private object lockObj = new object();

		public Coroutine unityCoroutine;

		public MonoBehaviour unityOwner;

		private string trackedName;

		private static readonly List<string> activeCoroutineStrings = new List<string>();

		public TimeSpan Duration
		{
			get
			{
				return new TimeSpan(Timer.ElapsedTicks);
			}
		}

		public bool Cancelled
		{
			get;
			private set;
		}

		public bool Completed
		{
			get;
			private set;
		}

		public bool Paused
		{
			get;
			private set;
		}

		public bool Disposed
		{
			get;
			private set;
		}

		public string TrackedName
		{
			get
			{
				return trackedName;
			}
		}

		public event Action ECancelled
		{
			add
			{
				lock (lockObj)
				{
					checkCanAddListener();
					eCancelled = (Action)Delegate.Combine(eCancelled, value);
				}
			}
			remove
			{
				lock (lockObj)
				{
					eCancelled = (Action)Delegate.Remove(eCancelled, value);
				}
			}
		}

		public event Action ECompleted
		{
			add
			{
				lock (lockObj)
				{
					checkCanAddListener();
					eCompleted = (Action)Delegate.Combine(eCompleted, value);
				}
			}
			remove
			{
				lock (lockObj)
				{
					eCompleted = (Action)Delegate.Remove(eCompleted, value);
				}
			}
		}

		public event Action EPaused
		{
			add
			{
				lock (lockObj)
				{
					checkCanAddListener();
					ePaused = (Action)Delegate.Combine(ePaused, value);
				}
			}
			remove
			{
				lock (lockObj)
				{
					ePaused = (Action)Delegate.Remove(ePaused, value);
				}
			}
		}

		public event Action EResumed
		{
			add
			{
				lock (lockObj)
				{
					checkCanAddListener();
					eResumed = (Action)Delegate.Combine(eResumed, value);
				}
			}
			remove
			{
				lock (lockObj)
				{
					eResumed = (Action)Delegate.Remove(eResumed, value);
				}
			}
		}

		public event Action EDisposed
		{
			add
			{
				lock (lockObj)
				{
					checkCanAddListener();
					eDisposed = (Action)Delegate.Combine(eDisposed, value);
				}
			}
			remove
			{
				lock (lockObj)
				{
					eDisposed = (Action)Delegate.Remove(eDisposed, value);
				}
			}
		}

		private void checkCanAddListener()
		{
			if (Disposed)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Cannot add event listeners to coroutine after it has been disposed. ");
				stringBuilder.Append("For Coroutines that complete immediately becasue they returns an empty enumerator, you must check the 'Disposed' property before adding listeners. ");
				stringBuilder.Append("debugName = " + trackedName);
				throw new InvalidOperationException(stringBuilder.ToString());
			}
		}

		public void Cancel()
		{
			if (nestedRoutine != null)
			{
				nestedRoutine.Cancel();
			}
			if (Completed || Cancelled)
			{
				throw new InvalidOperationException("Cannot cancell a coroutine that is cancelled or completed. debugName: " + trackedName);
			}
			Cancelled = true;
			Paused = false;
		}

		public void Pause()
		{
			if (nestedRoutine != null)
			{
				nestedRoutine.Pause();
			}
			if (Completed || Cancelled)
			{
				throw new InvalidOperationException("Cannot pause a coroutine that is cancelled or completed. debugName: " + trackedName);
			}
			Paused = true;
			Timer.Stop();
		}

		public void Resume()
		{
			if (nestedRoutine != null)
			{
				nestedRoutine.Resume();
			}
			if (Completed || Cancelled)
			{
				throw new InvalidOperationException("Cannot resume a coroutine that is cancelled or completed. debugName: " + trackedName);
			}
			Paused = false;
			Timer.Start();
		}

		private void clearListeners()
		{
			eCompleted = null;
			eCancelled = null;
			ePaused = null;
			eResumed = null;
			eDisposed = null;
		}

		public void Stop()
		{
			if (nestedRoutine != null)
			{
				nestedRoutine.Stop();
			}
			/*if (Disposed)
			{
				throw new InvalidOperationException("Cannot Stop a coroutine that is already disposed. debugName: " + trackedName);
			}*/
			dispose();
			if (unityOwner != null && unityCoroutine != null)
			{
				unityOwner.StopCoroutine(unityCoroutine);
			}
		}

		public static Coroutine Run(MonoBehaviour owner, IEnumerator extendedCoRoutine, string debugName)
		{
			RadicalCoroutine radicalCoroutine = new RadicalCoroutine();
			radicalCoroutine.Enumerator = radicalCoroutine.Execute(extendedCoRoutine, debugName);
			radicalCoroutine.unityOwner = owner;
			radicalCoroutine.unityCoroutine = owner.StartCoroutine(radicalCoroutine.Enumerator);
			return radicalCoroutine.unityCoroutine;
		}

		public static RadicalCoroutine Create(IEnumerator extendedCoRoutine, string debugName)
		{
			RadicalCoroutine radicalCoroutine = new RadicalCoroutine();
			radicalCoroutine.Enumerator = radicalCoroutine.Execute(extendedCoRoutine, debugName);
			return radicalCoroutine;
		}

		private IEnumerator Execute(IEnumerator extendedCoroutine, string debugName)
		{
			trackedName = debugName;
			Timer = Stopwatch.StartNew();
			if (Disposed)
			{
				yield break;
			}
			while (!Cancelled && extendedCoroutine != null)
			{
				Profiler.BeginSample(trackedName);
				bool res = extendedCoroutine.MoveNext();
				Profiler.EndSample();
				if (!res)
				{
					break;
				}
				bool cancelledWhilePaused = false;
				if (Paused)
				{
					if (ePaused != null)
					{
						ePaused();
					}
					while (Paused)
					{
						if (Cancelled)
						{
							cancelledWhilePaused = true;
							break;
						}
						yield return null;
					}
					if (eResumed != null && !Cancelled)
					{
						eResumed();
					}
				}
				if (cancelledWhilePaused)
				{
					break;
				}
				object v = extendedCoroutine.Current;
				CoroutineReturn cr = v as CoroutineReturn;
				ICoroutine c = v as ICoroutine;
				IEnumerator e = v as IEnumerator;
				if (cr != null)
				{
					while (!cr.Finished)
					{
						yield return null;
					}
				}
				else if (c != null)
				{
					while (!c.Disposed)
					{
						yield return null;
					}
				}
				else if (e != null)
				{
					nestedRoutine = Create(e, debugName + "+");
					nestedRoutine.unityOwner = unityOwner;
					nestedRoutine.unityCoroutine = unityOwner.StartCoroutine(nestedRoutine.Enumerator);
					yield return nestedRoutine.unityCoroutine;
					nestedRoutine = null;
				}
				else
				{
					yield return v;
				}
			}
			if (Disposed)
			{
				yield break;
			}
			if (Cancelled)
			{
				if (eCancelled != null)
				{
					eCancelled();
				}
			}
			else
			{
				Completed = true;
				if (eCompleted != null)
				{
					eCompleted();
				}
			}
			dispose();
		}

		private void dispose()
		{
			Timer.Stop();
			Disposed = true;
			if (eDisposed != null)
			{
				eDisposed();
			}
			clearListeners();
		}

		[Conditional("CI_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[Conditional("QA_BUILD")]
		private static void trackCoroutine(string debugName)
		{
			activeCoroutineStrings.Add(debugName);
		}

		[Conditional("QA_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[Conditional("CI_BUILD")]
		private static void untrackCoroutine(string debugName)
		{
			activeCoroutineStrings.Remove(debugName);
		}
	}
}
