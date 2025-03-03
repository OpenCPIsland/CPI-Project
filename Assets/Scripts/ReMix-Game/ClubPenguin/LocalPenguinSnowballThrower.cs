using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class LocalPenguinSnowballThrower : AbstractPenguinSnowballThrower
	{
		public struct SnowballInstance
		{
			public Vector3 LaunchVel;

			public float TrailAlpha;

			private SnowballController snowball;

			public LifetimeComponent Lifetime
			{
				get;
				private set;
			}

			public SnowballController Snowball
			{
				get
				{
					return snowball;
				}
				set
				{
					snowball = value;
					if (snowball != null)
					{
						Lifetime = snowball.GetComponent<LifetimeComponent>();
					}
					else
					{
						Lifetime = null;
					}
				}
			}

			public void Reset()
			{
				Snowball = null;
				Lifetime = null;
				LaunchVel = Vector3.zero;
				TrailAlpha = 0f;
			}
		}

		private enum PenguinSnowballThrowState
		{
			Idle,
			Charge,
			Throw
		}

		private PenguinSnowballThrowData mutableData;

		[Tooltip("Bone used to launch snowball with")]
		public Transform LauncherBone;

		private SnowballInstance snowballInst;

		private SnowballAimingRays snowballAiming;

		private float chargeTime = 0f;

		private Animator anim;

		private int chargeAnimTrigger;

		private int launchAnimTrigger;

		private CharacterController charController;

		private CPDataEntityCollection dataEntityCollection;

		private bool isPendingChargeSnowball;

		private PenguinSnowballThrowState currentState;

		public void OnValidate()
		{
		}

		private void Awake()
		{
			anim = GetComponent<Animator>();
			snowballAiming = GetComponent<SnowballAimingRays>();
			charController = GetComponent<CharacterController>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		private void Start()
		{
			mutableData = SnowballManager.Instance.GetData();
			chargeAnimTrigger = Animator.StringToHash(mutableData.ChargeAnimTrigger);
			launchAnimTrigger = Animator.StringToHash(mutableData.LaunchAnimTrigger);
			anim.SetBool(AnimationHashes.Params.SnowballThrowEnabled, true);
			setState(PenguinSnowballThrowState.Idle);
		}

		public void ChargeSnowball()
		{
			if (currentState == PenguinSnowballThrowState.Throw)
			{
				isPendingChargeSnowball = true;
			}
			else if (currentState == PenguinSnowballThrowState.Idle)
			{
				isPendingChargeSnowball = false;
				if (snowballAiming != null)
				{
					snowballAiming.Init();
				}
				anim.ResetTrigger(launchAnimTrigger);
				anim.ResetTrigger(chargeAnimTrigger);
				anim.SetTrigger(chargeAnimTrigger);
				setState(PenguinSnowballThrowState.Charge);
				LocomotionActionEvent action = default(LocomotionActionEvent);
				action.Type = LocomotionAction.ChargeThrow;
				action.Position = base.transform.position;
				action.Direction = base.transform.forward;
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.StartSnowballPowerMeter(mutableData.MaxChargeTime));
				sendNetworkMessage(action);
			}
		}

		public void LaunchSnowball(float _chargeTime = 0f)
		{
			if (isPendingChargeSnowball && currentState == PenguinSnowballThrowState.Throw)
			{
				isPendingChargeSnowball = false;
			}
			else if (currentState == PenguinSnowballThrowState.Charge)
			{
				chargeTime = _chargeTime;
				setState(PenguinSnowballThrowState.Throw);
				anim.SetTrigger(launchAnimTrigger);
				Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.StopSnowballPowerMeter(false));
			}
		}

		private void setState(PenguinSnowballThrowState newState)
		{
			currentState = newState;
		}

		public override void OnEnterIdle()
		{
			setState(PenguinSnowballThrowState.Idle);
			if (isPendingChargeSnowball)
			{
				ChargeSnowball();
			}
		}

		public void LateUpdate()
		{
			if (snowballInst.Snowball != null && !LocomotionUtils.IsThrowingSnowball(LocomotionUtils.GetAnimatorStateInfo(anim, mutableData.AnimLayerIndex)) && !LocomotionUtils.IsChargingSnowball(LocomotionUtils.GetAnimatorStateInfo(anim, mutableData.AnimLayerIndex)))
			{
				abortSnowball();
			}
		}

		private void abortSnowball()
		{
			if (snowballInst.Lifetime != null)
			{
				snowballInst.Lifetime.KillImmediately();
			}
			if (snowballAiming != null)
			{
				snowballAiming.StopPlaying();
			}
			snowballInst.Reset();
			LocomotionActionEvent action = default(LocomotionActionEvent);
			action.Type = LocomotionAction.CancelThrow;
			sendNetworkMessage(action);
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.StopSnowballPowerMeter(true));
		}

		private void sendNetworkMessage(LocomotionActionEvent action)
		{
			if (GetComponent<LocomotionBroadcaster>() != null)
			{
				Service.Get<INetworkServicesManager>().PlayerActionService.LocomotionAction(action);
			}
		}

		public void AnimEvent_SpawnSnowball()
		{
			if (!base.gameObject.IsDestroyed())
			{
				snowballInst.Snowball = SnowballManager.Instance.SpawnSnowball();
				if (snowballInst.Snowball != null)
				{
					snowballInst.Snowball.transform.SetParent(LauncherBone, false);
					snowballInst.Snowball.transform.localPosition = Vector3.zero;
					snowballInst.Snowball.OnAttached();
					snowballInst.Lifetime.Reset();
				}
			}
		}

		public void AnimEvent_ReleaseSnowball()
		{
			if (!base.gameObject.IsDestroyed())
			{
				if (snowballInst.Snowball == null)
				{
					AnimEvent_SpawnSnowball();
				}
				float d = mutableData.MinThrowSpeed + (mutableData.MaxThrowSpeed - mutableData.MinThrowSpeed) * Mathf.Clamp01(chargeTime / mutableData.MaxChargeTime);
				float d2 = mutableData.MinLiftForce + (mutableData.MaxLiftForce - mutableData.MinLiftForce) * Mathf.Clamp01(chargeTime / mutableData.MaxChargeTime);
				Vector3 a = base.transform.TransformDirection(mutableData.ThrowDirection.forward) * d;
				Vector3 b = Vector3.up * d2;
				snowballInst.LaunchVel = a + b;
				snowballInst.TrailAlpha = mutableData.MinTrailAlpha + (mutableData.MaxTrailAlpha - mutableData.MinTrailAlpha) * Mathf.Clamp01(chargeTime / mutableData.MaxChargeTime);
				if (mutableData.EnableAimAssist)
				{
					Vector3 position = snowballInst.Snowball.transform.position;
					Vector3 initialVelocity = mutableData.CharVelocityFactor * charController.velocity + snowballInst.LaunchVel;
					snowballInst.LaunchVel = AimAssist(initialVelocity, position);
				}
				LocomotionActionEvent action = default(LocomotionActionEvent);
				action.Type = LocomotionAction.LaunchThrow;
				action.Position = base.transform.position;
				action.Velocity = snowballInst.LaunchVel;
				action.Direction = base.transform.forward;
				sendNetworkMessage(action);
				ReleaseSnowball(snowballInst.Snowball.transform.position, snowballInst.LaunchVel, snowballInst.TrailAlpha);
			}
		}

		private Vector3 AimAssist(Vector3 initialVelocity, Vector3 initialPos)
		{
			Vector3 result = initialVelocity;
			Vector3 up = Vector3.up;
			float num = Vector3.Dot(initialVelocity, up);
			Vector3 vector = initialVelocity - num * up;
			float magnitude = vector.magnitude;
			vector /= magnitude;
			float num2 = -0.5f * Physics.gravity.y * num * num;
			RaycastHit[] array = Physics.BoxCastAll(initialPos, new Vector3(mutableData.AimAssistRaycastError.x, num2 + mutableData.AimAssistRaycastError.y, mutableData.AimAssistRaycastError.z), vector, Quaternion.LookRotation(vector), mutableData.AimAssistRange, mutableData.AimAssistCollisionLayers.value);
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 center = array[i].collider.bounds.center;
					Vector3 vector2 = center - base.transform.position;
					vector2.y = 0f;
					float num3 = Vector3.Dot(vector2, vector);
					float num4 = num3 / magnitude;
					float num5 = num * num4 + 0.5f * Physics.gravity.y * num4 * num4 + initialPos.y;
					float num6 = Mathf.Abs(num5 - center.y);
					if (num6 <= mutableData.AimAssistRaycastError.y)
					{
						vector2.Normalize();
						result = num * up + magnitude * vector2;
						break;
					}
				}
			}
			return result;
		}

		public void ReleaseSnowball(Vector3 position, Vector3 velocity, float trailAlpha)
		{
			if (snowballInst.Snowball != null)
			{
				snowballInst.Snowball.transform.position = position;
				snowballInst.Snowball.transform.SetParent(null);
				snowballInst.Snowball.OnDetached(dataEntityCollection.LocalPlayerSessionId, ref velocity, chargeTime, trailAlpha);
				snowballInst.Lifetime.OnSpawn();
				snowballInst.Reset();
				if (snowballAiming != null)
				{
					snowballAiming.StopPlaying();
				}
			}
		}

		public bool IsHoldingSnowball()
		{
			bool result = false;
			if (snowballInst.Snowball != null)
			{
				result = true;
			}
			return result;
		}

		public override void EnableSnowballThrow(bool enable)
		{
			anim.SetBool(AnimationHashes.Params.SnowballThrowEnabled, enable);
			if (!enable && snowballInst.Snowball != null)
			{
				CancelChargeSnowball();
			}
		}

		public void CancelChargeSnowball()
		{
			abortSnowball();
		}
	}
}
