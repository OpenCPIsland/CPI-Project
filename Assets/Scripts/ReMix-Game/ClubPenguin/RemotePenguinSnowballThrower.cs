using ClubPenguin.Cinematography;
using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePenguinSnowballThrower : AbstractPenguinSnowballThrower
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

		private abstract class SnowballCommand
		{
			public abstract void Execute();
		}

		private class ChargeSnowballCommand : SnowballCommand
		{
			private readonly RemotePenguinSnowballThrower thrower;

			public ChargeSnowballCommand(RemotePenguinSnowballThrower thrower)
			{
				this.thrower = thrower;
			}

			public override void Execute()
			{
				thrower.ChargeSnowball();
			}
		}

		private class LaunchSnowballCommand : SnowballCommand
		{
			private readonly RemotePenguinSnowballThrower thrower;

			private readonly Vector3 velocity;

			public LaunchSnowballCommand(RemotePenguinSnowballThrower thrower, Vector3 velocity)
			{
				this.thrower = thrower;
				this.velocity = velocity;
			}

			public override void Execute()
			{
				thrower.LaunchSnowball(velocity);
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

		private float chargeTime = 0f;

		private Animator anim;

		private int chargeAnimTrigger;

		private int launchAnimTrigger;

		private PenguinSnowballThrowState currentState;

		private Queue<SnowballCommand> commandQueue;

		public void OnValidate()
		{
		}

		private void Awake()
		{
			anim = GetComponent<Animator>();
			commandQueue = new Queue<SnowballCommand>();
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
			if (currentState != 0)
			{
				commandQueue.Enqueue(new ChargeSnowballCommand(this));
				return;
			}
			anim.SetTrigger(chargeAnimTrigger);
			setState(PenguinSnowballThrowState.Charge);
			if (commandQueue.Count > 0)
			{
				if (commandQueue.Peek() is LaunchSnowballCommand)
				{
					commandQueue.Dequeue().Execute();
					return;
				}
				commandQueue.Clear();
				anim.SetTrigger(launchAnimTrigger);
			}
		}

		public void LaunchSnowball(Vector3 velocity)
		{
			if (currentState != PenguinSnowballThrowState.Charge)
			{
				commandQueue.Enqueue(new LaunchSnowballCommand(this, velocity));
				return;
			}
			currentState = PenguinSnowballThrowState.Throw;
			snowballInst.LaunchVel = velocity;
			anim.SetTrigger(launchAnimTrigger);
		}

		private void setState(PenguinSnowballThrowState newState)
		{
			currentState = newState;
		}

		public override void OnEnterIdle()
		{
			setState(PenguinSnowballThrowState.Idle);
			if (commandQueue.Count > 0)
			{
				if (commandQueue.Peek() is ChargeSnowballCommand)
				{
					commandQueue.Dequeue().Execute();
				}
				else
				{
					commandQueue.Clear();
				}
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
			snowballInst.Reset();
		}

		public void AnimEvent_SpawnSnowball()
		{
			if (!base.gameObject.IsDestroyed() && CameraCullingMaskHelper.IsLayerOn(Camera.main, "RemotePlayer"))
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
			if (!base.gameObject.IsDestroyed() && CameraCullingMaskHelper.IsLayerOn(Camera.main, "RemotePlayer"))
			{
				if (snowballInst.Snowball == null)
				{
					AnimEvent_SpawnSnowball();
				}
				ReleaseSnowball(snowballInst.Snowball.transform.position, snowballInst.LaunchVel, snowballInst.TrailAlpha);
			}
		}

		public void ReleaseSnowball(Vector3 position, Vector3 velocity, float trailAlpha)
		{
			if (snowballInst.Snowball != null)
			{
				snowballInst.Snowball.transform.position = position;
				snowballInst.Snowball.transform.SetParent(null);
				snowballInst.Snowball.OnDetached(0L, ref velocity, chargeTime, trailAlpha);
				snowballInst.Lifetime.OnSpawn();
				snowballInst.Reset();
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
