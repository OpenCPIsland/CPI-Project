using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Cinematography.Cameras
{
	public class ChaseCameraTrigger : MonoBehaviour
	{
		public enum ActionType
		{
			EnableChaseCamera,
			DisableChaseCamera,
			SetTrackDirection
		}

		[Tooltip("Whether to enable or disable Chase Camera when entering this volume.")]
		public ActionType Action = ActionType.EnableChaseCamera;

		public string Tag = "Player";

		public float LookatDistance = 10f;

		public void OnTriggerEnter(Collider col)
		{
			if (!(col != null) || !col.CompareTag(Tag) || !(col.gameObject != null))
			{
				return;
			}
			ChaseCamera chaseCamera = SceneRefs.Get<ChaseCamera>();
			if (chaseCamera != null)
			{
				if (Action == ActionType.EnableChaseCamera)
				{
					chaseCamera.Enable(col.gameObject);
				}
				else if (Action == ActionType.SetTrackDirection)
				{
					chaseCamera.SetTrackDirection(base.gameObject);
				}
				else
				{
					chaseCamera.Disable();
				}
			}
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Vector3 vector = base.transform.position + base.transform.forward * LookatDistance;
			Gizmos.DrawLine(base.transform.position, vector);
			Gizmos.DrawSphere(vector, 0.3f);
		}
	}
}
