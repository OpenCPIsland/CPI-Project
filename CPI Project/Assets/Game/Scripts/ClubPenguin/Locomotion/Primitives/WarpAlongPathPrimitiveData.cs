using UnityEngine;

namespace ClubPenguin.Locomotion.Primitives
{
	public class WarpAlongPathPrimitiveData : ScriptableObject
	{
		public string AnimBool;

		public bool StopAtEndPoint;

		public float WarpSpeed = 8f;

		public float TurnSmoothing = 20f;

		public float StartAccel = 60f;

		[Range(0f, 1.5f)]
		public float Curvature = 1f;

		[Range(2f, 100f)]
		public int Steps = 10;

		public GameObject Waypoints
		{
			get;
			set;
		}
	}
}
