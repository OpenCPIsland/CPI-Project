using ClubPenguin.Cinematography;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class LevelUpParticles : MonoBehaviour
	{
		public delegate void LevelUpParticlesCompleteHandler();

		private CameraController profileCameraController;

		private GameObject playerPenguinGO;

		private readonly Vector3 PENGUIN_FRAMER_OFFSET = new Vector3(0f, 0.4f, 0f);

		private bool wasCameraChanged = false;

		public event LevelUpParticlesCompleteHandler OnLevelUpParticlesComplete;

		public IEnumerator Start()
		{
			playerPenguinGO = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (playerPenguinGO != null)
			{
				RunController runController = playerPenguinGO.GetComponent<RunController>();
				ParticipationController participationController = playerPenguinGO.GetComponent<ParticipationController>();
				if (runController != null && runController.enabled && participationController != null && (participationController.GetParticipationData().CurrentParticipationState == ParticipationState.Ready || participationController.GetParticipationData().CurrentParticipationState == ParticipationState.Pending))
				{
					Animator anim = playerPenguinGO.GetComponent<Animator>();
					if (anim != null)
					{
						playerPenguinGO.GetComponent<Animator>().SetTrigger("LevelUp");
						focusCameraOnPenguin();
						yield return null;
						yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
					}
				}
			}
			Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			if (wasCameraChanged)
			{
				resetCamera();
			}
			if (this.OnLevelUpParticlesComplete != null)
			{
				this.OnLevelUpParticlesComplete();
			}
		}

		private void focusCameraOnPenguin()
		{
			if (playerPenguinGO != null)
			{
				GameObject gameObject = new GameObject("ProfileCamera");
				gameObject.AddComponent<FixedPositionGoalPlanner>();
				gameObject.AddComponent<FixedOffsetFramer>().Offset = PENGUIN_FRAMER_OFFSET;
				profileCameraController = gameObject.AddComponent<CameraController>();
				Vector3 forward = Camera.main.transform.position - playerPenguinGO.transform.position;
				forward.y = 0f;
				playerPenguinGO.transform.rotation = Quaternion.LookRotation(forward);
				gameObject.transform.position = playerPenguinGO.transform.position + forward.normalized * 2f + new Vector3(0f, 2f, 0f);
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				evt.Controller = profileCameraController;
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				wasCameraChanged = true;
			}
		}

		private void resetCamera()
		{
			if (profileCameraController != null && profileCameraController.gameObject != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = profileCameraController;
				EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
				if (eventDispatcher != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(evt);
					Object.Destroy(profileCameraController.gameObject);
				}
			}
		}
	}
}
