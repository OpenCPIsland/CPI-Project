// (c) Copyright HutongGames, LLC 2010-2016. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Stops the animator record mode. It will lock the recording buffer's contents in its current state. The data get saved for subsequent playback with StartPlayback.")]
	public class AnimatorStopRecording : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		[Tooltip("The recorder StartTime")]
		public FsmFloat recorderStartTime;

		[UIHint(UIHint.Variable)]
		[Tooltip("The recorder StopTime")]
		public FsmFloat recorderStopTime;

		public override void Reset()
		{
			gameObject = null;

			recorderStartTime = null;
			recorderStopTime = null;
		}
		
		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                cachedComponent.StopRecording();
                recorderStartTime.Value = cachedComponent.recorderStartTime;
                recorderStopTime.Value = cachedComponent.recorderStopTime;
            }

			Finish();
        }
    }
}