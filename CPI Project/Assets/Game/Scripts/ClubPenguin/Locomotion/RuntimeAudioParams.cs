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
            Sprinting,
            Stopping,  // Skid
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

        private int prevStoppingAnimStateHash = -1;

        private static readonly int StoppingTagHash = Animator.StringToHash("Stopping");
        private static readonly int PivotingTagHash = Animator.StringToHash("Pivoting");

        private void Awake()
        {
            if (gameObject.CompareTag("Player") && SurfaceSamplingData != null)
            {
                anim = GetComponent<Animator>();
                tracker = GetComponent<LocomotionTracker>();
                locoStatus = LocomotionStatus.Unknown;
                CoroutineRunner.Start(SampleSurface(), this, "SampleSurface");
            }
            else
            {
                enabled = false;
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
                if (isActiveAndEnabled)
                {
                    Vector3 hitPoint = Vector3.zero;
                    int surfaceIndex = LocomotionUtils.SampleSurface(transform, SurfaceSamplingData, out hitPoint);
                    AnimatorStateInfo stateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);

                    if (LocomotionUtils.IsLocomoting(stateInfo))
                    {
                        if (LocomotionUtils.IsWalking(stateInfo))
                        {
                            if (locoStatus != LocomotionStatus.Walking || surfaceIndex != prevSurfaceTypeIndex)
                            {
                                locoStatus = LocomotionStatus.Walking;
                                PlaySurfaceSwitch(SurfaceSamplingData.DefaultWalkSwitch, surfaceIndex, data => data.WalkSwitch);
                            }
                        }
                        else if (LocomotionUtils.IsSprinting(stateInfo))
                        {
                            if (locoStatus != LocomotionStatus.Sprinting || surfaceIndex != prevSurfaceTypeIndex)
                            {
                                locoStatus = LocomotionStatus.Sprinting;
                                PlaySurfaceSwitch(SurfaceSamplingData.DefaultSprintSwitch, surfaceIndex, data => data.SprintSwitch);
                            }
                        }
                        else if (stateInfo.tagHash == StoppingTagHash || stateInfo.tagHash == PivotingTagHash)
                        {
                            int curStopStateHash = stateInfo.fullPathHash;
                            if (locoStatus != LocomotionStatus.Stopping || surfaceIndex != prevSurfaceTypeIndex || curStopStateHash != prevStoppingAnimStateHash)
                            {
                                locoStatus = LocomotionStatus.Stopping;
                                prevStoppingAnimStateHash = curStopStateHash;
                                PlaySurfaceEvent(SurfaceSamplingData.DefaultStoppingSwitch, surfaceIndex, data => data.StoppingSwitch);
                            }
                        }
                        else if (locoStatus != LocomotionStatus.Jogging || surfaceIndex != prevSurfaceTypeIndex)
                        {
                            locoStatus = LocomotionStatus.Jogging;
                            PlaySurfaceSwitch(SurfaceSamplingData.DefaultJogSwitch, surfaceIndex, data => data.JogSwitch);
                        }
                    }
                    else if (LocomotionUtils.IsInAir(stateInfo) || LocomotionUtils.IsLanding(stateInfo))
                    {
                        if (locoStatus != LocomotionStatus.InAir || surfaceIndex != prevSurfaceTypeIndex)
                        {
                            locoStatus = LocomotionStatus.InAir;
                            PlaySurfaceSwitch(SurfaceSamplingData.DefaultLandSwitch, surfaceIndex, data => data.LandSwitch);
                        }
                    }
                    else if (tracker.IsCurrentControllerOfType<SlideController>())
                    {
                        if (locoStatus != LocomotionStatus.Tubing || surfaceIndex != prevSurfaceTypeIndex)
                        {
                            locoStatus = LocomotionStatus.Tubing;
                            PlaySurfaceSwitch(SurfaceSamplingData.DefaultTubeSlideLoopSwitch, surfaceIndex, data => data.TubeSlideLoopSwitch);
                        }
                    }
                    else
                    {
                        locoStatus = LocomotionStatus.Unknown;
                    }

                    prevSurfaceTypeIndex = surfaceIndex;
                }

                yield return new WaitForSeconds(sampleTime);
            }
        }

        private void PlaySurfaceSwitch(SurfaceEffectsData.AudioSwitch defaultSwitch, int surfaceIndex, System.Func<SurfaceEffectsData.Effect, SurfaceEffectsData.AudioSwitch> switchSelector)
        {
            if (surfaceIndex >= 0 && surfaceIndex < SurfaceSamplingData.Effects.Length)
            {
                var effect = SurfaceSamplingData.Effects[surfaceIndex];
                var audioSwitch = switchSelector(effect);
                if (!string.IsNullOrEmpty(audioSwitch.SwitchValue))
                {
                    EventManager.Instance.PostEvent(audioSwitch.EventName, EventAction.SetSwitch, audioSwitch.SwitchValue, gameObject);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(defaultSwitch.SwitchValue))
            {
                EventManager.Instance.PostEvent(defaultSwitch.EventName, EventAction.SetSwitch, defaultSwitch.SwitchValue, gameObject);
            }
        }

        private void PlaySurfaceEvent(SurfaceEffectsData.AudioSwitch defaultSwitch, int surfaceIndex, System.Func<SurfaceEffectsData.Effect, SurfaceEffectsData.AudioSwitch> eventSelector)
        {
            if (surfaceIndex >= 0 && surfaceIndex < SurfaceSamplingData.Effects.Length)
            {
                var effect = SurfaceSamplingData.Effects[surfaceIndex];
                var audioEvent = eventSelector(effect);
                if (!string.IsNullOrEmpty(audioEvent.EventName))
                {
                    EventManager.Instance.PostEvent(audioEvent.EventName, EventAction.PlaySound, gameObject);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(defaultSwitch.EventName))
            {
                EventManager.Instance.PostEvent(defaultSwitch.EventName, EventAction.PlaySound, gameObject);
            }
        }
    }
}
