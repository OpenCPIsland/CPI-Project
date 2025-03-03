using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class CameraFacingController : MonoBehaviour
	{
		public Transform AttachPoint;

		private void Update()
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<BaseCamera>())
			{
				Transform transform = ClubPenguin.Core.SceneRefs.Get<BaseCamera>().transform;
				Transform transform2 = base.transform;
				if ((bool)AttachPoint)
				{
					transform2.position = AttachPoint.position;
				}
				transform2.LookAt(transform2.position + transform.rotation * Vector3.forward, transform.rotation * Vector3.up);
			}
		}
	}
}
