using Disney.Kelowna.Common;
using Fabric;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RuntimeAudioParams : MonoBehaviour
	{
		public enum LocomotionStatus
		{
			Unknown,
			Walking,
			Jogging,
			Tubing,
			InAir
		}

		private static readonly float sampleTime = 0.2f;

		[Header("Surface Sampling")]
		public SurfaceEffectsData SurfaceSamplingData;

		private LocomotionTracker tracker;

		private Animator anim;

		private int prevSurfaceTypeIndex = -2;

		private LocomotionStatus locoStatus;

		private void Awake()
		{
			if (base.gameObject.CompareTag("Player") && SurfaceSamplingData != null)
			{
				anim = GetComponent<Animator>();
				tracker = GetComponent<LocomotionTracker>();
				locoStatus = LocomotionStatus.Unknown;
				CoroutineRunner.Start(SampleSurface(), this, "SampleSurface");
			}
			else
			{
				base.enabled = false;
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private IEnumerator SampleSurface()
		{
			while (true)
			{
				if (base.isActiveAndEnabled)
				{
					Vector3 hitPoint = Vector3.zero;
					int num = LocomotionUtils.SampleSurface(base.transform, SurfaceSamplingData, out hitPoint);
					AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
					if (LocomotionUtils.IsLocomoting(animatorStateInfo))
					{
						if (LocomotionUtils.IsWalking(animatorStateInfo))
						{
							if (locoStatus != LocomotionStatus.Walking || num != prevSurfaceTypeIndex)
							{
								locoStatus = LocomotionStatus.Walking;
								if (num >= 0)
								{
									if (!string.IsNullOrEmpty(SurfaceSamplingData.Effects[num].WalkSwitch.SwitchValue))
									{
										EventManager.Instance.PostEvent(SurfaceSamplingData.Effects[num].WalkSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.Effects[num].WalkSwitch.SwitchValue, base.gameObject);
									}
								}
								else if (!string.IsNullOrEmpty(SurfaceSamplingData.DefaultWalkSwitch.EventName))
								{
									EventManager.Instance.PostEvent(SurfaceSamplingData.DefaultWalkSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.DefaultWalkSwitch.SwitchValue, base.gameObject);
								}
							}
						}
						else if (locoStatus != LocomotionStatus.Jogging || num != prevSurfaceTypeIndex)
						{
							locoStatus = LocomotionStatus.Jogging;
							if (num >= 0)
							{
								if (!string.IsNullOrEmpty(SurfaceSamplingData.Effects[num].JogSwitch.SwitchValue))
								{
									EventManager.Instance.PostEvent(SurfaceSamplingData.Effects[num].JogSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.Effects[num].JogSwitch.SwitchValue, base.gameObject);
								}
							}
							else if (!string.IsNullOrEmpty(SurfaceSamplingData.DefaultJogSwitch.EventName))
							{
								EventManager.Instance.PostEvent(SurfaceSamplingData.DefaultJogSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.DefaultJogSwitch.SwitchValue, base.gameObject);
							}
						}
					}
					else if (LocomotionUtils.IsInAir(animatorStateInfo) || LocomotionUtils.IsLanding(animatorStateInfo))
					{
						if (locoStatus != LocomotionStatus.InAir || num != prevSurfaceTypeIndex)
						{
							locoStatus = LocomotionStatus.InAir;
							if (num >= 0)
							{
								if (!string.IsNullOrEmpty(SurfaceSamplingData.Effects[num].LandSwitch.SwitchValue))
								{
									EventManager.Instance.PostEvent(SurfaceSamplingData.Effects[num].LandSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.Effects[num].LandSwitch.SwitchValue, base.gameObject);
								}
							}
							else if (!string.IsNullOrEmpty(SurfaceSamplingData.DefaultLandSwitch.EventName))
							{
								EventManager.Instance.PostEvent(SurfaceSamplingData.DefaultLandSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.DefaultLandSwitch.SwitchValue, base.gameObject);
							}
						}
					}
					else if (tracker.IsCurrentControllerOfType<SlideController>())
					{
						if (locoStatus != LocomotionStatus.Tubing || num != prevSurfaceTypeIndex)
						{
							locoStatus = LocomotionStatus.Tubing;
							if (num >= 0)
							{
								if (!string.IsNullOrEmpty(SurfaceSamplingData.Effects[num].TubeSlideLoopSwitch.SwitchValue))
								{
									EventManager.Instance.PostEvent(SurfaceSamplingData.Effects[num].TubeSlideLoopSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.Effects[num].TubeSlideLoopSwitch.SwitchValue, base.gameObject);
								}
							}
							else if (!string.IsNullOrEmpty(SurfaceSamplingData.DefaultTubeSlideLoopSwitch.EventName))
							{
								EventManager.Instance.PostEvent(SurfaceSamplingData.DefaultTubeSlideLoopSwitch.EventName, EventAction.SetSwitch, SurfaceSamplingData.DefaultTubeSlideLoopSwitch.SwitchValue, base.gameObject);
							}
						}
					}
					else
					{
						locoStatus = LocomotionStatus.Unknown;
					}
					prevSurfaceTypeIndex = num;
				}
				yield return new WaitForSeconds(sampleTime);
			}
		}
	}
}
