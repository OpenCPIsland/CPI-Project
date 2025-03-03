using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ScreenEdgeBandCalculator : MonoBehaviour
	{
		public const int NO_BAND = -1;

		[Range(0f, 50f)]
		[Tooltip("The number segments")]
		public int BandCount = 3;

		[Tooltip("Percentage of screen height or width each band occupies ")]
		[Range(2f, 50f)]
		public int BandPercentage = 5;

		[Tooltip("Percentage of the longer aspect. In portrait this would be the height for example.")]
		[Range(2f, 50f)]
		public int BandPercentageLongAspect = 8;

		private Vector2 TopLeft;

		private Vector2 TopRight;

		private Vector2 BottomRight;

		private Vector2 BottomLeft;

		public void Update()
		{
		}

		public void OnValidate()
		{
		}

		public int CalculateBandNumber(Vector2 screenPosition)
		{
			int result = -1;
			for (int i = 1; i <= BandCount; i++)
			{
				if (IsPositionGreaterThanOrEqualToBand(screenPosition, i))
				{
					result = BandCount + 1 - i;
					break;
				}
			}
			return result;
		}

		private bool IsPositionGreaterThanOrEqualToBand(Vector2 position, int band)
		{
			calculateScreenPositions(band);
			if (position.y < TopLeft.y)
			{
				return true;
			}
			if (position.x < TopLeft.x)
			{
				return true;
			}
			if (position.x > TopRight.x)
			{
				return true;
			}
			if (position.y > BottomLeft.y)
			{
				return true;
			}
			return false;
		}

		private void calculateScreenPositions(int band)
		{
			float num = (float)(band * BandPercentage) / 100f;
			float num2 = (float)(band * BandPercentageLongAspect) / 100f;
			if (Screen.height > Screen.width)
			{
				num = (float)(band * BandPercentageLongAspect) / 100f;
				num2 = (float)(band * BandPercentage) / 100f;
			}
			float num3 = (float)Screen.width * num2;
			float num4 = (float)Screen.height * num;
			TopLeft = new Vector2(num3, num4);
			TopRight = new Vector2((float)Screen.width - num3, num4);
			BottomRight = new Vector2((float)Screen.width - num3, (float)Screen.height - num4);
			BottomLeft = new Vector2(num3, (float)Screen.height - num4);
		}
	}
}
