// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the GameObject mapped to this human bone id")]
	public class GetAnimatorBoneGameObject : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
        [Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The bone reference")]
		[ObjectType(typeof(HumanBodyBones))]
		public FsmEnum bone;

		[ActionSection("Results")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bone's GameObject")]
		public FsmGameObject boneGameObject;

		public override void Reset()
		{
			gameObject = null;
			bone = HumanBodyBones.Hips;
			boneGameObject = null;
		}

        public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                boneGameObject.Value = cachedComponent.GetBoneTransform((HumanBodyBones)bone.Value).gameObject;
            }

            Finish();
        }

    }
}