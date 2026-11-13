using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public struct Visibility
	{
		public readonly Vector3 Position;

		public readonly Quaternion Orientation;

		public readonly float Fov;

		public Vector3 Forward
		{
			get
			{
				return Orientation * Vector3.forward;
			}
		}

		public Vector3 Up
		{
			get
			{
				return Orientation * Vector3.up;
			}
		}

		public Vector3 Right
		{
			get
			{
				return Orientation * Vector3.right;
			}
		}

		public Visibility(Vector3 position, Quaternion orientation, float fov)
		{
			Position = position;
			Orientation = orientation;
			Fov = fov;
		}
	}
}
