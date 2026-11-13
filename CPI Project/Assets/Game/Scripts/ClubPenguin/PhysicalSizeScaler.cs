using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(RectTransform))]
	public class PhysicalSizeScaler : MonoBehaviour
	{
		protected const float DEFAULT_SCREEN_DPI = 96f;

		protected const float IPAD_DIAGONAL_INCHES = 10f;

		protected const float IPHONE5_DIAGONAL_INCHES = 4f;

		protected float screenDPI = 0f;

		protected float GetDeviceSize()
		{
			return Mathf.Sqrt(Mathf.Pow((float)Screen.width / GetScreenDPI(), 2f) + Mathf.Pow((float)Screen.height / GetScreenDPI(), 2f));
		}

		protected float NormalizeScaleProperty(float iPadValue, float iPhoneValue)
		{
			return (iPadValue - iPhoneValue) / 6f;
		}

		protected float GetScreenDPI()
		{
			if (screenDPI == 0f)
			{
				screenDPI = Screen.dpi;
				if (screenDPI == 0f || screenDPI == -1f)
				{
					screenDPI = 96f;
				}
			}
			return screenDPI;
		}
	}
}
