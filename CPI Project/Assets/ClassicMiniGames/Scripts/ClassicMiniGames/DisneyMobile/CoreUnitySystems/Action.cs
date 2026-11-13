using System;
using System.Collections;

namespace DisneyMobile.CoreUnitySystems
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

		public virtual void Start()
		{
			if (!CanStart())
			{
				Logger.LogFatal(this, "Cannot start Action: invalid state", Logger.TagFlags.INIT);
				throw new ApplicationException("[InitAction] Invalid state");
			}
			State = ActionState.RUNNING;
			EventDispatcher.DispatchEvent(new ActionStartEvent(this));
		}

		public virtual bool CanStart()
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
