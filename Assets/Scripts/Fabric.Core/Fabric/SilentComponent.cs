using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/SilentComponent")]
	public class SilentComponent : AudioComponent
	{
		[SerializeField]
		[Range(0f, 120f)]
		public float _silenceLength = 1f;

		[SerializeField]
		[Range(0f, 120f)]
		public float _randomizeSilenceLength;

		[SerializeField]
		public bool _looping;

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			base.Loop = _looping;
			float num = _silenceLength + (float)(base._random.NextDouble() * (double)_randomizeSilenceLength);
			int outputSampleRate = AudioSettings.outputSampleRate;
			AudioClip audioClip2 = base.AudioClip = AudioClip.Create("silenceAudioClip", (int)((float)outputSampleRate * num), 1, outputSampleRate, false);
			base.OnInitialise(inPreviewMode);
		}
	}
}
