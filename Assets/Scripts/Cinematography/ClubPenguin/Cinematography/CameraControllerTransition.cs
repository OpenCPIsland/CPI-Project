using UnityEngine;
using UnityEngine.Serialization;

namespace ClubPenguin.Cinematography
{
	public class CameraControllerTransition : MonoBehaviour
	{
		public enum CameraTransitionType
		{
			NONE,
			CUT,
			MAX_SPEED,
			CURVE
		}

		public bool DefaultTransitionIn;

		public bool DefaultTransitionOut;

		public int DefaultTransitionInPriority;

		public int DefaultTransitionOutPriority;

		public CameraController OtherController;

		public CameraTransitionType TransitionType = CameraTransitionType.MAX_SPEED;

		[Space(10f)]
		[FormerlySerializedAs("MaxSpeed")]
		public float MaxMoveSpeed = 8f;

		public float MaxAimSpeed = -1f;

		public bool LimitMoveSpeed = true;

		public bool LimitAimSpeed = false;

		public AnimationCurve Curve = new AnimationCurve();

		public float Duration = 1f;

		public bool DampenMovingTarget = false;

		public bool DoubleDampenMovingTargetAim = false;

		public void OnDrawGizmosSelected()
		{
			if (OtherController != null)
			{
				string text = "Cinematography/CameraControllerTransitionOther.png";
				Gizmos.color = Color.grey;
				if (TransitionType == CameraTransitionType.CUT)
				{
					Gizmos.color = Color.yellow;
					text = "Cinematography/CameraControllerTransitionCut.png";
				}
				else if (TransitionType == CameraTransitionType.MAX_SPEED)
				{
					Gizmos.color = Color.green;
					text = "Cinematography/CameraControllerTransitionMaxSpeed.png";
				}
				else
				{
					Gizmos.color = Color.grey;
					text = "Cinematography/CameraControllerTransitionOther.png";
				}
				Gizmos.DrawLine(base.transform.position, OtherController.transform.position);
				Gizmos.DrawIcon(OtherController.transform.position, text);
			}
		}
	}
}
