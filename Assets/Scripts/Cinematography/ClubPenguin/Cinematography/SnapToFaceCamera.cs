using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class SnapToFaceCamera : MonoBehaviour
	{
		public bool FaceY = false;

		public void LateUpdate()
		{
			Vector3 forward = Camera.main.transform.position - base.transform.position;
			if (!FaceY)
			{
				forward.y = 0f;
			}
			base.transform.rotation = Quaternion.LookRotation(forward);
		}
	}
}
