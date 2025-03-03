using System;
using UnityEngine;

namespace ClubPenguin
{
	public class WaterProperties : MonoBehaviour
	{
		[Serializable]
		public struct Properties
		{
			public float Drag;

			public float MaxSpeed;

			public float RippleRate;

			public float RippleAmplitude;

			public float SurfaceOffset;
		}

		public Properties properties = new Properties
		{
			Drag = 0.5f,
			MaxSpeed = 0.4f,
			RippleRate = 0.75f,
			RippleAmplitude = 3f,
			SurfaceOffset = -0.07f
		};
	}
}
