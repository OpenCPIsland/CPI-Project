// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Tests if a rigid body is controlled by physics. " +
             "See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-isKinematic.html\">IsKinematic</a>.")]
    public class IsKinematic : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The game object to test.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event sent if it is kinematic (not controlled by physics).")]
        public FsmEvent trueEvent;

        [Tooltip("Event sent if it is not kinematic (controlled by physics).")]
        public FsmEvent falseEvent;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a Bool Variable")]
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
			DoIsKinematic();
			
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoIsKinematic();
		}

		void DoIsKinematic()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(go))
			{
                var isKinematic = rigidbody.isKinematic;
                store.Value = isKinematic;
                Fsm.Event(isKinematic ? trueEvent : falseEvent);
			}
		}
	}
}

