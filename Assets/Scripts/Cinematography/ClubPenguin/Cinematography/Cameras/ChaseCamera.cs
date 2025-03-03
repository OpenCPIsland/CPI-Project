using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography.Cameras
{
	public class ChaseCamera : MonoBehaviour
	{
		private static readonly int feelerCount = 8;

		[Tooltip("Desired distance from target to camera (default=10).")]
		[Header("BOOM")]
		[Range(1f, 100f)]
		public float BoomDist = 10f;

		[Range(0f, 20f)]
		[Tooltip("Desired height offset from target to camera (default=2).")]
		public float BoomHeight = 2f;

		[Range(0.5f, 10f)]
		[Tooltip("The higher the value above 1, the farther the boom is pushed in trying to move out of the way (sideways) when player is running at camera (default=1). This multiplies BoomDist.")]
		public float SideBoomDistMult = 1f;

		[Range(0f, 100f)]
		[Tooltip("The higher the value, the more quickly the camera aims at its target (default=6).")]
		public float LookatSmoothing = 6f;

		[Range(-60f, 60f)]
		public float VerticalOffsetAngle = 30f;

		public float CatchupBoomDistCountForDouble = 0f;

		[Range(0.1f, 2f)]
		[Space(7f)]
		[Tooltip("Camera's mass: The higher the value, the more sluggish and springy it reacts to movement, making it feel very heavy (default=0.2).")]
		[Header("PHYSICS")]
		public float Mass = 0.2f;

		[Range(0.01f, 20f)]
		[Tooltip("Arrival deceleration: The higher the value, the more gradually the camera arrives to its target (default=8).")]
		public float Deceleration = 8f;

		[Range(0.1f, 500f)]
		[Tooltip("Camera's maximum speed (default=200).")]
		public float MaxSpeed = 200f;

		[Tooltip("This can be used in the future if we need to cap overall forces (default=99999).")]
		public float MaxForce = 99999f;

		[Header("COLLISION")]
		[Space(7f)]
		[Tooltip("Toggle to enable/disable collision with environment.")]
		public bool Collision = true;

		[Tooltip("Camera's collision radius: Distance allowed from walls before they begin to push camera away (default=1).")]
		[Range(0.1f, 5f)]
		public float CollisionRadius = 1f;

		[Tooltip("The higher the value, the more reliable and stronger the camera reacts to wall penetration, but the less smooth the transition out of walls (default=20).")]
		[Range(0f, 100f)]
		public float WallForceMult = 20f;

		[Space(7f)]
		[Tooltip("Toggle to enable/disable camera roll.")]
		[Header("BANKING")]
		public bool Banking = true;

		[Tooltip("Scales the camera banking (default=1.2). The lower this value, the less the camera uniformally banks.")]
		[Range(0f, 3f)]
		public float BankWeight = 1.2f;

		[Tooltip("Dampens bank changes. The higher the value, the less smoothing (default=2).")]
		[Range(0f, 10f)]
		public float BankSmoothing = 2f;

		[Tooltip("The higher the value, the more the camera has to be facing along the bank's direction to roll (default=0.25).")]
		[Range(0f, 1f)]
		public float MinBankHeadingErrorDot = 0.25f;

		[Header("TRACK GUIDE")]
		[Space(7f)]
		public bool TrackGuides = false;

		public AnimationCurve TrackDirBlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public float TrackDirBlendDuration = 1f;

		public float TargetDirectionContribution = 1f;

		public float TrackDirectionContribution = 0.5f;

		[Space(7f)]
		[Tooltip("Blend curve to use when blending in and out of this camera.")]
		[Header("CAMERA BLENDING")]
		public AnimationCurve BlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Tooltip("Blend-in duration in seconds (default=0.75)")]
		public float BlendInDuration = 0.75f;

		[Tooltip("Blend-out duration in seconds (default=1)")]
		public float BlendOutDuration = 1f;

		private int raycastLayerMask = 0;

		private Vector3 targetPos = Vector3.zero;

		private Vector3 desiredBoomPos = Vector3.zero;

		private Vector3 curVelocity = Vector3.zero;

		private Vector3 targetFwd = Vector3.forward;

		private Vector3 targetFlatFwd = Vector3.forward;

		private Vector3 totalForce = Vector3.zero;

		private float bankAngle = 0f;

		private float targetFlatSpeed = 0f;

		private Quaternion oldRotation = Quaternion.identity;

		private Vector3 curTrackDir = Vector3.forward;

		private float curTrackDirBlendTime = 0f;

		private Vector3 startTrackDir = Vector3.zero;

		private Transform desiredTransform = null;

		private GameObject player = null;

		private Vector3[] feeler;

		private Vector3 trackTargetPoint;

		private bool isTrackTargetPointValid;

		public Vector3 TrackDir
		{
			get;
			private set;
		}

		public Vector3 TrackPos
		{
			get;
			private set;
		}

		public void Awake()
		{
			SceneRefs.Set(this);
			desiredTransform = new GameObject().transform;
			feeler = new Vector3[feelerCount];
			feeler[0] = base.transform.forward;
			feeler[1] = Vector3.down;
			feeler[2] = Vector3.up;
			feeler[3] = Vector3.right;
			feeler[4] = Vector3.left;
			feeler[5] = Vector3.back;
			feeler[6] = (Vector3.right + Vector3.down).normalized;
			feeler[7] = (Vector3.left + Vector3.down).normalized;
			base.enabled = false;
		}

		public void Enable(GameObject _player)
		{
			if (!base.enabled)
			{
				player = _player;
				Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.ChangeCameraContextEvent(Director.CameraContext.Chase, BlendCurve, BlendInDuration));
				oldRotation = Camera.main.transform.rotation;
				desiredTransform.position = Camera.main.transform.position;
				desiredTransform.rotation = oldRotation;
				TrackDir = Vector3.zero;
				base.enabled = true;
			}
		}

		public void SetTrackDirection(GameObject trigger)
		{
			if (base.enabled)
			{
				TrackPos = trigger.transform.position;
				TrackDir = trigger.transform.forward;
				trackTargetPoint = trigger.transform.position + trigger.transform.forward * 200f;
				if (!isTrackTargetPointValid)
				{
					curTrackDir = base.transform.forward;
					isTrackTargetPointValid = true;
				}
				startTrackDir = curTrackDir;
				curTrackDirBlendTime = 0f;
			}
		}

		public void Disable()
		{
			if (base.enabled)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.ChangeCameraContextEvent(Director.CameraContext.Gameplay, BlendCurve, BlendOutDuration));
				base.enabled = false;
			}
		}

		public void LateUpdate()
		{
			if (!(player != null) || !base.enabled)
			{
				return;
			}
			raycastLayerMask = LayerConstants.GetTubeLayerCollisionMask();
			totalForce = Vector3.zero;
			calcTargetForward(player);
			targetPos = player.transform.position;
			calcDesiredBoom();
			if (Collision)
			{
				avoidWalls();
			}
			arrive();
			Vector3 a = totalForce / Mass;
			curVelocity += a * Time.deltaTime;
			curVelocity = Vector3.ClampMagnitude(curVelocity, MaxSpeed);
			desiredTransform.position += curVelocity * Time.deltaTime;
			Vector3 normalized = (targetPos - base.transform.position).normalized;
			Quaternion b;
			if (TrackGuides && isTrackTargetPointValid)
			{
				Vector3 vector = trackTargetPoint - base.transform.position;
				vector.y = targetPos.y - base.transform.position.y;
				vector.Normalize();
				float num = Vector3.Dot(normalized, vector);
				if (num < 0f)
				{
					vector = Vector3.zero;
				}
				else
				{
					vector *= num;
				}
				if (curTrackDirBlendTime < TrackDirBlendDuration)
				{
					curTrackDirBlendTime += Time.deltaTime;
					curTrackDirBlendTime = Mathf.Min(curTrackDirBlendTime, TrackDirBlendDuration);
					curTrackDir = Vector3.Lerp(startTrackDir, vector, TrackDirBlendCurve.Evaluate(curTrackDirBlendTime / TrackDirBlendDuration));
				}
				else
				{
					curTrackDir = Vector3.Lerp(curTrackDir, vector, 5f * Time.deltaTime);
				}
				Debug.DrawLine(base.transform.position, base.transform.position + curTrackDir * 20f, Color.blue, 0.1f);
				Debug.DrawLine(base.transform.position, trackTargetPoint, Color.black, 0.1f);
				Vector3 vector2 = normalized * TargetDirectionContribution + curTrackDir * TrackDirectionContribution;
				b = Quaternion.LookRotation(vector2);
				Debug.DrawLine(base.transform.position, base.transform.position + vector2 * 20f, Color.red, 0.1f);
			}
			else
			{
				b = Quaternion.LookRotation(normalized);
			}
			if (Banking)
			{
				bank(player);
				desiredTransform.rotation = oldRotation;
				desiredTransform.rotation = Quaternion.Slerp(desiredTransform.rotation, b, LookatSmoothing * Time.deltaTime);
				oldRotation = desiredTransform.rotation;
				float num2 = 1f - Mathf.Min(bankAngle / 45f, 1f);
				desiredTransform.Rotate(VerticalOffsetAngle * num2, 0f, bankAngle * BankWeight, Space.Self);
			}
			else
			{
				desiredTransform.rotation = Quaternion.Slerp(base.transform.rotation, b, LookatSmoothing * Time.deltaTime);
				oldRotation = desiredTransform.rotation;
			}
			base.transform.rotation = desiredTransform.rotation;
			base.transform.position = desiredTransform.position;
		}

		private void accumulateForce(Vector3 force)
		{
			totalForce = Vector3.ClampMagnitude(totalForce + force, MaxForce);
		}

		private void calcTargetForward(GameObject target)
		{
			targetFwd = target.GetComponent<IMotionTrackerAdapter>().Velocity;
			if (targetFwd.sqrMagnitude < 0.1f)
			{
				targetFwd = base.transform.forward;
				targetFlatFwd = targetFwd;
				targetFlatFwd.y = 0f;
				targetFlatSpeed = 0f;
			}
			else
			{
				targetFlatFwd = targetFwd;
				targetFlatFwd.y = 0f;
				targetFlatSpeed = targetFlatFwd.magnitude;
			}
			targetFwd.Normalize();
			targetFlatFwd.Normalize();
		}

		private void calcDesiredBoom()
		{
			float d = 1f;
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			if (targetFlatFwd.sqrMagnitude > 0f && forward.sqrMagnitude > 0f)
			{
				float num = Vector3.Dot(targetFlatFwd, forward);
				if (num < 0f)
				{
					Vector3 vector = Vector3.Cross(targetFlatFwd, Vector3.up);
					vector.y = 0f;
					vector.Normalize();
					if (Vector3.Dot(vector, forward) < 0f)
					{
						targetFlatFwd = -vector;
					}
					else
					{
						targetFlatFwd = vector;
					}
					d = SideBoomDistMult;
				}
			}
			desiredBoomPos = targetPos - targetFlatFwd * BoomDist * d;
			desiredBoomPos.y += BoomHeight;
		}

		private void arrive()
		{
			Vector3 force = Vector3.zero;
			Vector3 a = desiredBoomPos - base.transform.position;
			float magnitude = a.magnitude;
			if (magnitude > 0.001f)
			{
				float num = magnitude / Deceleration;
				num += targetFlatSpeed;
				num = Mathf.Min(num, MaxSpeed);
				Vector3 a2 = a / magnitude * num;
				force = a2 - curVelocity;
			}
			accumulateForce(force);
		}

		private bool feel(Vector3 feelDir, float rayDist, out Vector3 collisionPoint, out Vector3 collisionNormal, out float collisionDistance)
		{
			bool result = false;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, feelDir, out hitInfo, rayDist, raycastLayerMask))
			{
				result = true;
				collisionNormal = hitInfo.normal;
				collisionPoint = hitInfo.point;
				collisionDistance = hitInfo.distance;
			}
			else
			{
				collisionNormal = base.transform.up;
				collisionPoint = Vector3.one * 9999f;
				collisionDistance = 9999f;
			}
			return result;
		}

		private void avoidWalls()
		{
			feeler[0] = base.transform.forward;
			for (int i = 0; i < feelerCount; i++)
			{
				Vector3 collisionPoint;
				Vector3 collisionNormal;
				float collisionDistance;
				if (feel(feeler[i], CollisionRadius, out collisionPoint, out collisionNormal, out collisionDistance))
				{
					if (i == 0)
					{
						break;
					}
					Vector3 vector = base.transform.position + feeler[i] * CollisionRadius - collisionPoint;
					Vector3 force = collisionNormal * vector.magnitude * WallForceMult;
					accumulateForce(force);
				}
			}
		}

		private void bank(GameObject target)
		{
			float b = 0f;
			ISlideControllerAdapter component = target.GetComponent<ISlideControllerAdapter>();
			if (component != null && component.Enabled)
			{
				Vector3 up = target.transform.up;
				Vector3 rhs = Vector3.Cross(up, Vector3.up);
				float f = Vector3.Dot(base.transform.forward, rhs);
				if (Mathf.Abs(f) > MinBankHeadingErrorDot)
				{
					Vector3 vector = Vector3.ProjectOnPlane(up, base.transform.forward);
					float num = 0f - Vector3.Angle(vector, base.transform.up);
					if (Vector3.Dot(vector, base.transform.right) < 0f)
					{
						num = 0f - num;
					}
					float num2 = (Mathf.Abs(f) - MinBankHeadingErrorDot) / (1f - MinBankHeadingErrorDot);
					float num3 = 1f;
					b = num * num2 * num3;
				}
			}
			bankAngle = Mathf.LerpAngle(bankAngle, b, BankSmoothing * Time.deltaTime);
		}
	}
}
