// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets the Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
	public class GetVelocity : ComponentAction<Rigidbody>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
        [Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the velocity in a Vector3 Variable.")]
		public FsmVector3 vector;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the X component of the velocity in a Float Variable.")]
		public FsmFloat x;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Y component of the velocity in a Float Variable.")]
		public FsmFloat y;
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Z component of the velocity in a Float Variable.")]
		public FsmFloat z;

        [Tooltip("The coordinate space to get the velocity in.")]
        public Space space;

        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = null;
			y = null;
			z = null;
			space = Space.World;
			everyFrame = false;
		}
		
		public override void OnPreprocess()
		{
			Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			DoGetVelocity();

		    if (!everyFrame)
		    {
		        Finish();
		    }		
		}

		public override void OnUpdate()
		{
			DoGetVelocity();
	
		    if (!everyFrame)
		    {
		        Finish();
		    }
		}

		public override void OnFixedUpdate()
		{
			DoGetVelocity();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}

		void DoGetVelocity()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (!UpdateCache(go))
		    {
		        return;
		    }

			var velocity = rigidbody.velocity;
		    if (space == Space.Self)
		    {
		        velocity = go.transform.InverseTransformDirection(velocity);
		    }
			
			vector.Value = velocity;
			x.Value = velocity.x;
			y.Value = velocity.y;
			z.Value = velocity.z;
		}


	}
}