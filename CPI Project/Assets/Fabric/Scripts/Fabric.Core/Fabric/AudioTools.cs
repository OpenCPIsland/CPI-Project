using UnityEngine;

namespace Fabric
{
	public class AudioTools
	{
		private static float dynamicRange = 96f;

		public static void Limit(ref float value, float min, float max)
		{
			if (value < min)
			{
				value = min;
			}
			if (value > max)
			{
				value = max;
			}
		}

		public static float LinearToDB(float linear)
		{
			Limit(ref linear, 1E-05f, 100000f);
			float value = 20f * Mathf.Log10(linear);
			Limit(ref value, 0f - dynamicRange, dynamicRange);
			return value;
		}

		public static float DBToLinear(float db)
		{
			Limit(ref db, 0f - dynamicRange, dynamicRange);
			return Mathf.Pow(10f, db / 20f);
		}

		public static float DBToNormalizedDB(float db)
		{
			return (dynamicRange + Mathf.Min(db, 0f)) / dynamicRange;
		}
	}
}
