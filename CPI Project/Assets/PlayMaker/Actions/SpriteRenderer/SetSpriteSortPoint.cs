// (c) Copyright HutongGames, LLC 2010-2019. All rights reserved.


using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Determines the position of the Sprite used for sorting the Renderer. Unity 2018.2 or higher.")]
	public class SetSpriteSortPoint : ComponentAction<SpriteRenderer>
    {
#if UNITY_2018_2_OR_NEWER

        [RequiredField]
        [CheckForComponent(typeof(SpriteRenderer))]
        [Tooltip("The GameObject with the SpriteRenderer component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Set the Sprite Sorting Point value")]
        [ObjectType(typeof(SpriteSortPoint))]
        public FsmEnum spriteSortPoint;

		public override void Reset()
		{
			gameObject = null;
            spriteSortPoint = new FsmEnum() { UseVariable = true };
        }

		public override void OnEnter()
		{


            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                return;
            }

            cachedComponent.spriteSortPoint = (SpriteSortPoint)spriteSortPoint.Value;

            Finish();
        }
#endif

    }
}