// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("A GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to assign to the AudioSource.")]
		public FsmObject audioClip;

		public override void Reset()
		{
			gameObject = null;
			audioClip = null;
		}

		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
			{
	            audio.clip = audioClip.Value as AudioClip;
			}
			
			Finish();
		}

#if UNITY_EDITOR
	    public override string AutoName()
	    {
	        return ActionHelpers.AutoName(this, audioClip);
	    }
#endif

	}
}