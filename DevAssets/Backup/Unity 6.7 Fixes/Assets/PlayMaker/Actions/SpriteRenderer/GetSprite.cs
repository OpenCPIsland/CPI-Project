// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Gets the source image sprite of a SpriteRenderer component.")]
    public class GetSprite : ComponentAction<SpriteRenderer>
    {

        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The source sprite of the SpriteRenderer component.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(Sprite))]
        public FsmObject sprite;


        public override void Reset()
        {
            gameObject = null;
            sprite = null;
        }

        public override void OnEnter()
        {
            ExecuteAction();

            Finish();
        }

        void ExecuteAction()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }
            sprite.Value = cachedComponent.sprite;
        }
    }
}