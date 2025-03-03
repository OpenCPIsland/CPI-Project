using System;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationRenderData : ScriptableObject
	{
		public Vector3 ItemPosition;

		public Quaternion ItemRotation;

		public Quaternion CameraRotation;

		public float CameraFOV;

		public override string ToString()
		{
			return string.Format("[DecorationRenderData] ItemPosition: {0}, ItemRotation: {1}, CameraRotation: {2}, CameraFOV: {3}", ItemPosition, ItemRotation, CameraRotation, CameraFOV);
		}
	}
}
