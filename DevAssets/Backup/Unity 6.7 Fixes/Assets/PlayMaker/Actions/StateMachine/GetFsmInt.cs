// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName")]
	[Tooltip("Get the value of an Integer Variable from another FSM.")]
	public class GetFsmInt : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;
		
        [RequiredField]
		[UIHint(UIHint.FsmInt)]
        [Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;
		
        [RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the value in an Int variable in this FSM.")]
		public FsmInt storeValue;
		
        [Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;

        private GameObject goLastFrame;
        private string fsmNameLastFrame;
        private PlayMakerFSM fsm;
		
		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
			storeValue = null;
		}

		public override void OnEnter()
		{
			DoGetFsmInt();

			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoGetFsmInt();
		}

        private void DoGetFsmInt()
		{
			if (storeValue == null) return;

			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;

            if (go != goLastFrame || fsmName.Value != fsmNameLastFrame)
            {
                goLastFrame = go;
                fsmNameLastFrame = fsmName.Value;
                // only get the fsm component if go or fsm name has changed
				fsm = ActionHelpers.GetGameObjectFsm(go, fsmName.Value);
			}
			
			if (fsm == null) return;
			
			FsmInt fsmInt = fsm.FsmVariables.GetFsmInt(variableName.Value);
			
			if (fsmInt == null) return;
			
			storeValue.Value = fsmInt.Value;
		}

#if UNITY_EDITOR

        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, variableName);
        }

#endif
    }
}