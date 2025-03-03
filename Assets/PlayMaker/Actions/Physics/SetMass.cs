// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Sets the mass of a game object's rigid body. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-mass.html\">Rigidbody.Mass</a>")]

    public class SetMass : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("A GameObject with a RigidBody component.")]
        public FsmOwnerDefault gameObject;

		[RequiredField]
		[HasFloatSlider(0.1f,10f)]
        [Tooltip("Set the mass. Unity recommends a mass between 0.1 and 10.")]
        public FsmFloat mass;

		public override void Reset()
		{
			gameObject = null;
			mass = 1;
		}

		public override void OnEnter()
		{
			DoSetMass();
			
			Finish();
		}

		void DoSetMass()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (UpdateCache(go))
		    {
		        rigidbody.mass = mass.Value;
		    }
		}
	}
}