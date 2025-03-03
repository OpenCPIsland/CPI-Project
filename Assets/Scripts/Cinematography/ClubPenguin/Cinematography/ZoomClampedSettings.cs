using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[Serializable]
	public struct ZoomClampedSettings
	{
		public AspectRatioType Type;

		[Range(0f, 100f)]
		public float MinFOV;

		[Range(0f, 100f)]
		public float MaxFOV;
	}
}
