using UnityEngine;

namespace Fabric
{
	public class StereoSpreader : MonoBehaviour
	{
		private float[] buffer = new float[4096];

		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (channels != 2)
			{
				return;
			}
			for (int i = 0; i < buffer.Length - data.Length; i++)
			{
				buffer[i] = buffer[i + data.Length];
			}
			for (int j = buffer.Length - data.Length; j < buffer.Length; j++)
			{
				buffer[j] = data[j - (buffer.Length - data.Length)];
			}
			bool flag = true;
			for (int k = 0; k < data.Length; k++)
			{
				if (flag)
				{
					data[k] *= 0.85f;
				}
				else
				{
					data[k] = buffer[k - 1];
				}
				flag = !flag;
			}
		}
	}
}
