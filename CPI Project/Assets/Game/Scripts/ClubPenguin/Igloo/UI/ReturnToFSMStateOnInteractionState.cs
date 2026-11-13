using ClubPenguin.ObjectManipulation.Input;
using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(ObjectManipulationInputController))]
	public class ReturnToFSMStateOnInteractionState : MonoBehaviour
	{
		private ObjectManipulationInputController objManipulationController;

		private StateMachineContextListener stateMachineContextListener;

		private StateMachineContext stateMachineContext;

		private GameObject previousStateObject;

		private string state;

		private string evt;

		private void Awake()
		{
			objManipulationController = GetComponent<ObjectManipulationInputController>();
			stateMachineContextListener = GetComponent<StateMachineContextListener>();
			stateMachineContextListener.OnContextAdded += onContextAdded;
		}

		private void onContextAdded(StateMachineContext stateMachineContext)
		{
			this.stateMachineContext = stateMachineContext;
		}

		public void SetCurrentTargetAndState(string state, string evt)
		{
			this.state = state;
			this.evt = evt;
			objManipulationController.InteractionStateChanged += onInteractionStateChanged;
		}

		public void SetPreviousState(GameObject stateObject)
		{
			previousStateObject = stateObject;
		}

		private void onInteractionStateChanged(InteractionState interactionState)
		{
			switch (interactionState)
			{
			case InteractionState.NoSelectedItem:
			case InteractionState.ActiveSelectedItem:
				if (stateMachineContext != null && !string.IsNullOrEmpty(state))
				{
					stateMachineContext.SendEvent(new ExternalEvent(state, evt));
					objManipulationController.InteractionStateChanged -= onInteractionStateChanged;
				}
				if (previousStateObject != null)
				{
					previousStateObject.SetActive(true);
					previousStateObject = null;
				}
				objManipulationController.InteractionStateChanged -= onInteractionStateChanged;
				break;
			}
		}

		private void OnDestroy()
		{
			if (stateMachineContextListener != null)
			{
				stateMachineContextListener.OnContextAdded -= onContextAdded;
			}
			if (objManipulationController != null)
			{
				objManipulationController.InteractionStateChanged -= onInteractionStateChanged;
			}
		}
	}
}
