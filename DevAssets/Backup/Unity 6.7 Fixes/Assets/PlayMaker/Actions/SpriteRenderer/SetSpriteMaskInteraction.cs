// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

#if UNITY_2017 || UNITY_2017_1_OR_NEWER

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the mode under which the sprite will interact with the masking system.")]
	public class SetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Set the Mask Interactions of the SpriteRenderer component.")]
        [ObjectType(typeof(SpriteMaskInteraction))]
        public FsmEnum spriteMaskInteraction;

		public override void Reset()
		{
			gameObject = null;
            spriteMaskInteraction = new FsmEnum() { UseVariable = true };
        }

		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            cachedComponent.maskInteraction = (SpriteMaskInteraction)spriteMaskInteraction.Value;

            Finish();
        }
	}
}

#endif