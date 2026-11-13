// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Controls whether 2D physics affects the Game Object.")]
    public class SetIsKinematic2d : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The isKinematic value")]
		public FsmBool isKinematic;
		
		public override void Reset()
		{
			gameObject = null;
			isKinematic = false;
		}
		
		public override void OnEnter()
		{
			DoSetIsKinematic();
			Finish();
		}
		
		void DoSetIsKinematic()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (!UpdateCache(go))
            {
                return;
            }

#if UNITY_6000_0_OR_NEWER
            rigidbody2d.bodyType = isKinematic.Value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
#else
			rigidbody2d.isKinematic = isKinematic.Value;
#endif
        }
    }
}

