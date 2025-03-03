// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition : ComponentAction<Transform>
    {
		[RequiredField]
		[Tooltip("The Game Object to position.")]
		public FsmOwnerDefault gameObject;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
		public FsmVector3 vector;

        [Tooltip("Set the X position.")]
        public FsmFloat x;
        [Tooltip("Set the Y position.")]
        public FsmFloat y;
        [Tooltip("Set the Z position.")]
        public FsmFloat z;

        [Tooltip("Set position in local (relative to parent) or world space.")]
        public Space space;

        [Tooltip("Perform this action every frame. Useful if position is changing.")]
        public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

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
			lateUpdate = false;
		}

        public override void OnPreprocess()
        {
            if (lateUpdate)
            {
                Fsm.HandleLateUpdate = true;
            }
        }

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate)
			{
				DoSetPosition();
				Finish();
			}		
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoSetPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoSetPosition();
			}

			if (!everyFrame)
			{
				Finish();
			}
		}

        private void DoSetPosition()
		{
            if (!UpdateCachedTransform(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            // init position

            Vector3 position;

			if (vector.IsNone)
			{
				position = space == Space.World ? cachedTransform.position : cachedTransform.localPosition;
			}
			else
			{
				position = vector.Value;
			}
			
			// override any axis

			if (!x.IsNone) position.x = x.Value;
			if (!y.IsNone) position.y = y.Value;
			if (!z.IsNone) position.z = z.Value;

			// apply
			
			if (space == Space.World)
			{
                cachedTransform.position = position;
			}
			else
			{
                cachedTransform.localPosition = position;
			}
		}

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName(this, Fsm, gameObject);
        }
#endif
    }
}