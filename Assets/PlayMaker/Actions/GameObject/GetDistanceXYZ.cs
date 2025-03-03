// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance between a GameObject and a target GameObject/Position. " +
             "If both GameObject and Position are defined, position is taken a local offset from the GameObject's position.")]
	public class GetDistanceXYZ : ComponentAction<Transform>
	{
		[RequiredField]
        [Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Measure distance to this GameObject (or set world position below).")]
        public FsmGameObject target;

        [Tooltip("World position or local offset from target GameObject, if defined.")]
        public FsmVector3 position;

		[UIHint(UIHint.Variable)]
        [Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeDistance;

        [Tooltip("Space used to measure the distance in. E.g. along the world X axis or the GameObject's local X axis.")]
        public Space space;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance along the X axis.")]
        public FsmFloat storeXDistance;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance along the Y axis.")]
        public FsmFloat storeYDistance;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the distance along the Z axis.")]
        public FsmFloat storeZDistance;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private Transform gameObjectTransform
        {
            get { return cachedComponent; }
        }

        private GameObject cachedTargetGameObject;
        private Transform targetTransform;

		public override void Reset()
		{
			gameObject = null;
			target = null;
            position = null;
			storeDistance = null;
            space = Space.World;
            storeXDistance = null;
            storeYDistance = null;
            storeZDistance = null;
			everyFrame = true;
		}
		
		public override void OnEnter()
		{
			DoGetDistanceXYZ();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
		public override void OnUpdate()
		{
			DoGetDistanceXYZ();
		}

        private void DoGetDistanceXYZ()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

            if (target.Value != null && cachedTargetGameObject != target.Value)
            {
                cachedTargetGameObject = target.Value;
                targetTransform = cachedTargetGameObject.transform;
            }

            var targetPosition = Vector3.zero;
            if (position.IsNone)
            {
                if (targetTransform != null)
                {
                    targetPosition = targetTransform.position;
                }
            }
            else
            {
                targetPosition = targetTransform == null
                    ? position.Value // world position
                    : targetTransform.TransformPoint(position.Value); // local offset
            }

            if (!storeDistance.IsNone)
            {
                storeDistance.Value = Vector3.Distance(gameObjectTransform.position, targetPosition);
            }

            if (storeXDistance.IsNone && storeYDistance.IsNone && storeZDistance.IsNone)
                return;

            if (space == Space.Self)
            {
                targetPosition = gameObjectTransform.InverseTransformPoint(targetPosition);
            }
            else
            {
                targetPosition -= gameObjectTransform.position;
            }

            storeXDistance.Value = targetPosition.x;
            storeYDistance.Value = targetPosition.y;
            storeZDistance.Value = targetPosition.z;
        }

    }
}