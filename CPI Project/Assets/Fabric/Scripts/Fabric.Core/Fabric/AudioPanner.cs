using System;
using UnityEngine;

namespace Fabric
{
	public class AudioPanner : MonoBehaviour
	{
		private static int numChannels = 8;

		[NonSerialized]
		[HideInInspector]
		public float[] _channelGains = new float[numChannels];

		private void OnAudioFilterRead(float[] data, int channels)
		{
			for (int i = 0; i < data.Length; i += channels)
			{
				for (int j = 0; j < channels; j++)
				{
					data[i + j] *= _channelGains[j];
				}
			}
		}
	}
}
