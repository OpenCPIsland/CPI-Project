using UnityEngine;

namespace ClubPenguin.Actions
{
	public class CameraLayerVisibilityAction : Action
	{
		public string LayerName;

		public bool IsVisible;

		protected override void CopyTo(Action _destWarper)
		{
			CameraLayerVisibilityAction cameraLayerVisibilityAction = _destWarper as CameraLayerVisibilityAction;
			cameraLayerVisibilityAction.LayerName = LayerName;
			cameraLayerVisibilityAction.IsVisible = IsVisible;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			if (!string.IsNullOrEmpty(LayerName))
			{
				if (IsVisible)
				{
					Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(LayerName);
				}
				else
				{
					Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer(LayerName));
				}
			}
			Completed();
		}
	}
}
