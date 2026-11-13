using ClubPenguin.Actions;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public abstract class AbstractPenguinInteraction : MonoBehaviour
	{
		protected EventDispatcher dispatcher;

		protected LocomotionEventBroadcaster locoEventBroadcaster;

		protected ActionRequest interactRequest;

		protected CPDataEntityCollection dataEntityCollection;

		protected LocomotionTracker locomotionTracker;

		protected GameObject currentActionGraphGameObject;

		public virtual void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			locoEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
			dispatcher = Service.Get<EventDispatcher>();
			interactRequest = new ActionRequest(PenguinUserControl.DefaultActionRequestBufferTime);
			locomotionTracker = GetComponent<LocomotionTracker>();
			currentActionGraphGameObject = null;
		}

		public virtual void Start()
		{
		}

		public virtual void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (SceneRefs.ActionSequencer != null)
			{
				SceneRefs.ActionSequencer.SequenceCompleted -= OnActionSequencerSequenceCompleted;
			}
		}

		public virtual void OnActionSequencerSequenceCompleted(GameObject owner)
		{
			SceneRefs.ActionSequencer.SequenceCompleted -= OnActionSequencerSequenceCompleted;
		}

		protected bool canInteractWithActionGraph(GameObject actionGraphGO)
		{
			bool result = false;
			FilterTriggerAction component = actionGraphGO.GetComponent<FilterTriggerAction>();
			if (component == null)
			{
				result = true;
			}
			else if (actionGraphGO.activeInHierarchy)
			{
				result = checkCanInteractWithFilter(component);
			}
			return result;
		}

		private bool checkCanInteractWithFilter(FilterTriggerAction filter)
		{
			DataEntityHandle handle;
			SessionIdData component;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && dataEntityCollection.TryGetComponent(handle, out component) && filter.CanInteract(component.SessionId, base.gameObject) && (string.IsNullOrEmpty(filter.OwnerTag) || CompareTag(filter.OwnerTag)))
			{
				return true;
			}
			return false;
		}

		protected IEnumerator preStartInteraction(GameObject go)
		{
			DataEntityHandle handle;
			if (!base.gameObject.IsDestroyed() && AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && locoEventBroadcaster != null && !go.IsDestroyed())
			{
				GameObject actionGraphObject = ActionSequencer.FindActionGraphObject(go);
				if (!actionGraphObject.IsDestroyed())
				{
					locoEventBroadcaster.BroadcastOnInteractionPreStarted(actionGraphObject);
					yield return CoroutineRunner.Start(waitForValidLocomotionModeToInteract(), this, "waitForValidLocomotionModeToInteract");
					startInteraction();
				}
			}
		}

		protected void startInteraction()
		{
			GameObject gameObject = ActionSequencer.FindActionGraphObject(currentActionGraphGameObject);
			if (gameObject != null && SceneRefs.ActionSequencer.StartSequence(base.gameObject, gameObject))
			{
				if (locoEventBroadcaster != null)
				{
					locoEventBroadcaster.BroadcastOnInteractionStarted(gameObject);
				}
				DataEntityHandle handle;
				SessionIdData component;
				if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && dataEntityCollection.TryGetComponent(handle, out component))
				{
					dispatcher.DispatchEvent(new PenguinInteraction.InteractionStartedEvent(component.SessionId, gameObject));
				}
				LocomotionController currentController = locomotionTracker.GetCurrentController();
				if (currentController != null && (gameObject.GetComponent<ToggleCoreGameplayAction>() != null || gameObject.GetComponent<WarpTunnelAction>() != null))
				{
					currentController.OnBlockingInteractionStarted();
				}
				interactRequest.Reset();
				RetainParticipationWithActionGraphGO();
				SceneRefs.ActionSequencer.SequenceCompleted += OnActionSequencerSequenceCompleted;
			}
		}

		protected virtual void RetainParticipationWithActionGraphGO()
		{
		}

		protected IEnumerator waitForValidLocomotionModeToInteract()
		{
			if (locomotionTracker != null && locomotionTracker.GetCurrentController() is RunController)
			{
				Animator anim = GetComponent<Animator>();
				RunController runController = (RunController)locomotionTracker.GetCurrentController();
				RunController.ControllerBehaviour oldBehaviour = runController.Behaviour;
				runController.Behaviour = new RunController.ControllerBehaviour
				{
					IgnoreCollisions = false,
					IgnoreGravity = false,
					IgnoreRotation = false,
					IgnoreTranslation = false,
					IgnoreJumpRequests = true,
					IgnoreStickInput = true,
					LastModifier = this
				};
				AnimatorStateInfo animStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
				while (!LocomotionUtils.IsLocomoting(animStateInfo) && !LocomotionUtils.IsLanding(animStateInfo) && !LocomotionUtils.IsIdling(animStateInfo))
				{
					yield return null;
					animStateInfo = LocomotionUtils.GetAnimatorStateInfo(anim);
				}
				runController.ResetMomentum();
				if (runController.Behaviour.LastModifier == this)
				{
					runController.Behaviour = oldBehaviour;
				}
			}
		}
	}
}
