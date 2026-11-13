using DisneyMobile.CoreUnitySystems.Utility;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class LoadingTransition : Transition
	{
		private StateChangeArgs mStateChangeDetails = null;

		public override void Perform(StateChangeArgs stateChangeDetails)
		{
			mStateChangeDetails = stateChangeDetails;
			mStateChangeDetails.StartState.RaisePreExitEvent(stateChangeDetails);
			mStateChangeDetails.StartState.RaiseExitEvent(stateChangeDetails);
			mStateChangeDetails.StartState.RaisePostExitEvent(stateChangeDetails);
			InitializerComponent safeComponent = base.gameObject.GetSafeComponent<InitializerComponent>();
			if (safeComponent != null)
			{
				Logger.LogDebug(this, "LoadingTransition beginning initialization...", Logger.TagFlags.INIT);
				safeComponent.EventDispatcher.AddListener<InitCompleteEvent>(OnInitComplete);
				safeComponent.Initialize();
			}
			else
			{
				FinishTransition();
			}
		}

		public override void Reset()
		{
			mStateChangeDetails = null;
			base.EventDispatcher.ClearAll();
		}

		protected virtual bool OnInitComplete(InitCompleteEvent evt)
		{
			Logger.LogDebug(this, "LoadingTransition completed initialization", Logger.TagFlags.INIT);
			FinishTransition();
			RaiseTransitionCompletedEvent();
			return false;
		}

		protected void FinishTransition()
		{
			mStateChangeDetails.EndState.RaisePreEnterEvent(mStateChangeDetails);
			mStateChangeDetails.EndState.RaiseEnterEvent(mStateChangeDetails);
			mStateChangeDetails.EndState.RaisePostEnterEvent(mStateChangeDetails);
		}
	}
}
