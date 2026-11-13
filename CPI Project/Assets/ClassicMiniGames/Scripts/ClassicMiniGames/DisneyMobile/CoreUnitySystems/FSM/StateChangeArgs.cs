using System;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateChangeArgs : EventArgs
	{
		public StateTraverser Traverser
		{
			get;
			set;
		}

		public State StartState
		{
			get;
			set;
		}

		public State EndState
		{
			get;
			set;
		}

		public Signal TriggeringSignal
		{
			get;
			set;
		}

		public Transition TransitionUsed
		{
			get;
			set;
		}

		public StateChangeArgs(StateTraverser traverser, State startState, State endState, Signal signal, Transition transition)
		{
			Traverser = traverser;
			StartState = startState;
			EndState = endState;
			TriggeringSignal = signal;
			TransitionUsed = transition;
		}
	}
}
