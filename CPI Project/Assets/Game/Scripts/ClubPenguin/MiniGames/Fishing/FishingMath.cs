using System;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class FishingMath
	{
		public static Vector2 GetCirclePosition(float t, float radius)
		{
			Vector2 result = Vector3.zero;
			float f = t * (float)Math.PI * 2f;
			result.x = radius * Mathf.Cos(f);
			result.y = radius * Mathf.Sin(f);
			return result;
		}

		public static Vector2 GetRosePosition(float t, float radius, FishingGamePatternConfig config)
		{
			return GetRosePosition(t, radius, config.roseK, config.roseOffset, config.roseRange, config.roseRotation);
		}

		public static Vector2 GetRosePosition(float t, float radius, float roseK, float offsetFactor, float range, float rotation)
		{
			Vector2 vector = Vector3.zero;
			float num = t * range;
			float num2 = radius * offsetFactor;
			float num3 = radius - num2;
			float num4 = Mathf.Cos(roseK * num);
			float num5 = Mathf.Cos(num);
			float num6 = Mathf.Sin(num);
			vector.x = num2 * num5 + num3 * num4 * num5;
			vector.y = num2 * num6 + num3 * num4 * num6;
			if (rotation != 0f)
			{
				vector = Quaternion.Euler(Vector3.forward * rotation) * vector;
			}
			return vector;
		}

		public static float GetRoseRange(int roseN, int roseD)
		{
			return (float)Math.PI * 2f * (float)LeastCommonMultiple(Mathf.Abs(roseN), Mathf.Abs(roseD));
		}

		private static int LeastCommonMultiple(int a, int b)
		{
			return a * b / GreatestCommonDivisor(a, b);
		}

		private static int GreatestCommonDivisor(int a, int b)
		{
			if (a < 0 || b < 0)
			{
				return -1;
			}
			if (b == 0)
			{
				return a;
			}
			return GreatestCommonDivisor(b, a % b);
		}
	}
}
