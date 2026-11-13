// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#if UNITY_2018_2_OR_NEWER
#pragma warning disable 618  
#endif

#if !(UNITY_SWITCH || UNITY_TVOS || UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID || UNITY_FLASH || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_BLACKBERRY || UNITY_METRO || UNITY_WP8 || UNITY_PSM || UNITY_WEBGL || UNITY_SWITCH)

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Movie)]
#if UNITY_2019_3_OR_NEWER
    // Mark Obsolete but keep parameters
    // so user can convert them to new actions.
    [Obsolete("Use VideoPlayer actions instead.")]
#endif
	[Tooltip("Stops playing the Movie Texture, and rewinds it to the beginning.")]
	public class StopMovieTexture : FsmStateAction
	{
		[RequiredField]
#if !UNITY_2019_3_OR_NEWER
        [ObjectType(typeof(MovieTexture))]
#endif
        [Tooltip("The Movie Texture to stop.")]
        public FsmObject movieTexture;

		public override void Reset()
		{
			movieTexture = null;
		}

		public override void OnEnter()
		{
#if !UNITY_2019_3_OR_NEWER
			var movie = movieTexture.Value as MovieTexture;

			if (movie != null)
			{
				movie.Stop();
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