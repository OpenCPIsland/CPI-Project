// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioPitch : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
        [Tooltip("A GameObject with an AudioSource component.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Set the pitch.")]
        public FsmFloat pitch;

        [Tooltip("Repeat every frame. Useful if you're driving pitch with a float variable.")]
        public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			pitch = 1;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetAudioPitch();

		    if (!everyFrame)
		    {
		        Finish();
		    }
		}
				
		public override void OnUpdate ()
		{
			DoSetAudioPitch();
		}	
		
		void DoSetAudioPitch()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
			{
			    if (!pitch.IsNone)
			    {
			        audio.pitch = pitch.Value;
			    }
			}
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, pitch);
	    }
#endif

	}
}