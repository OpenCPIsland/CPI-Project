using ClubPenguin.Core;
using ClubPenguin.Interactables;
using ClubPenguin.Participation;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public static class LocomotionUtils
	{
		public enum AxisIndex
		{
			X,
			Y,
			Z
		}

		public const int BASE_LAYER_INDEX = 0;

		public const int TORSO_LAYER_INDEX = 1;

		public static readonly string WaterVolumeTag = "WaterVolume";

		public static readonly string GroundVolumeTag = "GroundVolume";

		public static readonly float DefaultGravity = 9.8f;

		public static Vector3 StickInputToWorldSpaceTransform(Vector2 stickInput, AxisIndex forwardAxis)
		{
			Vector3 wsForward;
			Vector3 wsUp;
			StickInputToWorldSpaceTransform(stickInput, out wsForward, out wsUp, forwardAxis);
			return wsForward;
		}

		public static void StickInputToWorldSpaceTransform(Vector2 stickInput, out Vector3 wsForward, out Vector3 wsUp, AxisIndex forwardAxis)
		{
			wsForward = Vector3.zero;
			wsUp = Vector3.zero;
			if (stickInput != Vector2.zero)
			{
				Transform transform = Camera.main.transform;
				switch (forwardAxis)
				{
				case AxisIndex.Y:
					wsForward = stickInput.y * transform.up + stickInput.x * transform.right;
					wsForward.z = 0f;
					wsForward.Normalize();
					wsUp.x = 0f - wsForward.y;
					wsUp.y = wsForward.x;
					wsUp.z = 0f;
					break;
				case AxisIndex.Z:
				{
					Vector3 normalized = Vector3.Scale(transform.forward, new Vector3(1f, 0f, 1f)).normalized;
					wsForward = stickInput.y * normalized + stickInput.x * transform.right;
					wsForward.y = 0f;
					wsForward.Normalize();
					wsUp = Vector3.up;
					break;
				}
				}
			}
		}

		public static Vector3 GetUpVector(ref Vector3 wsForward, AxisIndex forwardAxis)
		{
			Vector3 result = Vector3.zero;
			if (wsForward != Vector3.zero)
			{
				switch (forwardAxis)
				{
				case AxisIndex.Y:
					result.x = 0f - wsForward.y;
					result.y = wsForward.x;
					result.z = 0f;
					break;
				case AxisIndex.Z:
					result = Camera.main.transform.up;
					break;
				default:
					Log.LogError(typeof(LocomotionUtils), "GetUpVector() doesn't currently implement AxisIndex.X.");
					break;
				}
			}
			return result;
		}

		public static AnimatorStateInfo GetAnimatorStateInfo(Animator animator, int layerIndex = 0)
		{
			return animator.IsInTransition(layerIndex) ? animator.GetNextAnimatorStateInfo(layerIndex) : animator.GetCurrentAnimatorStateInfo(layerIndex);
		}

		public static void UnEquipProp(GameObject player)
		{
			if (player.IsDestroyed())
			{
				return;
			}
			AvatarDataHandle component = player.GetComponent<AvatarDataHandle>();
			if (!(component != null) || !component.IsLocalPlayer)
			{
				return;
			}
			InvitationalItemExperience componentInChildren = player.GetComponentInChildren<InvitationalItemExperience>();
			if (componentInChildren != null)
			{
				UnityEngine.Object.Destroy(componentInChildren.gameObject);
			}
			PropUser component2 = player.GetComponent<PropUser>();
			if (component2 != null && component2.Prop != null)
			{
				PropCancel component3 = component2.Prop.GetComponent<PropCancel>();
				if (component3 != null)
				{
					component3.UnequipProp(true);
				}
			}
		}

		public static int SampleSurface(Transform transform, SurfaceEffectsData data, out Vector3 hitPoint)
		{
			hitPoint = Vector3.zero;
			int result = -1;
			RaycastHit hitInfo;
			if (Physics.Raycast(transform.position + Vector3.up * 0.9f, Vector3.down, out hitInfo, 2f, LayerConstants.GetSurfaceSamplerLayerCollisionMask()))
			{
				hitPoint = hitInfo.point;
				result = GetSurfaceType(ref hitInfo, data);
			}
			return result;
		}

		public static int GetSurfaceType(ref RaycastHit hit, SurfaceEffectsData data)
		{
			int result = -1;
			if (hit.collider != null)
			{
				int num = 1 << hit.collider.gameObject.layer;
				for (int i = 0; i < data.Effects.Length; i++)
				{
					if (num == data.Effects[i].SurfaceLayer.value)
					{
						result = i;
						break;
					}
					if (!string.IsNullOrEmpty(data.Effects[i].SurfaceTag) && hit.collider.CompareTag(data.Effects[i].SurfaceTag))
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsLocomoting(AnimatorStateInfo stateInfo)
		{
			return IsWalking(stateInfo) || IsJogging(stateInfo) || IsSprinting(stateInfo) || IsStopping(stateInfo) || IsPivoting(stateInfo);
		}

		public static bool IsWalking(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Walking;
		}

		public static bool IsJogging(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Jogging;
		}

		public static bool IsSprinting(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Sprinting;
		}

		public static bool IsStopping(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Stopping;
		}

		public static bool IsPivoting(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Pivoting;
		}

		public static bool IsIdling(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Idling;
		}

		public static bool IsInAir(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.InAir;
		}

		public static bool IsLanding(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Landing;
		}

		public static bool IsReactingToHit(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.ReactingToHit;
		}

		public static bool IsTurboing(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.Turboing;
		}

		public static bool IsThrowingSnowball(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.ThrowingSnowball;
		}

		public static bool IsChargingSnowball(AnimatorStateInfo stateInfo)
		{
			return stateInfo.tagHash == AnimationHashes.Tags.ChargingSnowball;
		}

		public static bool IsHolding(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.TorsoHold;
		}

		public static bool IsCelebrating(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.TorsoCelebration;
		}

		public static bool IsUsing(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.TorsoUse;
		}

		public static bool IsOffering(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.TorsoOffer;
		}

		public static bool IsRetrieving(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.TorsoRetrieve;
		}

		public static bool IsSwimmingIdle(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.Swim.SwimIdle;
		}

		public static bool IsSwimming(AnimatorStateInfo stateInfo)
		{
			return stateInfo.fullPathHash == AnimationHashes.States.Swim.SwimIdle || stateInfo.fullPathHash == AnimationHashes.States.Swim.SwimState || stateInfo.fullPathHash == AnimationHashes.States.Swim.SwimTorpedo || stateInfo.fullPathHash == AnimationHashes.States.Swim.WaterTunnel || stateInfo.fullPathHash == AnimationHashes.States.Swim.SwimTakeDamage || stateInfo.fullPathHash == AnimationHashes.States.Swim.Resurface || stateInfo.fullPathHash == AnimationHashes.States.Swim.QuickResurface;
		}

		public static bool CanPlaySizzle(GameObject go)
		{
			bool result = false;
			if (go != null)
			{
				ParticipationController component = go.GetComponent<ParticipationController>();
				if (component == null || !component.IsInteracting())
				{
					RunController component2 = go.GetComponent<RunController>();
					if (component2 != null && component2.enabled)
					{
						Animator component3 = go.GetComponent<Animator>();
						if (component3 != null)
						{
							AnimatorStateInfo currentAnimatorStateInfo = component3.GetCurrentAnimatorStateInfo(AnimationHashes.Layers.Base);
							result = (IsIdling(currentAnimatorStateInfo) || currentAnimatorStateInfo.IsTag("Sizzling"));
						}
					}
				}
			}
			return result;
		}

		public static float SignedAngle(Vector2 from, Vector2 to)
		{
			float y = from.x * to.y - from.y * to.x;
			return (0f - Mathf.Atan2(y, Vector2.Dot(from, to))) * 57.29578f;
		}

		public static IEnumerator nudgePlayer(LocomotionTracker locomotionTracker, System.Action nudgeCompleted = null)
		{
			float finishTime = Time.time + UnityEngine.Random.Range(0.2f, 0.4f);
			Vector2 down = Vector2.down;
			Vector2 direction;
			switch (UnityEngine.Random.Range(1, 5))
			{
			case 1:
				direction = Vector2.up;
				break;
			case 2:
				direction = Vector2.left;
				break;
			case 3:
				direction = Vector2.right;
				break;
			default:
				direction = Vector2.down;
				break;
			}
			while (Time.time < finishTime && !locomotionTracker.gameObject.IsDestroyed())
			{
				locomotionTracker.GetCurrentController().Steer(direction);
				yield return null;
			}
			if (nudgeCompleted != null)
			{
				nudgeCompleted();
			}
		}
	}
}
