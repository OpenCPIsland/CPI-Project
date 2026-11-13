using UnityEngine;

namespace ClubPenguin.Core
{
	public static class FeepMath
	{
		public static void calcDampedSimpleHarmonicMotion(ref float pPos, ref float pVel, float equilibriumPos, float deltaTime, float angularFrequency, float dampingRatio)
		{
			if (!(angularFrequency < 0.0001f))
			{
				if (dampingRatio < 0f)
				{
					dampingRatio = 0f;
				}
				float num = pPos - equilibriumPos;
				float num2 = pVel;
				if (dampingRatio > 1.0001f)
				{
					float num3 = (0f - angularFrequency) * dampingRatio;
					float num4 = angularFrequency * Mathf.Sqrt(dampingRatio * dampingRatio - 1f);
					float num5 = num3 - num4;
					float num6 = num3 + num4;
					float num7 = Mathf.Exp(num5 * deltaTime);
					float num8 = Mathf.Exp(num6 * deltaTime);
					float num9 = (num2 - num * num6) / (-2f * num4);
					float num10 = num - num9;
					pPos = equilibriumPos + num9 * num7 + num10 * num8;
					pVel = num9 * num5 * num7 + num10 * num6 * num8;
				}
				else if (dampingRatio > 0.9999f)
				{
					float num11 = Mathf.Exp((0f - angularFrequency) * deltaTime);
					float num9 = num2 + angularFrequency * num;
					float num10 = num;
					float num12 = (num9 * deltaTime + num10) * num11;
					pPos = equilibriumPos + num12;
					pVel = num9 * num11 - num12 * angularFrequency;
				}
				else
				{
					float num13 = angularFrequency * dampingRatio;
					float num14 = angularFrequency * Mathf.Sqrt(1f - dampingRatio * dampingRatio);
					float num11 = Mathf.Exp((0f - num13) * deltaTime);
					float num15 = Mathf.Cos(num14 * deltaTime);
					float num16 = Mathf.Sin(num14 * deltaTime);
					float num9 = num;
					float num10 = (num2 + num13 * num) / num14;
					pPos = equilibriumPos + num11 * (num9 * num15 + num10 * num16);
					pVel = (0f - num11) * ((num9 * num13 - num10 * num14) * num15 + (num9 * num14 + num10 * num13) * num16);
				}
			}
		}

		public static void bezierInterp(ref Vector3 ptA, ref Vector3 handleA, ref Vector3 ptB, ref Vector3 handleB, float t, ref Vector3 output)
		{
			float num = t * t;
			float d = num * t;
			float num2 = 1f - t;
			float num3 = num2 * num2;
			float d2 = num2 * num3;
			Vector3 a = ptA * d2;
			Vector3 b = handleA * (3f * num3 * t);
			Vector3 b2 = handleB * (3f * num2 * num);
			Vector3 b3 = ptB * d;
			output = a + b + b2 + b3;
		}
	}
}
