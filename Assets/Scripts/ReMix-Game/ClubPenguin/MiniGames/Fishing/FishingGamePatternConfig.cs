using System;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	[Serializable]
	public class FishingGamePatternConfig : ScriptableObject
	{
		public enum Shapes
		{
			Circle,
			Rose
		}

		public FishPatternData[] fishPatternDatas = null;

		public Shapes shape = Shapes.Circle;

		public int roseN = 2;

		public int roseD = 1;

		public float roseOffset = 0f;

		private float _cachedRange = -1f;

		private float _cachedRangeK = -1f;

		public float roseRotation = 0f;

		public float hookInterp = 0.25f;

		public float baseSpeed = 1f;

		public float roseK
		{
			get
			{
				float result = 1f;
				if (roseD != 0)
				{
					result = (float)roseN / ((float)roseD * 1f);
				}
				return result;
			}
		}

		public float roseRange
		{
			get
			{
				if (_cachedRangeK == -1f || _cachedRangeK != roseK)
				{
					_cachedRangeK = roseK;
					_cachedRange = FishingMath.GetRoseRange(roseN, roseD);
				}
				return _cachedRange;
			}
		}
	}
}
