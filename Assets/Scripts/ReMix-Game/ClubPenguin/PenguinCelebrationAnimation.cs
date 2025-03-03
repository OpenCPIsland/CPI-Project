using ClubPenguin.Cinematography;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class PenguinCelebrationAnimation : MonoBehaviour
	{
		public string AnimationTrigger;

		public float DelaySeconds;

		public Vector3 FramerOffset = new Vector3(0f, 0.4f, 0f);

		private GameObject localPlayerGameObject;

		private CameraController cameraController;

		private bool wasCameraMoved;

		public event System.Action OnAnimationStarted;

		public event Action<bool> OnAnimationEnded;

		private IEnumerator Start()
		{
			bool animationCompleted = false;
			if (DelaySeconds > 0f)
			{
				yield return new WaitForSeconds(DelaySeconds);
			}
			localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				RunController runController = localPlayerGameObject.GetComponent<RunController>();
				ParticipationController participationController = localPlayerGameObject.GetComponent<ParticipationController>();
				if (runController != null && runController.enabled && participationController != null && (participationController.GetParticipationData().CurrentParticipationState == ParticipationState.Ready || participationController.GetParticipationData().CurrentParticipationState == ParticipationState.Pending))
				{
					Animator animator = localPlayerGameObject.GetComponent<Animator>();
					if (animator != null)
					{
						localPlayerGameObject.GetComponent<Animator>().SetTrigger(AnimationTrigger);
						focusCameraOnPenguin();
						if (this.OnAnimationStarted != null)
						{
							this.OnAnimationStarted();
						}
						yield return null;
						yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
						animationCompleted = true;
					}
				}
			}
			if (wasCameraMoved)
			{
				resetCamera();
			}
			if (this.OnAnimationEnded != null)
			{
				this.OnAnimationEnded(animationCompleted);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void focusCameraOnPenguin()
		{
			if (localPlayerGameObject != null)
			{
				GameObject gameObject = new GameObject("CelebrationCameraController");
				gameObject.AddComponent<FixedPositionGoalPlanner>();
				gameObject.AddComponent<FixedOffsetFramer>().Offset = FramerOffset;
				cameraController = gameObject.AddComponent<CameraController>();
				Vector3 forward = Camera.main.transform.position - localPlayerGameObject.transform.position;
				forward.y = 0f;
				localPlayerGameObject.transform.rotation = Quaternion.LookRotation(forward);
				gameObject.transform.position = localPlayerGameObject.transform.position + forward.normalized * 2f + new Vector3(0f, 2f, 0f);
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				evt.Controller = cameraController;
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				wasCameraMoved = true;
			}
		}

		private void resetCamera()
		{
			if (cameraController != null && cameraController.gameObject != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = cameraController;
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				UnityEngine.Object.Destroy(cameraController.gameObject);
			}
		}

		private void OnDestroy()
		{
			this.OnAnimationStarted = null;
			this.OnAnimationEnded = null;
		}
	}
}
