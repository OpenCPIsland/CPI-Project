// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets looping on the AudioSource component on a Game Object.")]
	public class SetAudioLoop : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
        [Tooltip("A GameObject with an AudioSource component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Set the Audio Source looping.")]
		public FsmBool loop;

		public override void Reset()
		{
			gameObject = null;
			loop = false;
		}

		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
			{
		        audio.loop = loop.Value;
			}
			
			Finish();
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, loop);
	    }
#endif

	}
}