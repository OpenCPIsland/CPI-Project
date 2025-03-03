using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[Serializable]
	public class RectTransformSettings : AbstractAspectRatioSpecificSettings
	{
		public bool AnchorMinXOption;

		public bool AnchorMinYOption;

		public bool AnchorMaxXOption;

		public bool AnchorMaxYOption;

		public bool SizeDeltaXOption;

		public bool SizeDeltaYOption;

		public bool PositionZOption;

		public Vector2 AnchorMin;

		public Vector2 AnchorMax;

		public Vector2 SizeDelta;

		public float PositionZ;
	}
}
