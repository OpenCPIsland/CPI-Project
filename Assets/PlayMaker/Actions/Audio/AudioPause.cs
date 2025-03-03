// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioPause : ComponentAction<AudioSource>
    {
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
        [Tooltip("The GameObject with an Audio Source component.")]
        public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                audio.Pause();
            }

			Finish();
		}
	}
}