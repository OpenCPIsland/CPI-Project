// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioStop : ComponentAction<AudioSource>
    {
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
        [Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

        [Tooltip("Audio Stop can make a hard pop sound. A short fade out can fix this glitch.")]
        public FsmFloat fadeTime;

        private float volume;

		public override void Reset()
		{
			gameObject = null;
            fadeTime = null;
        }

		public override void OnEnter()
		{
            if (UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
            {
                volume = audio.volume;

                if (fadeTime.Value < 0.01f)
                {
                    audio.Stop();
                }
                else
                {
                    StartCoroutine(VolumeFade(audio, 0, fadeTime.Value));
                }
            }

            Finish();
		}

        private IEnumerator VolumeFade(AudioSource audioSource, float endVolume, float fadeDuration)
        {
            var startTime = Time.time;
            while (Time.time < startTime + fadeDuration)
            {
                var alpha = (startTime + fadeDuration - Time.time) / fadeDuration;
                // use the square so that we fade faster and without popping
                alpha *= alpha;
                audioSource.volume = alpha * volume + endVolume * (1.0f - alpha);

                yield return null;
            }

            if (Math.Abs(endVolume) < .01f)
            {
                audioSource.Stop();
            }
        }
    }
}