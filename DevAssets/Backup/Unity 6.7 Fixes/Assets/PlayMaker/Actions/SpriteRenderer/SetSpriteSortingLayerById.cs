// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
    [Tooltip("Set the Sorting Layer of a SpriteRenderer component by Id (by id is faster than by name)")]
    public class SetSpriteSortingLayerById : ComponentAction<SpriteRenderer>
    {
        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The sorting Layer Name")]
        public FsmInt sortingLayerId;

        [Tooltip("If true, set the sorting layer to all children")]
        public FsmBool setAllSpritesInChildren;

        public override void Reset()
        {
            gameObject = null;
            sortingLayerId = null;
            setAllSpritesInChildren = false;
        }

        public override void OnEnter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            if (setAllSpritesInChildren.Value)
            {
                SpriteRenderer[] sprites = cachedComponent.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer _sprite in sprites)
                {
                    _sprite.sortingLayerID = sortingLayerId.Value;
                }
            }
            else
            {
                cachedComponent.sortingLayerID = sortingLayerId.Value;
            }

            Finish();
        }
    }
}
