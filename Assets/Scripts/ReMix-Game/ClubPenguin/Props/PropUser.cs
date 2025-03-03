using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Interactables;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(LocomotionEventBroadcaster))]
	[RequireComponent(typeof(Animator))]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LocomotionTracker))]
	[RequireComponent(typeof(Rig))]
	public class PropUser : MonoBehaviour
	{
		public const int NULL_PENDING_EXPERIENCE_ID = -1;

		private const int TORSO_LAYER_INDEX = 1;

		public long PendingExperienceId = -1L;

		public bool IsPropUseCompleted = true;

		private Animator anim;

		private int oldAnimState;

		private bool isUserModeCompleted;

		private bool isPropAnimCompleted = true;

		private bool stateTransitionComplete;

		private bool waitForModeAnimCompletion;

		private List<PropInteractionRequest> propInteractionQueue;

		private PropInteractionRequest currentRequest;

		private RuntimeAnimatorController defaultAnimatorController;

		private CPDataEntityCollection dataEntityCollection;

		private HeldObjectsData heldObjectsData;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private LocomotionTracker locomotionTracker;

		public Prop Prop
		{
			get;
			private set;
		}

		public DataEntityHandle PlayerHandle
		{
			get
			{
				DataEntityHandle handle;
				if (!AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
				{
					return DataEntityHandle.NullHandle;
				}
				return handle;
			}
		}

        public DataEntityHandle PlayerHandle2
        {
            get
            {
                DataEntityHandle handle2;
                if (!AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle2))
                {
                    return DataEntityHandle.NullHandle;
                }
                return handle2;
            }
        }

        public event Action<Prop> EPropSpawned;

		public event Action<Prop> EPropRetrieved;

		public event Action<Prop> EPropUseStarted;

		public event Action<Prop> EPropUseCompleted;

		public event Action<Prop> EPropRemoved;

		public event Action<Prop> EPropStored;

		public event System.Action EPropUserEnteredIdle;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			locomotionEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
			locomotionEventBroadcaster.OnDoActionEvent += OnLocomotionBroadcasterDoAction;
			locomotionTracker = GetComponent<LocomotionTracker>();
			propInteractionQueue = new List<PropInteractionRequest>();
		}

		private void OnLocomotionBroadcasterDoAction(LocomotionController.LocomotionAction action, object userData = null)
		{
			if (!(Prop != null))
			{
				Debug.Log("not passed. PropUser OnLocomotionBroadcasterDoAction");
				return;
			}
			Debug.Log("passed. PropUser OnLocomotionBroadcasterDoAction");
			switch (action)
			{
			case LocomotionController.LocomotionAction.Action1:
			case LocomotionController.LocomotionAction.Action2:
			case LocomotionController.LocomotionAction.Action3:
			Debug.Log("passed 1 . PropUser OnLocomotionBroadcasterDoAction");
				locomotionTracker.SetCurrentController<RunController>();
				Debug.Log("passed 2 . PropUser OnLocomotionBroadcasterDoAction");
				if (Prop.PropDef.PropType == PropDefinition.PropTypes.InteractiveObject && Prop.IsOwnerLocalPlayer)
				{
					Debug.Log("passed 3 . PropUser OnLocomotionBroadcasterDoAction");
					Service.Get<ICPSwrveService>().Action("game.interactive_object", Prop.PropDef.name, action.ToString());
				}
				break;
			}
		}

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			oldAnimState = anim.GetCurrentAnimatorStateInfo(1).fullPathHash;
			defaultAnimatorController = anim.runtimeAnimatorController;
			if (dataEntityCollection.TryGetComponent(PlayerHandle, out heldObjectsData))
			{
				if (!heldObjectsData.IsInvitationalExperience)
				{
					onPlayerHeldItemChanged(heldObjectsData.HeldObject);
				}
				addListeners();
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<HeldObjectsData>>(onHeldObjectAdded);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			removeListeners();
			this.EPropRetrieved = null;
			this.EPropUseStarted = null;
			this.EPropUseCompleted = null;
			this.EPropRemoved = null;
			this.EPropStored = null;
			this.EPropUserEnteredIdle = null;
		}

		public void OnPenguinAnimationTorsoIdleEnter()
		{
			if (this.EPropUserEnteredIdle != null)
			{
				this.EPropUserEnteredIdle();
			}
			IsPropUseCompleted = true;
		}

		private bool onHeldObjectAdded(DataEntityEvents.ComponentAddedEvent<HeldObjectsData> evt)
		{
			if (evt.Handle == PlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<HeldObjectsData>>(onHeldObjectAdded);
				heldObjectsData = evt.Component;
				addListeners();
			}
			return false;
		}

		private void addListeners()
		{
			heldObjectsData.PlayerHeldObjectChanged += onPlayerHeldItemChanged;
		}

		private void removeListeners()
		{
			if (heldObjectsData != null)
			{
				heldObjectsData.PlayerHeldObjectChanged -= onPlayerHeldItemChanged;
			}
			locomotionEventBroadcaster.OnDoActionEvent -= OnLocomotionBroadcasterDoAction;
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<HeldObjectsData>>(onHeldObjectAdded);
			}
		}

		private void onPlayerHeldItemChanged(DHeldObject heldObject)
		{
			DataEntityHandle playerHandle = PlayerHandle;
			SessionIdData component;
			if (!heldObjectsData.IsInvitationalExperience && dataEntityCollection.TryGetComponent(playerHandle, out component))
			{
				Service.Get<PropService>().PlayerHeldObjectChanged(component.SessionId, heldObject);
			}
		}

		public void OnPropAction(string actionName)
		{
			Prop.PerformAction(actionName);
		}

		private void StartUserPropMode(PropMode propMode, bool waitForModeAnimCompletion)
		{
			this.waitForModeAnimCompletion = waitForModeAnimCompletion;
			stateTransitionComplete = false;
			isUserModeCompleted = false;
			isPropAnimCompleted = false;
			anim.SetInteger("PropMode", (int)propMode);
			if (propMode == PropMode.Retrieve && Prop.PropDef != null)
			{
				anim.SetFloat(AnimationHashes.Params.PropRetrieveType, (float)Prop.PropDef.RetrieveAnimType);
			}
		}

		public void RemoveProp()
		{
			if (Prop == null)
			{
				StartUserPropMode(PropMode.Idle, false);
				return;
			}
			propInteractionQueue.Clear();
			Prop.UpdateUser = false;
			stopInteractingWithProp();
			if (Prop.PropDef.PropType != 0 || Prop.PropDef.ExperienceType != 0)
			{
				IsPropUseCompleted = true;
			}
			Prop prop = Prop;
			Prop = null;
			if (this.EPropRemoved != null)
			{
				this.EPropRemoved(prop);
			}
			StartUserPropMode(PropMode.Idle, false);
		}

		private void stopInteractingWithProp()
		{
			GameObjectReferenceData component;
			if (!PlayerHandle.IsNull && dataEntityCollection.TryGetComponent(PlayerHandle, out component))
			{
				ParticipationController component2 = component.GameObject.GetComponent<ParticipationController>();
				if (component2 != null)
				{
					component2.StopParticipation(new ParticipationRequest(ParticipationRequest.Type.Stop, Prop.gameObject, "PropUser"));
				}
			}
		}

		public void ResetAnimController()
		{
			CoroutineRunner.Start(resetAnimController(), this, "resetAnimController");
		}

		private IEnumerator resetAnimController()
		{
			yield return null;
			if (anim.runtimeAnimatorController != defaultAnimatorController && Prop == null)
			{
				AnimatorExtensions.AnimatorState state = anim.SaveState();
				anim.runtimeAnimatorController = defaultAnimatorController;
				anim.RestoreState(ref state);
				anim.Update(0f);
			}
		}

		private void parentPropToTargetBone(Prop prop)
		{
			Rig component = GetComponent<Rig>();
			Transform transform = component[prop.TargetBoneName];
			if (transform == null)
			{
				throw new ArgumentException("Could not find user bone with name: " + prop.TargetBoneName, "prop.TargetBoneName");
			}
			prop.transform.SetParent(transform, false);
		}

		public IEnumerator RetrieveProp(Prop propToRetrieve)
		{
			yield return CoroutineRunner.Start(performPropRetrieve(propToRetrieve, default(Vector3)), this, "performPropRetrieve");
		}

		public IEnumerator RetrievePropWithImmediateUseDest(Prop propToRetrieve, Vector3 immediateUseDest)
		{
			yield return CoroutineRunner.Start(performPropRetrieve(propToRetrieve, immediateUseDest), this, "performPropRetrieve");
		}

		private IEnumerator performPropRetrieve(Prop propToRetrieve, Vector3 immediateUseDestination)
		{
			while (anim != null && anim.GetCurrentAnimatorStateInfo(1).fullPathHash != AnimationHashes.States.TorsoIdle)
			{
				yield return null;
			}
			if (anim == null || base.gameObject.IsDestroyed() || Prop != null)
			{
				yield break;
			}
			Prop = propToRetrieve;
			parentPropToTargetBone(Prop);
			Prop.PropUserRef = this;
			CameraCullingMaskHelper.SetLayerRecursive(Prop.transform, LayerMask.LayerToName(base.gameObject.layer));
			Prop.gameObject.SetActive(true);
			ShowPropControls();
			List<PropAccessory> accessories = new List<PropAccessory>(propToRetrieve.GetComponentsInChildren<PropAccessory>());
			float MAX_WAIT_TIME = 1f;
			float time = Time.time;
			while (accessories.Count > 0)
			{
				yield return null;
				if ((Time.time - time).CompareTo(MAX_WAIT_TIME) > 0)
				{
					break;
				}
				for (int num = accessories.Count - 1; num >= 0; num--)
				{
					if (accessories[num].IsLoadingComplete)
					{
						accessories.RemoveAt(num);
					}
				}
			}
			if (Prop.AnimReplacementController != null)
			{
				anim.runtimeAnimatorController = Prop.AnimReplacementController;
			}
			else if (Prop.AnimOverrideController != null)
			{
				anim.runtimeAnimatorController = Prop.AnimOverrideController;
			}
			else
			{
				anim.runtimeAnimatorController = defaultAnimatorController;
			}
			setInteractionButtonActive();
			if (this.EPropSpawned != null)
			{
				this.EPropSpawned(Prop);
			}
			if (Prop.UseOnceImmediately)
			{
				UsePropAtDestination(immediateUseDestination);
				if (Prop.StoreAfterUse)
				{
					StoreProp();
				}
			}
			StartUserPropMode(PropMode.Retrieve, true);
			Prop.SetAnimatorMode(PropMode.Retrieve, OnPropRetrieveCompleted);
			if (Prop.PropDef.PropType == PropDefinition.PropTypes.InteractiveObject && Prop.IsOwnerLocalPlayer)
			{
				Service.Get<ICPSwrveService>().Action("game.interactive_object", Prop.PropDef.name, "start");
			}
		}

		private void setInteractionButtonActive()
		{
			GameObjectReferenceData component;
			if (PlayerHandle.IsNull || Prop.PropDef.PropType == PropDefinition.PropTypes.PartyGame || !dataEntityCollection.TryGetComponent(PlayerHandle, out component))
			{
				return;
			}
			if (component == null)
			{
				Log.LogError(this, "THIS WOULD HAVE CRASHED! setInteractionButtonActive(): gameObjectRef in NULL!");
				return;
			}
			if (component.GameObject == null)
			{
				Log.LogError(this, "THIS WOULD HAVE CRASHED! setInteractionButtonActive(): gameObjectRef.GameObject in NULL!");
				return;
			}
			ParticipationController component2 = component.GameObject.GetComponent<ParticipationController>();
			if (component2 != null)
			{
				component2.PrepareParticipation(new ParticipationRequest(ParticipationRequest.Type.Prepare, Prop.gameObject, "PropUser"));
			}
		}

		public void OnPropRetrieveCompleted()
		{
			if (PlayerHandle.IsNull)
			{
				return;
			}
			isPropAnimCompleted = true;
			checkRetrieveCompleted();
			HeldObjectsData component = dataEntityCollection.GetComponent<HeldObjectsData>(PlayerHandle);
			if (component != null && component.HeldObject != null)
			{
				HeldObjectType objectType = component.HeldObject.ObjectType;
				if (objectType == HeldObjectType.DURABLE && Prop != null)
				{
					InteractiveZonePropEventHandler component2 = Prop.GetComponent<InteractiveZonePropEventHandler>();
					if (component2 != null)
					{
						UnityEngine.Object.Destroy(component2);
					}
				}
			}
			if (Prop != null && Prop.IsOwnerLocalPlayer)
			{
				ParticipationData component3 = dataEntityCollection.GetComponent<ParticipationData>(dataEntityCollection.LocalPlayerHandle);
				if (component3 != null)
				{
					component3.IsInteractButtonAvailable = true;
				}
			}
		}

		private void checkRetrieveCompleted()
		{
			if (Prop != null && isUserModeCompleted && isPropAnimCompleted)
			{
				if (this.EPropRetrieved != null)
				{
					this.EPropRetrieved(Prop);
				}
				if (Prop.UseOnceImmediately)
				{
					performNextInteractionRequest();
					return;
				}
				StartUserPropMode(PropMode.Hold, false);
				Prop.SetAnimatorMode(PropMode.Hold, OnPropHoldCompleted, !Prop.IgnoreHoldAnimationComplete);
			}
		}

		public void OnPropHoldCompleted()
		{
			isPropAnimCompleted = true;
			checkHoldCompleted();
		}

		private void checkHoldCompleted()
		{
			if (isUserModeCompleted && isPropAnimCompleted)
			{
				performNextInteractionRequest();
			}
		}

		private void performNextInteractionRequest()
		{
			if (propInteractionQueue.Count > 0)
			{
				currentRequest = propInteractionQueue[0];
				propInteractionQueue.RemoveAt(0);
				if (currentRequest.IsStore)
				{
					performStore();
				}
				else
				{
					performUse();
				}
			}
		}

		private bool queueInteractionRequest(PropInteractionRequest interationRequest)
		{
			if (Prop == null || queueHasStoreRequest())
			{
				return false;
			}
			propInteractionQueue.Add(interationRequest);
			checkHoldCompleted();
			return true;
		}

		public bool UsePropAtDestination(Vector3 onUseDestination)
		{
			if (Prop == null)
			{
				return false;
			}
			PropInteractionRequest propInteractionRequest = new PropInteractionRequest();
			propInteractionRequest.IsStore = false;
			propInteractionRequest.OnUseDestination = onUseDestination;
			return queueInteractionRequest(propInteractionRequest);
		}

		private void performUse()
		{
			startPropInteraction();
			StartUserPropMode(PropMode.Use, true);
			Prop.OnUseDestination = currentRequest.OnUseDestination;
			IsPropUseCompleted = false;
			Prop.UseStarted(OnPropUseCompleted);
			if (this.EPropUseStarted != null)
			{
				this.EPropUseStarted(Prop);
			}
			if (Prop.IsOwnerLocalPlayer)
			{
				Service.Get<ICPSwrveService>().Action("game.consumable", "deploy", Prop.PropDef.name, SceneManager.GetActiveScene().name);
				Service.Get<ICPSwrveService>().Timing(Mathf.RoundToInt(Time.timeSinceLevelLoad), "consumable_success", Prop.PropDef.name);
				if (Prop.PropDef.ServerAddedItem)
				{
					Service.Get<ICPSwrveService>().Action("game.consumable", "use_other", Prop.PropDef.name, SceneManager.GetActiveScene().name);
				}
			}
		}

		private void startPropInteraction()
		{
			GameObjectReferenceData component;
			if (PlayerHandle.IsNull || !dataEntityCollection.TryGetComponent(PlayerHandle, out component))
			{
				return;
			}
			Animator component2 = component.GameObject.GetComponent<Animator>();
			if (component2 != null)
			{
				AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(component2, 1);
				if (!LocomotionUtils.IsHolding(animatorStateInfo))
				{
					component2.Play(AnimationHashes.States.TorsoHold, AnimationHashes.Layers.Torso);
				}
			}
			ParticipationController component3 = component.GameObject.GetComponent<ParticipationController>();
			if (component3 != null)
			{
				component3.StartParticipation(new ParticipationRequest(ParticipationRequest.Type.Start, Prop.gameObject, "PropUser"));
			}
		}

		private void ShowPropControls()
		{
			if (Prop.IsOwnerLocalPlayer)
			{
				PropControlsOverride propControls = Prop.PropControls;
				GameObject gameObject = dataEntityCollection.GetComponent<GameObjectReferenceData>(dataEntityCollection.LocalPlayerHandle).GameObject;
				InputButtonGroupContentKey inputButtonGroupContentKey = LocomotionHelper.IsCurrentControllerOfType<SwimController>(gameObject) ? ((!(LocomotionHelper.GetCurrentController(gameObject) as SwimController).IsInShallowWater) ? getDivingControls(propControls) : propControls.SwimControls) : (LocomotionHelper.IsCurrentControllerOfType<SlideController>(gameObject) ? getTubingControls(propControls) : ((!LocomotionHelper.IsCurrentControllerOfType<SitController>(gameObject)) ? propControls.DefaultControls : ((!(LocomotionHelper.GetCurrentController(gameObject) as SitController).IsUnderwater) ? getSittingControls(propControls) : propControls.SitSwimControls)));
				if (inputButtonGroupContentKey != null && !string.IsNullOrEmpty(inputButtonGroupContentKey.Key))
				{
					Service.Get<EventDispatcher>().DispatchEvent(new ControlsScreenEvents.SetRightOption(inputButtonGroupContentKey));
				}
				else
				{
					Log.LogError(this, "Did not find a valid controls content key for this state");
				}
			}
		}

		private InputButtonGroupContentKey getDivingControls(PropControlsOverride propControlsOverride)
		{
			if (propControlsOverride.DivingControls != null && !string.IsNullOrEmpty(propControlsOverride.DivingControls.Key))
			{
				return propControlsOverride.DivingControls;
			}
			return propControlsOverride.SwimControls;
		}

		private InputButtonGroupContentKey getTubingControls(PropControlsOverride propControlsOverride)
		{
			if (propControlsOverride.TubeControls != null && !string.IsNullOrEmpty(propControlsOverride.TubeControls.Key))
			{
				return propControlsOverride.TubeControls;
			}
			return propControlsOverride.DefaultControls;
		}

		private InputButtonGroupContentKey getSittingControls(PropControlsOverride propControlsOverride)
		{
			if (propControlsOverride.SitControls != null && !string.IsNullOrEmpty(propControlsOverride.SitControls.Key))
			{
				return propControlsOverride.SitControls;
			}
			return propControlsOverride.DefaultControls;
		}

		public void ResetPropControls(bool isPropLocalPlayer)
		{
			if (isPropLocalPlayer && heldObjectsData.HeldObject == null && !dataEntityCollection.LocalPlayerHandle.IsNull && !SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.IsDestroyed())
			{
				LocomotionHelper.GetCurrentController(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject).LoadControlsLayout();
			}
		}

		public void OnPropUseCompleted()
		{
			isPropAnimCompleted = true;
			if (Prop != null && Prop.IsOwnerLocalPlayer && Prop.PropDef.PropType == PropDefinition.PropTypes.Consumable && Prop.PropDef.ExperienceType == PropDefinition.ConsumableExperience.World)
			{
				heldObjectsData.HeldObject = null;
				ResetPropControls(true);
			}
			checkInteractionRequestComplete();
		}

		public bool StoreProp()
		{
			if (Prop == null)
			{
				return false;
			}
			anim.SetTrigger(AnimationHashes.Params.ExitPropAction);
			PropInteractionRequest propInteractionRequest = new PropInteractionRequest();
			propInteractionRequest.IsStore = true;
			return queueInteractionRequest(propInteractionRequest);
		}

		private void performStore()
		{
			bool flag = !(Prop != null) || (!(Prop.PropId == "Dance") && !(Prop.PropId == "GearDance"));
			StartUserPropMode(PropMode.Store, flag);
			Prop.StoreStarted(OnPropStoreCompleted);
		}

		public void OnPropStoreCompleted()
		{
			isPropAnimCompleted = true;
			checkInteractionRequestComplete();
			if (this.EPropStored != null)
			{
				this.EPropStored(Prop);
			}
			IsPropUseCompleted = true;
			anim.ResetTrigger(AnimationHashes.Params.ExitPropAction);
		}

		private void checkInteractionRequestComplete()
		{
			if (!(Prop != null) || !isUserModeCompleted || !isPropAnimCompleted || currentRequest == null)
			{
				return;
			}
			if (!currentRequest.IsStore)
			{
				if (Prop.StoreAfterUse)
				{
					propInteractionQueue.Clear();
					StoreProp();
				}
				else
				{
					anim.runtimeAnimatorController = defaultAnimatorController;
				}
				if (this.EPropUseCompleted != null)
				{
					this.EPropUseCompleted(Prop);
				}
				StartUserPropMode(PropMode.Hold, false);
				Prop.SetAnimatorMode(PropMode.Hold, OnPropHoldCompleted, !Prop.IgnoreHoldAnimationComplete);
			}
			else
			{
				RemoveProp();
			}
		}

		private void Update()
		{
			if (isUserModeCompleted)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(1);
			int fullPathHash = currentAnimatorStateInfo.fullPathHash;
			if (stateTransitionComplete && waitForModeAnimCompletion)
			{
				if (currentAnimatorStateInfo.normalizedTime >= 1f)
				{
					waitForModeAnimCompletion = false;
					CompleteUserPropMode(fullPathHash);
				}
			}
			else if (fullPathHash != oldAnimState && !anim.IsInTransition(1))
			{
				stateTransitionComplete = true;
				oldAnimState = fullPathHash;
				if (!waitForModeAnimCompletion)
				{
					CompleteUserPropMode(fullPathHash);
				}
			}
		}

		private void CompleteUserPropMode(int currentStateHash)
		{
			isUserModeCompleted = true;
			if (currentStateHash == AnimationHashes.States.TorsoRetrieve)
			{
				checkRetrieveCompleted();
			}
			else if (currentStateHash == AnimationHashes.States.TorsoHold)
			{
				checkHoldCompleted();
			}
			else if (currentStateHash == AnimationHashes.States.TorsoUse || currentStateHash == AnimationHashes.States.TorsoStore)
			{
				checkInteractionRequestComplete();
			}
			else if (currentStateHash != AnimationHashes.States.TorsoIdle)
			{
			}
		}

		private bool queueHasStoreRequest()
		{
			for (int i = 0; i < propInteractionQueue.Count; i++)
			{
				if (propInteractionQueue[i].IsStore)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAllowedToCelebrate()
		{
			if (!IsPropUseCompleted)
			{
				return false;
			}
			if (GetComponentInChildren<FishingController>() != null)
			{
				return false;
			}
			return true;
		}
	}
}
