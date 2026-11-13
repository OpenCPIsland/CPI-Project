// (c) Copyright HutongGames. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName")]
	[Tooltip("Tests if an FSM has a variable with the given name.")]
	public class FsmHasVariable : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		[UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;
		
        [RequiredField]
        [Tooltip("Check to see if the FSM has this variable.")]
        public FsmString variableName;

        [Tooltip("Event to send if the FSM has the variable.")]
		public FsmEvent trueEvent;

        [Tooltip("Event to send if the FSM does NOT have the variable.")]
		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test.")]
		public FsmBool storeResult;

        [Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;
		
		// store game object last frame so we know when it's changed
		// and have to cache a new fsm
		private GameObject previousGo;
		
		// cache the fsm component since that's an expensive operation
        private PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			fsmName = null;
			variableName = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoFsmVariableTest();
			
			if (!everyFrame)
			{
			    Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoFsmVariableTest();
		}	
		
		void DoFsmVariableTest()
		{
			var go = gameObject.Value;
			if (go == null) return;
			
			if (go != previousGo)
			{
				fsm = ActionHelpers.GetGameObjectFsm(go, fsmName.Value);
				previousGo = go;
			}
			
			if (fsm == null)
			{
			    return;
			}
			
			var hasVariable = false;
			
			if (fsm.FsmVariables.Contains(variableName.Value))
			{
				Fsm.Event(trueEvent);
				hasVariable = true;
			}
			else 
			{
				Fsm.Event(falseEvent);
			}

			storeResult.Value = hasVariable;
		}


	}
}