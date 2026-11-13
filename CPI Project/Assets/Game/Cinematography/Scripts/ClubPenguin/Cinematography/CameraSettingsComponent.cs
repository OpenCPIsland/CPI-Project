using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraSettingsComponent : AspectRatioSpecificSettingsComponent<Camera, CameraSettings>
	{
		protected override void applySettings(Camera component, CameraSettings settings)
		{
			component.fieldOfView = settings.FieldOfView;
		}
	}
}
