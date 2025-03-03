using System;
using UnityEngine;

namespace ClubPenguin
{
	public class RaceTrackProperties : MonoBehaviour
	{
		[Serializable]
		public struct Properties
		{
			public float Drag;

			public float MaxSpeed;
		}

		public Properties properties = new Properties
		{
			Drag = 0.25f,
			MaxSpeed = 12f
		};
	}
}
