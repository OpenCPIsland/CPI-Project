// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
    [Tooltip("Sets the velocity of a game object with a rigid body. To leave any axis unchanged, set variable to 'None'." +
             "\nIn most cases you should not modify the velocity directly, as this can result in unrealistic behaviour. " +
             "See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-velocity.html\">Rigidbody.velocity</a>.")]
    public class SetVelocity : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The Game Object with the RigidBody component.")]
        public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Set velocity using Vector3 variable and/or individual channels below.")]
        public FsmVector3 vector;

        [Tooltip("Velocity in X axis.")]
        public FsmFloat x;
        [Tooltip("Velocity in Y axis.")]
        public FsmFloat y;
        [Tooltip("Velocity in Z axis.")]
        public FsmFloat z;

        [Tooltip("You can set velocity in world or local space.")]
        public Space space;

        [Tooltip("Set the velocity every frame.")]
        public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };
			z = new FsmFloat { UseVariable = true };
			space = Space.Self;
			everyFrame = false;
		}

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = true;
        }		

		// TODO: test this works in OnEnter!
		public override void OnEnter()
		{
			DoSetVelocity();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			DoSetVelocity();
			
		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		
		public override void OnFixedUpdate()
		{
			DoSetVelocity();
		
		    if (!everyFrame)
		    {
		        Finish();
		    }
		}

        private void DoSetVelocity()
		{
            if (!UpdateCacheAndTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				return;
			}
			
			// init velocity
			
			Vector3 velocity;

			if (vector.IsNone)
			{
				velocity = space == Space.World ?
					rigidbody.velocity : 
					cachedTransform.InverseTransformDirection(rigidbody.velocity);
			}
			else
			{
				velocity = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) velocity.x = x.Value;
			if (!y.IsNone) velocity.y = y.Value;
			if (!z.IsNone) velocity.z = z.Value;

			// apply
			
			rigidbody.velocity = space == Space.World ? velocity : cachedTransform.TransformDirection(velocity);
		}
	}
}