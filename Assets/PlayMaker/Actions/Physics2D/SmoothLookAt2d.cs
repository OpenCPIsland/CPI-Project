// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a 2d Game Object so its right vector points at a Target. The target can be defined as a 2d Game Object or a 2d/3d world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
	public class SmoothLookAt2d : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate to face a target.")]
		public FsmOwnerDefault gameObject;
		
        [ActionSection("Target")]

		[Tooltip("A target GameObject.")]
		public FsmGameObject targetObject;

		[Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
		public FsmVector2 targetPosition2d;

		[Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
		public FsmVector3 targetPosition;

        [ActionSection("Rotation")]

		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		[HasFloatSlider(0.5f,15)]
		[Tooltip("How fast to rotate to look at the target. Higher numbers are faster. Note, you can enter numbers outside the slider range.")]
		public FsmFloat speed;

        [Tooltip("Set min/max angle limits for the look at rotation. Note, you can use a scene gizmo to set the angles.")]
        public FsmBool useLimits;
        
        [HideIf("HideLimits")]
        [Tooltip("Min angle limit.")]
        public FsmFloat minAngle;

        [HideIf("HideLimits")]
        [Tooltip("Max angle limit.")]
        public FsmFloat maxAngle;

		[Tooltip("Draw a line in the Scene View to the look at position.")]
		public FsmBool debug;
		
        [ActionSection("Finished")]

		[Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
		public FsmFloat finishTolerance;
		
		[Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
		public FsmEvent finishEvent;

        [Tooltip("Should the event stop running when it succeeds.")]
        public FsmBool finish;

		private GameObject previousGo; // track game object so we can re-initialize when it changes.
		private Quaternion lastRotation;
		private Quaternion desiredRotation;

        private Vector3 lookAtPos;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			targetPosition2d = new FsmVector2 { UseVariable = true};
			targetPosition = new FsmVector3 { UseVariable = true};
			rotationOffset = null;
            useLimits = null;
            minAngle = null;
            maxAngle = null;
			debug = false;
			speed = 5;
			finishTolerance = 1;
			finishEvent = null;
            finish = null;
        }

        /// <summary>
        /// Used by HideIf attributes
        /// </summary>
        public bool HideLimits()
        {
            return !useLimits.Value;
        }

        public override void OnPreprocess()
        {
            Fsm.HandleLateUpdate = true;
        }
		
		public override void OnEnter()
		{
			previousGo = null;
		}
		
		public override void OnLateUpdate()
		{
			DoSmoothLookAt();
		}

        private void DoSmoothLookAt()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null) return;

            var transform = go.transform;
			var target = targetObject.Value;

			// re-initialize if game object has changed
			
			if (previousGo != go)
			{
				lastRotation = transform.rotation;
				desiredRotation = lastRotation;
				previousGo = go;
			}
			
			// desired look at position

            if (target != null)
			{
				lookAtPos = target.transform.position;
				var lookAtOffset = Vector3.zero;

				if (!targetPosition.IsNone)
				{
					lookAtOffset += targetPosition.Value;
				}
				if (!targetPosition2d.IsNone)
				{
					lookAtOffset.x = lookAtOffset.x+ targetPosition2d.Value.x;
					lookAtOffset.y = lookAtOffset.y+ targetPosition2d.Value.y;
				}

				if (!targetPosition2d.IsNone || !targetPosition.IsNone)
				{
					lookAtPos += target.transform.TransformPoint(lookAtOffset);
				}
			}
            else
            {
                lookAtPos = new Vector3(targetPosition2d.Value.x,targetPosition2d.Value.y,0f);
                if (!targetPosition.IsNone)
                {
                    lookAtPos += targetPosition.Value;
                }
            }
            
            var lookOffset = lookAtPos - transform.position;
            lookOffset.Normalize();
            
            var zAngle = Mathf.Atan2(lookOffset.y, lookOffset.x) * Mathf.Rad2Deg;
            if (useLimits.Value)
            {
                var zOffset = rotationOffset.Value + (transform.parent != null ? transform.parent.eulerAngles.z : 0);
                zAngle = ClampAngle(zAngle, minAngle.Value  + zOffset, maxAngle.Value + zOffset);
            }

			desiredRotation = Quaternion.Euler(0f, 0f, zAngle - rotationOffset.Value );

            lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, speed.Value * Time.deltaTime);
			transform.rotation = lastRotation;
			
			// debug line to target
			
			if (debug.Value)
			{
				Debug.DrawLine(transform.position, lookAtPos, Color.grey);
			}
			
			// send finish event?
			
			if (finishEvent != null || finish.Value)
			{
                var targetDir = lookAtPos - transform.position;
                var targetAngle = Vector3.Angle(targetDir, transform.right) - rotationOffset.Value;
                if (Mathf.Abs(targetAngle ) <= finishTolerance.Value)
				{
					Fsm.Event(finishEvent);
				}
                
                if (finish.Value) Finish();
			}
        }

        /*
        float ClampAngle(float angle, float from, float to)
        {
            // accepts e.g. -80, 80
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360+from);
            return Mathf.Min(angle, to);
        }*/

        float ClampAngle(float angle, float min, float max)
        {
            if (angle<90 || angle>270){       // if angle in the critic region...
                if (angle>180) angle -= 360;  // convert all angles to -180..+180
                if (max>180) max -= 360;
                if (min>180) min -= 360;
            }    
            angle = Mathf.Clamp(angle, min, max);
            if (angle<0) angle += 360;  // if angle negative, convert to 0..360
            return angle;
        }


#if UNITY_EDITOR

        private float _originalAngle = -1;

		public override float GetProgress()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return 0f;
			}

			var targetDir = lookAtPos - go.transform.position;
			var angle = Vector3.Angle(targetDir, go.transform.right) - rotationOffset.Value;

			if (_originalAngle == -1) {
				_originalAngle = angle;
			}

			_originalAngle = Mathf.Max (_originalAngle, angle);

			return Mathf.Max(0,Mathf.Min(1f-(angle/_originalAngle) , 1f));
		}

#endif
		
	}
}