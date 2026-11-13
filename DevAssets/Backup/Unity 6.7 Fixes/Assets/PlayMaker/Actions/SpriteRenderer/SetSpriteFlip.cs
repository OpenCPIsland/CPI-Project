// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Sets the Flips values of a of a SpriteRenderer component.")]
    public class SetSpriteFlip : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The X Flip value")]
		public FsmBool x;

		[Tooltip("The Y Flip value")]
		public FsmBool y;

		[Tooltip("Reset flip values when state exits")]
		public FsmBool resetOnExit;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		bool x_orig;
		bool y_orig;

		public override void Reset()
		{
			gameObject = null;
			x = null;
			y = null;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            if (resetOnExit.Value)
			{
				x_orig = cachedComponent.flipX;
				y_orig = cachedComponent.flipY;
			}

			FlipSprites();

            if (!everyFrame)
            {
                Finish();
            }
		}

        public override void OnUpdate()
        {
            FlipSprites();
        }

        public override void OnExit()
		{
			if (resetOnExit.Value)
			{
                cachedComponent.flipX = x_orig;
                cachedComponent.flipY = y_orig;
			}
		}

		void FlipSprites()
		{
			if (!x.IsNone) cachedComponent.flipX = x.Value;
            if (!y.IsNone) cachedComponent.flipY = y.Value;
		}
	}
}
