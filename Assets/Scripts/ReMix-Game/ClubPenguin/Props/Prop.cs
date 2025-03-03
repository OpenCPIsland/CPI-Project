using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(PropControlsOverride))]
	[DisallowMultipleComponent]
	public class Prop : MonoBehaviour
	{
		public enum VisualTreatmentType
		{
			None,
			Solo,
			DeployLowInAir,
			DeployHighInAir,
			Shareable
		}

		public bool UseOnceImmediately = false;

		public bool StoreAfterUse = false;

		public bool PlayIdleAnimAfterUse = true;

		public VisualTreatmentType VisualTreatment = VisualTreatmentType.Solo;

		public bool PulsateInteractButton = true;

		public string TargetBoneName = "r_wrist_jnt";

		public float MaxDistanceFromUser = 0.5f;

		public bool CheckCollisions = true;

		public AnimatorOverrideController AnimOverrideController;

		public RuntimeAnimatorController AnimReplacementController;

		public bool IgnoreHoldAnimationComplete = false;

		public bool IgnoreStoreAnimationComplete = false;

		public CameraController CustomCamera = null;

		public iTween.EaseType RetrieveEaseType = iTween.EaseType.easeOutBounce;

		public float EaseTimeRetrieve = 1f;

		public iTween.EaseType StoreEaseType = iTween.EaseType.easeInOutCirc;

		public float EaseTimeStore = 1f;

		[HideInInspector]
		public Vector3 OnUseDestination;

		[HideInInspector]
		public PropUser PropUserRef;

		[HideInInspector]
		public bool UpdateUser = true;

		[HideInInspector]
		public string ExperienceInstanceId;

		[HideInInspector]
		public long OwnerId;

		[HideInInspector]
		public string PropId;

		[HideInInspector]
		public bool IsOwnerLocalPlayer;

		[HideInInspector]
		public PropDefinition PropDef;

		[HideInInspector]
		public bool IsUseCompleted = false;

		private Animator animator;

		private bool isPlayingAnimation;

		private System.Action animationCompletionHandler;

		private Vector3 fullScale;

		private CameraController customCameraInstance;

		private PropControlsOverride propControls;

		public PropControlsOverride PropControls
		{
			get
			{
				return propControls;
			}
		}

		public event Action<string> EActionEventReceived;

		public event System.Action EUsed;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			propControls = GetComponent<PropControlsOverride>();
			fullScale = base.transform.localScale;
			tweenRetrieve();
		}

		public void PerformAction(string action)
		{
			if (this.EActionEventReceived != null)
			{
				this.EActionEventReceived(action);
			}
		}

		private void Update()
		{
			if (UpdateUser && isPlayingAnimation && animator.GetCurrentAnimatorStateInfo(1).fullPathHash == AnimationHashes.States.TorsoIdle)
			{
				isPlayingAnimation = false;
				animationCompletionHandler();
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			restoreCamera();
			iTween.Stop(base.gameObject);
			this.EActionEventReceived = null;
			this.EUsed = null;
		}

		private void tweenRetrieve()
		{
			base.transform.localScale = Vector3.zero;
			iTween.ValueTo(base.gameObject, iTween.Hash("from", Vector3.zero, "to", fullScale, "time", EaseTimeRetrieve, "onupdatetarget", base.gameObject, "onupdate", "onUpdateTween", "easetype", RetrieveEaseType));
		}

		private void tweenStore()
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", fullScale, "to", Vector3.zero, "time", EaseTimeStore, "onupdatetarget", base.gameObject, "onupdate", "onUpdateTween", "easetype", StoreEaseType));
		}

		private void onUpdateTween(Vector3 newValue)
		{
			base.transform.localScale = newValue;
		}

		public void SetAnimatorMode(PropMode propMode, System.Action completionHandler, bool waitForAnimationComplete = true)
		{
			if (animator != null)
			{
				animator.SetInteger("PropMode", (int)propMode);
				if (waitForAnimationComplete)
				{
					animationCompletionHandler = completionHandler;
					isPlayingAnimation = true;
					return;
				}
				isPlayingAnimation = false;
				if (completionHandler != null)
				{
					completionHandler();
				}
			}
			else
			{
				CoroutineRunner.Start(delayTriggerHandler(completionHandler), this, "Delay for triggering prop removal");
			}
		}

		private IEnumerator delayTriggerHandler(System.Action completionHandler)
		{
			yield return null;
			completionHandler();
		}

		public void UseStarted(System.Action completionHandler)
		{
			SetAnimatorMode(PropMode.Use, completionHandler);
			if (this.EUsed != null)
			{
				this.EUsed();
			}
			switchToCamera();
		}

		public void StoreStarted(System.Action completionHandler)
		{
			tweenStore();
			SetAnimatorMode(PropMode.Store, completionHandler, !IgnoreStoreAnimationComplete);
		}

		private void switchToCamera()
		{
			if (IsOwnerLocalPlayer && CustomCamera != null && customCameraInstance == null)
			{
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				evt.Controller = (customCameraInstance = UnityEngine.Object.Instantiate(CustomCamera));
				Service.Get<EventDispatcher>().DispatchEvent(evt);
			}
		}

		private void restoreCamera()
		{
			if (IsOwnerLocalPlayer && customCameraInstance != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = customCameraInstance;
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				UnityEngine.Object.Destroy(customCameraInstance.gameObject);
				customCameraInstance = null;
			}
		}
	}
}
