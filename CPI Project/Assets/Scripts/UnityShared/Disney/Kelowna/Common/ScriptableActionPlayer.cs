using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ScriptableActionPlayer : MonoBehaviour
	{
		public ScriptableAction Action;

		public bool DestroyOnComplete = true;

		private bool _isPaused;

		private readonly List<Action> finalizers = new List<Action>();

		public ScriptableAction NextAction
		{
			get;
			set;
		}

		public bool AbortAction
		{
			get;
			set;
		}

		public bool ActionIsFinished
		{
			get;
			set;
		}

		public bool IsPaused
		{
			get
			{
				return _isPaused;
			}
			set
			{
				if (_isPaused != value)
				{
					_isPaused = value;
					if (_isPaused)
					{
						onPlayerPaused();
					}
					else
					{
						onPlayerResumed();
					}
				}
			}
		}

		public void Awake()
		{
			onAwake();
			if (Action == null)
			{
				base.enabled = false;
			}
		}

		protected virtual void onAwake()
		{
		}

		protected virtual void onActionStart()
		{
		}

		protected virtual void onActionDone()
		{
		}

		protected virtual void onPlayerStart()
		{
		}

		protected virtual void onPlayerDone()
		{
		}

		protected virtual void onPlayerPaused()
		{
		}

		protected virtual void onPlayerResumed()
		{
		}

		public IEnumerator Start()
		{
			onPlayerStart();
			while (Action != null)
			{
				AbortAction = false;
				ActionIsFinished = false;
				onActionStart();
				while (IsPaused)
				{
					yield return null;
				}
				IEnumerator enumerator = Action.Execute(this);
				while (enumerator.MoveNext())
				{
					while (IsPaused)
					{
						yield return null;
					}
					object current = enumerator.Current;
					if (!(current is IEnumerator))
					{
						yield return current;
					}
				}
				onActionDone();
				Action = NextAction;
				NextAction = null;
			}
			onPlayerDone();
			base.enabled = false;
			for (int num = finalizers.Count - 1; num >= 0; num--)
			{
				finalizers[num]();
			}
			if (DestroyOnComplete)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		public void AddFinalizer(Action finalizer)
		{
			finalizers.Add(finalizer);
		}
	}
}
