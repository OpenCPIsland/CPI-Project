using ClubPenguin.Core;
using HutongGames.PlayMaker;
using System;

namespace ClubPenguin.Cinematography
{
	[Serializable]
	public class ChangeCameraActionSettings : AbstractAspectRatioSpecificSettings
	{
		public string ControllerName;

		public string TargetName;

		public FsmVector3 StoreOriginalCameraPosition;

		public FsmVector3 CameraPosition;

		public bool PreserveCameraPriority;

		public bool WaitForCameraToComplete;

		public bool ResetOnExit;
	}
}
