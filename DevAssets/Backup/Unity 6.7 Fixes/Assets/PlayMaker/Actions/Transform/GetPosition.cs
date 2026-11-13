// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Position of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetPosition : ComponentAction<Transform>
	{
		[RequiredField]
        [Tooltip("The game object to examine.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the position in a Vector3 Variable.")]
		public FsmVector3 vector;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the X coordinate in a Float Variable.")]
		public FsmFloat x;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Y coordinate in a Float Variable.")]
		public FsmFloat y;
		
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the Z coordinate in a Float Variable.")]
		public FsmFloat z;

        [Tooltip("Use world or local coordinates.")]
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

		public override void OnEnter()
		{
			DoGetPosition();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			DoGetPosition();
		}

        private void DoGetPosition()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
				return;

			var position = space == Space.World ? cachedTransform.position : cachedTransform.localPosition;				
			
			vector.Value = position;
			x.Value = position.x;
			y.Value = position.y;
			z.Value = position.z;
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, Fsm, gameObject);
        }
#endif
    }
}