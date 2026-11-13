// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Tests if a rigid body is sleeping. " +
             "See Unity docs: <a href=\"http://unity3d.com/support/documentation/Components/RigidbodySleeping.html\">Rigidbody Sleeping</a>.")]
    public class IsSleeping : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The game object to test.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event to send if sleeping.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if not sleeping.")]
        public FsmEvent falseEvent;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool Variable.")]
        public FsmBool store;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			trueEvent = null;
			falseEvent = null;
			store = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoIsSleeping();
			
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoIsSleeping();
		}

		void DoIsSleeping()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
                var isSleeping = rigidbody.IsSleeping();
                store.Value = isSleeping;
                Fsm.Event(isSleeping ? trueEvent : falseEvent);
			}
		}
	}
}

