// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
	public class GetAnimatorRoot: FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;
		
		[ActionSection("Results")]
			
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmVector3 rootPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion rootRotation;
		
		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

        private Animator animator
        {
            get { return cachedComponent; }
        }

        private GameObject cachedBodyGameObject;
        private Transform _transform;
		
		public override void Reset()
		{
			base.Reset();

			gameObject = null;
			rootPosition= null;
			rootRotation = null;
			bodyGameObject = null;
        }
		
		public override void OnEnter()
		{
			DoGetBodyPosition();
			
			if (!everyFrame)
			{
				Finish();
			}
        }

		public override void OnActionUpdate() 
		{
			DoGetBodyPosition();
		}

        private void DoGetBodyPosition()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                Finish();
                return;
            }

            if (cachedBodyGameObject != bodyGameObject.Value)
            {
                cachedBodyGameObject = bodyGameObject.Value;
                _transform = cachedBodyGameObject != null ? cachedBodyGameObject.transform : null;
            }

            rootPosition.Value = animator.rootPosition;
			rootRotation.Value = animator.rootRotation;
			
			if (_transform != null)
			{
				_transform.position = animator.rootPosition;
				_transform.rotation = animator.rootRotation;
			}
		}

	}
}