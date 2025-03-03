// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Forces a rigid body to sleep at least one frame. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.Sleep.html\">Rigidbody.sleep</a>.")]

    public class Sleep : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("A Game Object with a Rigid Body.")]
        public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			DoSleep();
			Finish();
		}

		void DoSleep()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
		        rigidbody.Sleep();
		    }
		}
	}
}