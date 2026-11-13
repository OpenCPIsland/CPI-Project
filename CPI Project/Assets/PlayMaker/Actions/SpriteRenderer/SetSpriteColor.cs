// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Sets the color of a sprite renderer")]
	public class SetSpriteColor : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;
		
		[Tooltip("Set the Color of the SpriteRenderer component")]
		public FsmColor color;

		[HasFloatSlider(0,1)]
		[Tooltip("Set the red channel")]
		public FsmFloat red;
		
		[HasFloatSlider(0,1)]
		[Tooltip("Set the green channel")]
		public FsmFloat green;
		
		[HasFloatSlider(0,1)]
		[Tooltip("Set the blue channel")]
		public FsmFloat blue;
		
		[HasFloatSlider(0,1)]
		[Tooltip("Set the alpha channel")]
		public FsmFloat alpha;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private Color originalColor;

        private Color newColor;

        public override void Reset()
		{
			gameObject = null;
            color = null;
            red = new FsmFloat(){UseVariable=true};
			green = new FsmFloat(){UseVariable=true};
			blue = new FsmFloat(){UseVariable=true};
			alpha = new FsmFloat(){UseVariable=true};
            resetOnExit = null;
            everyFrame =false;
		}
		
		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            originalColor = cachedComponent.color;

            SetColor();

            if (!everyFrame)
            {
                Finish();
            }
        }
		
		public override void OnUpdate()
		{
			SetColor();
		}
		
		void SetColor()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            newColor = cachedComponent.color;

            if (!color.IsNone) newColor = color.Value;

			if (!red.IsNone) newColor.r = red.Value;

			if (!green.IsNone) newColor.g = green.Value;
			
			if (!blue.IsNone) newColor.b = blue.Value;
			
			if (!alpha.IsNone) newColor.a = alpha.Value;

            cachedComponent.color = newColor;
		}

        public override void OnExit()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            if (resetOnExit.Value)
            {
                cachedComponent.color = originalColor;
            }
        }
    }
}
