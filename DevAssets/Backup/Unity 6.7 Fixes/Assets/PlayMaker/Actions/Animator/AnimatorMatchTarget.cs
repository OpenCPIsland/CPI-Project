// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Automatically adjust the GameObject position and rotation so that the AvatarTarget reaches the Match Position when the current animation state is at the specified progress.")]
	public class AnimatorMatchTarget: ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator component.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The body part that is used to match the target.")]
		public AvatarTarget bodyPart;
		
		[Tooltip("A GameObject target to match. Leave empty to use position instead.")]
		public FsmGameObject target;
		
		[Tooltip("A target position to match. If Target GameObject is set, this is used as an offset from the Target's position.")]
		public FsmVector3 targetPosition;
		
		[Tooltip("A rotation to match. If Target GameObject is set, this is used as an offset from the Target's rotation.")]
		public FsmQuaternion targetRotation;
		
		[Tooltip("The MatchTargetWeightMask Position XYZ weight")]
		public FsmVector3 positionWeight;
		
		[Tooltip("The MatchTargetWeightMask Rotation weight")]
		public FsmFloat rotationWeight;
		
		[Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
		public FsmFloat startNormalizedTime;
		
		[Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip). " +
                 "Values greater than 1 trigger a match after a certain number of loops. " +
                 "Example: 2.3 means at 30% of 2nd loop.")]
		public FsmFloat targetNormalizedTime;
		
		[Tooltip("Should always be true")]
		public bool everyFrame;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        private GameObject cachedTarget;
		private Transform targetTransform;
        private MatchTargetWeightMask weightMask;
		
		public override void Reset()
		{
			gameObject = null;
			bodyPart = AvatarTarget.Root;
			target = null;
			targetPosition = new FsmVector3 {UseVariable = true};
			targetRotation = new FsmQuaternion {UseVariable = true};
			positionWeight = Vector3.one;
			rotationWeight = 0f;
			startNormalizedTime = null;
			targetNormalizedTime = null;
			everyFrame= true;
		}
		
		public override void OnEnter()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            if (cachedTarget != target.Value)
            {
                cachedTarget = target.Value;
                targetTransform = cachedTarget != null ? cachedTarget.transform : null;
            }

            weightMask = new MatchTargetWeightMask();

            DoMatchTarget();
			
			if(!everyFrame)
			{
				Finish();
			}
        }
		
		public override void OnUpdate()
		{
			DoMatchTarget();
		}

        private void DoMatchTarget()
        {
            if (animator == null) return;
			
			var pos = Vector3.zero;
			var rot = Quaternion.identity;
			
			if (targetTransform != null)
			{
				pos = targetTransform.position;
				rot = targetTransform.rotation;
			}
			
			if (!targetPosition.IsNone)
			{
				pos += targetPosition.Value;
			}
			
			if (!targetRotation.IsNone)
			{
				rot *= targetRotation.Value;
            }

            weightMask.positionXYZWeight = positionWeight.Value;
            weightMask.rotationWeight = rotationWeight.Value;

            animator.MatchTarget(pos, rot, bodyPart, weightMask, startNormalizedTime.Value, targetNormalizedTime.Value);
        }
		
	}
}