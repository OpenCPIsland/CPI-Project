using ClubPenguin.Core;
using HutongGames.PlayMaker;
using System;

namespace ClubPenguin.Cinematography
{
	[Serializable]
	public class ChangeCameraTargetActionSettings : AbstractAspectRatioSpecificSettings
	{
		public string TargetName;

		public FsmOwnerDefault TargetGameObject;
	}
}
