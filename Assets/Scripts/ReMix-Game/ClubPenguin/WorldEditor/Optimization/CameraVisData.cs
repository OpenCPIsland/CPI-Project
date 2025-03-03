using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class CameraVisData : ScriptableObject
	{
		public Vector3[] Positions;

		public Vector3[] ForwardVectors;

		public Vector3[] RightVectors;

		public Vector3[] UpVectors;

		public bool[] IsDerived;

		public float CameraFOV;
	}
}
