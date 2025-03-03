using UnityEngine;

namespace ClubPenguin.UI
{
	public class CanvasScalerExtMatchOverride : MonoBehaviour
	{
		[Range(0f, 1f)]
		[Header("Match Width Or Height Override")]
		[Tooltip("In some areas like the catalogs the lists are vertical. This allows the scaler to be overridden for specific UI.")]
		public float OverrideMatchWidthOrHeightValue;

		private CanvasScalerExt canvasScalerExt;

		private float previousMatchWidthOrHeightValue;

		private void Start()
		{
			canvasScalerExt = GetComponentInParent<CanvasScalerExt>();
			if (canvasScalerExt == null)
			{
			}
			if (canvasScalerExt != null)
			{
				previousMatchWidthOrHeightValue = canvasScalerExt.matchWidthOrHeight;
				canvasScalerExt.matchWidthOrHeight = OverrideMatchWidthOrHeightValue;
			}
		}

		private void OnDestroy()
		{
			if (canvasScalerExt != null)
			{
				canvasScalerExt.matchWidthOrHeight = previousMatchWidthOrHeightValue;
			}
		}
	}
}
