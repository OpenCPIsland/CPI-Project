// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#if UNITY_2018_2_OR_NEWER
#pragma warning disable 618  
#endif

#if !(UNITY_SWITCH || UNITY_TVOS || UNITY_IPHONE || UNITY_IOS  || UNITY_ANDROID || UNITY_FLASH || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_PSM || UNITY_WEBGL || UNITY_SWITCH)

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
#if UNITY_2019_3_OR_NEWER
    // Mark Obsolete but keep parameters
    // so user can convert them to new actions.
    [Obsolete("Use VideoPlayer actions instead.")]
#endif
	[ActionCategory(ActionCategory.Movie)]
    [Tooltip("Sets the Game Object to use to play the audio source associated with a movie texture. Note: the Game Object must have an <a href=\"http://unity3d.com/support/documentation/Components/class-AudioSource.html\">AudioSource</a> component.")]
    public class MovieTextureAudioSettings : FsmStateAction
	{

        [RequiredField]
#if !UNITY_2019_3_OR_NEWER
		[ObjectType(typeof(MovieTexture))]
#endif
        [Tooltip("The movie texture to set.")]
        public FsmObject movieTexture;

		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
        [Tooltip("The Game Object to use to play audio. Should have an AudioSource component.")]
        public FsmGameObject gameObject;
		
		// this gets overridden by AudioPlay...
		//public FsmFloat volume;

		public override void Reset()
		{
			movieTexture = null;
			gameObject = null;
			//volume = 1;
		}

		public override void OnEnter()
        {
#if !UNITY_2019_3_OR_NEWER
            
            var movie = movieTexture.Value as MovieTexture;

			if (movie != null && gameObject.Value != null)
			{
			    var audio = gameObject.Value.GetComponent<AudioSource>();
				if (audio != null)
				{
					audio.clip = movie.audioClip;
					
					//if (!volume.IsNone)
					//	audio.volume = volume.Value;
				}
			}
#endif

			Finish();
		}

#if UNITY_2019_3_OR_NEWER
        public override string ErrorCheck()
        {
            return "MovieTexture is Obsolete. Use VideoPlayer actions instead.";
        }
#endif
	}
}

#endif

