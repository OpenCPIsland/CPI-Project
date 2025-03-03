using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	public class TransformSettings : AbstractAspectRatioSpecificSettings
	{
		public bool PositionOption;

		public bool ScaleOption;

		public Vector3 LocalPosition;

		public Vector3 Scale;
	}
}
