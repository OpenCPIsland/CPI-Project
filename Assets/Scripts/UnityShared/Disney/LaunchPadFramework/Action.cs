using System;
using System.Collections;

namespace Disney.LaunchPadFramework
{
	public abstract class Action : IAction
	{
		public EventDispatcher mEventDispatcher = null;

		private ActionState mState = ActionState.WAITING;

		public string ActionName
		{
			get
			{
				return GetType().ToString();
			}
		}

		public ActionState State
		{
			get
			{
				return mState;
			}
			set
			{
				mState = value;
			}
		}

		public EventDispatcher EventDispatcher
		{
			get
			{
				return mEventDispatcher;
			}
			set
			{
				mEventDispatcher = value;
			}
		}

		public virtual bool completed
		{
			get
			{
				return State == ActionState.COMPLETE;
			}
		}

		public virtual void Begin()
		{
			if (!CanBegin())
			{
				Log.LogFatal(this, "Cannot start Action: invalid state");
				throw new ApplicationException("[InitAction] Invalid state");
			}
			State = ActionState.RUNNING;
			EventDispatcher.DispatchEvent(new ActionStartEvent(this));
		}

		public virtual bool CanBegin()
		{
			return State == ActionState.WAITING;
		}

		public abstract IEnumerator Perform();

		public virtual void OnComplete()
		{
			mState = ActionState.COMPLETE;
			mEventDispatcher.DispatchEvent(new ActionCompleteEvent(this));
		}

		public IEnumerator WaitTillComplete()
		{
			while (mState != ActionState.COMPLETE)
			{
				yield return null;
				if (mState == ActionState.COMPLETE)
				{
					break;
				}
			}
		}
	}
}
