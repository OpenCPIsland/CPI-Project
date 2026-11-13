using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class PhysicalSizeHorizontalLayoutScaler : PhysicalSizeScaler
	{
		public float Spacing_Iphone5 = 0f;

		public float Spacing_Ipad = 0f;

		private HorizontalLayoutGroup horizontalLayoutGroup;

		private void Start()
		{
			horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
			ApplyScale();
		}

		private void ApplyScale()
		{
			float targetDimensions = GetTargetDimensions();
			horizontalLayoutGroup.spacing = targetDimensions;
		}

		private float GetTargetDimensions()
		{
			float deviceSize = GetDeviceSize();
			float num = NormalizeScaleProperty(Spacing_Ipad, Spacing_Iphone5);
			return Spacing_Iphone5 + (deviceSize - 4f) * num;
		}
	}
}
