using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class CamVisibilityIterator : VisibilityIterator
	{
		public CameraVisData CamVisData;

		private int currentIndex = -1;

		private Visibility current;

		public override Visibility Current
		{
			get
			{
				return current;
			}
		}

		public override bool MoveNext()
		{
			currentIndex++;
			if (currentIndex < CamVisData.Positions.Length)
			{
				Vector3 position = CamVisData.Positions[currentIndex];
				Quaternion orientation = Quaternion.LookRotation(CamVisData.ForwardVectors[currentIndex], CamVisData.UpVectors[currentIndex]);
				float cameraFOV = CamVisData.CameraFOV;
				current = new Visibility(position, orientation, cameraFOV);
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			currentIndex = -1;
		}
	}
}
