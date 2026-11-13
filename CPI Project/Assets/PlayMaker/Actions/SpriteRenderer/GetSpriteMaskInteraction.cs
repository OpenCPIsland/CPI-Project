// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

#if UNITY_2017 || UNITY_2017_1_OR_NEWER

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the mode under which the sprite will interact with the masking system.")]
	public class GetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Get the Mask Interactions of the SpriteRenderer component.")]
        [ObjectType(typeof(SpriteMaskInteraction))]
        [UIHint(UIHint.Variable)]
        public FsmEnum spriteMaskInteraction;

		public override void Reset()
		{
			gameObject = null;
            spriteMaskInteraction = null;
        }

		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }
            spriteMaskInteraction.Value = cachedComponent.maskInteraction;
            Finish();
        }
	}
}

#endif