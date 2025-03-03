using DisneyMobile.CoreUnitySystems.Utility;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateTraverser : MonoBehaviour, IStateTraverser
	{
		[SerializeField]
		private State mInitialState;

		private State mCurrentState = null;

		private Signal mActiveSignal = null;

		protected EventDispatcher mEventDispatcher = new EventDispatcher();

		public EventDispatcher EventDispatcher
		{
			get
			{
				return mEventDispatcher;
			}
		}

		public State CurrentState
		{
			get
			{
				return mCurrentState;
			}
		}

		public State InitialState
		{
			get
			{
				return mInitialState;
			}
			set
			{
				mInitialState = value;
			}
		}

		public bool IsTransitioning
		{
			get;
			set;
		}

		public void Awake()
		{
			base.gameObject.SetActive(false);
		}

		public void OnEnable()
		{
			if (mCurrentState != null)
			{
				mCurrentState.gameObject.SetActive(true);
			}
			else if (InitialState != null)
			{
				SetCurrentState(InitialState);
			}
		}

		public void OnDisable()
		{
			if (mCurrentState != null)
			{
				mCurrentState.gameObject.SetActive(false);
			}
		}

		public void SetCurrentState(State state)
		{
			State state2 = mCurrentState;
			mCurrentState = state;
			if (mCurrentState != state2)
			{
				Logger.LogInfo(this, string.Format("state traverser {0} setting state from {1} to {2}", base.name, state2.GetSafeName(), mCurrentState.GetSafeName()), Logger.TagFlags.FLOW);
				StateChangeArgs stateChangeDetails = new StateChangeArgs(this, state2, mCurrentState, null, null);
				if (state2 != null)
				{
					state2.RaisePreExitEvent(stateChangeDetails);
					state2.RaiseExitEvent(stateChangeDetails);
					state2.RaisePostExitEvent(stateChangeDetails);
				}
				if (mCurrentState != null)
				{
					mCurrentState.RaisePreEnterEvent(stateChangeDetails);
					mCurrentState.RaiseEnterEvent(stateChangeDetails);
					mCurrentState.RaisePostEnterEvent(stateChangeDetails);
				}
				RaiseStateChangedEvent();
			}
		}

		public void Update()
		{
			if (!(mCurrentState != null) || IsTransitioning)
			{
				return;
			}
			mCurrentState.RaisePreUpdateEvent();
			mCurrentState.RaiseUpdateEvent();
			mCurrentState.RaisePostUpdateEvent();
			mActiveSignal = mCurrentState.GetActiveSignal();
			if (mActiveSignal != null)
			{
				Logger.LogInfo(this, string.Format("{0} signal activated on state traverser {1} is transitioning from {2} to {3} using transition {4}", mActiveSignal.GetSafeName(), this.GetSafeName(), mCurrentState.GetSafeName(), mActiveSignal.EndState.GetSafeName(), mActiveSignal.Transition.GetSafeName()), Logger.TagFlags.FLOW);
				mCurrentState.ResetSignals();
				IsTransitioning = true;
				RaiseTransitionBeganEvent(mCurrentState, mActiveSignal.EndState, mActiveSignal);
				Transition transition = mActiveSignal.Transition;
				if (transition != null)
				{
					transition.EventDispatcher.AddListener<TransitionCompletedEvent>(OnCompletedTransition, EventDispatcher.Priority.LAST);
				}
				mActiveSignal.PerformTransition();
			}
		}

		protected bool OnCompletedTransition(TransitionCompletedEvent evnt)
		{
			if (mActiveSignal != null)
			{
				if (mActiveSignal.Transition != null)
				{
					mActiveSignal.Transition.EventDispatcher.RemoveListener<TransitionCompletedEvent>(OnCompletedTransition);
				}
				State previousState = mCurrentState;
				mCurrentState = mActiveSignal.EndState;
				IsTransitioning = false;
				RaiseTransitionEndedEvent(previousState, mCurrentState, mActiveSignal);
				RaiseStateChangedEvent();
				mActiveSignal = null;
			}
			Logger.LogInfo(this, string.Format("state traverser {0} finished transitioning to state: {1}", base.name, mCurrentState.GetSafeName()), Logger.TagFlags.FLOW);
			return false;
		}

		private void RaiseStateChangedEvent()
		{
			if (EventDispatcher != null)
			{
				EventDispatcher.DispatchEvent(new StateTraverserStateChangeEvent(mCurrentState));
			}
		}

		private void RaiseTransitionBeganEvent(State previousState, State newState, Signal signal)
		{
			if (EventDispatcher != null)
			{
				EventDispatcher.DispatchEvent(new StateTraverserTransitionBeganEvent(previousState, newState, signal));
			}
		}

		private void RaiseTransitionEndedEvent(State previousState, State newState, Signal signal)
		{
			if (EventDispatcher != null)
			{
				EventDispatcher.DispatchEvent(new StateTraverserTransitionEndedEvent(previousState, newState, signal));
			}
		}
	}
}
