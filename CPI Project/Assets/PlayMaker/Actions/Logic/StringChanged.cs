// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if the value of a string variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class StringChanged : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("The String Variable to test.")]
        public FsmString stringVariable;
        [Tooltip("Event to send if changed.")]
        public FsmEvent changedEvent;
		[UIHint(UIHint.Variable)]
        [Tooltip("Set to True if changed, otherwise False.")]
        public FsmBool storeResult;
		
		string previousValue;

		public override void Reset()
		{
			stringVariable = null;
			changedEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			if (stringVariable.IsNone)
			{
				Finish();
				return;
			}
			
			previousValue = stringVariable.Value;
		}
		
		public override void OnUpdate()
		{
			if (stringVariable.Value != previousValue)
			{
				storeResult.Value = true;
				Fsm.Event(changedEvent);
			}
		}
	}
}

