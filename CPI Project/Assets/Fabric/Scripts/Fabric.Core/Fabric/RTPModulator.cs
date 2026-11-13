using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	[AddComponentMenu("Fabric/Effects/Modulator")]
	public class RTPModulator : MonoBehaviour
	{
		[SerializeField]
		public ModulatorType type;

		[Range(0f, 20000f)]
		[SerializeField]
		public float frequency = 10000f;

		[SerializeField]
		[Range(0f, 1f)]
		public float phase;

		[SerializeField]
		[Range(0f, 1f)]
		private float amplitude = 1f;

		[Range(0f, 1f)]
		public float offset;

		[Range(-1f, 1f)]
		[SerializeField]
		public float invert = 1f;

		public float GetValue(float time)
		{
			float num = 0f;
			float num2 = frequency / (float)AudioSettings.outputSampleRate * time + phase;
			switch (type)
			{
			case ModulatorType.Sine:
				num = (float)Math.Sin(Math.PI * 2.0 * (double)num2);
				break;
			case ModulatorType.Square:
				num = Math.Sign(Math.Sin(Math.PI * 2.0 * (double)num2));
				break;
			case ModulatorType.Triangle:
				num = 1f - 4f * (float)Math.Abs(Math.Round(num2 - 0.25f) - (double)(num2 - 0.25f));
				break;
			case ModulatorType.Sawtooth:
				num = 2f * (num2 - (float)Math.Floor(num2 + 0.5f));
				break;
			}
			return invert * amplitude * num + offset;
		}
	}
}
