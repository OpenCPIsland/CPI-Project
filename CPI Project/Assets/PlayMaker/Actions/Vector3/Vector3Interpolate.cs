// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
	public class Vector3Interpolate : FsmStateAction
	{
        [Tooltip("The type of interpolation to use.")]
        public InterpolationType mode;
		[RequiredField]
        [Tooltip("The start vector.")]
        public FsmVector3 fromVector;
		[RequiredField]
        [Tooltip("The end vector.")]
        public FsmVector3 toVector;
		[RequiredField]
        [Tooltip("How long it should take to interpolate from start to end.")]
        public FsmFloat time;
		[RequiredField]
		[UIHint(UIHint.Variable)]
        [Tooltip("Store the interpolated vector in a Vector3 Variable.")]
        public FsmVector3 storeResult;
        [Tooltip("Optionally send this event when finished.")]
        public FsmEvent finishEvent;
		[Tooltip("Ignore TimeScale e.g., if the game is paused.")]
		public bool realTime;

		private float startTime;
		private float currentTime;
		
		public override void Reset()
		{
			mode = InterpolationType.Linear;
			fromVector = new FsmVector3 { UseVariable = true };
			toVector = new FsmVector3 { UseVariable = true };
			time = 1.0f;
			storeResult = null;
			finishEvent = null;
			realTime = false;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			currentTime = 0f;
			
			if (storeResult == null)
				Finish();
			else
				storeResult.Value = fromVector.Value;
		}
		
		public override void OnUpdate()
		{
			// update time
			
			if (realTime)
			{
				currentTime = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				currentTime += Time.deltaTime;
			}
			
			float weight = currentTime/time.Value;
			
			switch (mode) {
			
			case InterpolationType.Linear:
				break;
				
			case InterpolationType.EaseInOut:
				weight = Mathf.SmoothStep(0, 1, weight);				
				break;
			}

			storeResult.Value = Vector3.Lerp(fromVector.Value, toVector.Value, weight);
			
			if (weight >= 1)
			{
				if (finishEvent != null)
					Fsm.Event(finishEvent);

				Finish();
			}
				
		}
	}
}

