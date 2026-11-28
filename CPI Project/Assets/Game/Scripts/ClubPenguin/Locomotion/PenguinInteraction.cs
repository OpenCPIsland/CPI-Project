using ClubPenguin.Actions;
using ClubPenguin.Compete;
using ClubPenguin.Core;
using ClubPenguin.Participation;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(ParticipationController))]
	public class PenguinInteraction : AbstractPenguinInteraction
	{
		public struct InteractChangeEvent
		{
			public readonly bool OnOff;

			public InteractChangeEvent(bool _onOff)
			{
				OnOff = _onOff;
			}
		}

		public struct InteractionStartedEvent
		{
			public readonly long InteractingPlayerId;

			public readonly GameObject ObjectInteractedWith;

			public InteractionStartedEvent(long interactingPlayerId, GameObject objectInteractedWith)
			{
				InteractingPlayerId = interactingPlayerId;
				ObjectInteractedWith = objectInteractedWith;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RequestInteractChangeEvent
		{
		}

		private ParticipationController participationController;

		private Competitor<GameObject> participantReference;

		private bool isReadyForParticipation;

		private ParticipationRequest pendingParticipationRequest;

		private const float TRIGGER_EXIT_GRACE_SEC = 0.2f;

		private readonly Dictionary<GameObject, float> outsideTriggerSince = new Dictionary<GameObject, float>();

		private readonly List<GameObject> competitorsOutsideTrigger = new List<GameObject>();

		private readonly List<Collider> triggerColliderBuffer = new List<Collider>();

		private Collider playerCollider;

		public bool InteractButtonState
		{
			get;
			private set;
		}

		private ParticipationData participationData
		{
			get
			{
				return participationController.GetParticipationData();
			}
		}

		public override void Awake()
		{
			base.Awake();
			participationController = GetComponent<ParticipationController>();
			ParticipationController obj = participationController;
			obj.InitializationCompleteAction = (System.Action)Delegate.Combine(obj.InitializationCompleteAction, new System.Action(onParticipationControllerInitializationComplete));
			participantReference = null;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			ParticipationController obj = participationController;
			obj.InitializationCompleteAction = (System.Action)Delegate.Remove(obj.InitializationCompleteAction, new System.Action(onParticipationControllerInitializationComplete));
		}

		private void onParticipationControllerInitializationComplete()
		{
			ParticipationController obj = participationController;
			obj.InitializationCompleteAction = (System.Action)Delegate.Remove(obj.InitializationCompleteAction, new System.Action(onParticipationControllerInitializationComplete));
			isReadyForParticipation = true;
			if (pendingParticipationRequest != null)
			{
				participationController.PrepareParticipation(pendingParticipationRequest);
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			GameObject gameObject = ActionSequencer.FindActionGraphObject(collider.gameObject);
			DataEntityHandle handle;
			if (!AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) || !(gameObject != null) || !canInteractWithActionGraph(gameObject))
			{
				return;
			}
			ForceInteractionAction component = gameObject.GetComponent<ForceInteractionAction>();
			if (component != null)
			{
				ParticipationRequest.Type type = ParticipationRequest.Type.ForcedInteraction;
				ParticipationRequest request = new ParticipationRequest(type, gameObject, "PenguinInteraction");
				participationController.PrepareParticipation(request);
				if (participationData != null && participationData.ParticipatingGO != null && participationData.ParticipatingGO.Value == gameObject)
				{
					participantReference = participationData.ParticipatingGO;
					currentActionGraphGameObject = gameObject;
					interactRequest.Set();
				}
			}
			else
			{
				ParticipationRequest.Type type = ParticipationRequest.Type.Prepare;
				ParticipationRequest request2 = new ParticipationRequest(type, gameObject, "PenguinInteraction");
				if (!isReadyForParticipation)
				{
					pendingParticipationRequest = request2;
				}
				else
				{
					participationController.PrepareParticipation(request2);
				}
			}
		}

		public bool RequestInteraction()
		{
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && participationData.CurrentParticipationState == ParticipationState.Pending)
			{
				ParticipationRequest.Type type = ParticipationRequest.Type.Start;
				if (participationController.StartParticipation(new ParticipationRequest(type, participationData.ParticipatingGO.Value, "PenguinInteraction")))
				{
					participantReference = participationData.ParticipatingGO;
					currentActionGraphGameObject = participationData.ParticipatingGO.Value;
					interactRequest.Set();
					return true;
				}
			}
			return false;
		}

		protected override void RetainParticipationWithActionGraphGO()
		{
			if (participantReference != null)
			{
				GameObject x = ActionSequencer.FindActionGraphObject(participantReference.Value);
				if (x != null && !participationController.LockParticipation(new ParticipationRequest(ParticipationRequest.Type.Lock, participantReference, "PenguinInteraction")))
				{
					OnActionSequencerSequenceCompleted(base.gameObject);
				}
			}
		}

		public override void OnActionSequencerSequenceCompleted(GameObject owner)
		{
			if (participationController != null && owner == base.gameObject)
			{
				if (!participationController.StopParticipation(new ParticipationRequest(ParticipationRequest.Type.Stop, participantReference, "PenguinInteraction")))
				{
					Debug.Log("Failed to stop participation");
				}
				participantReference = null;
				base.OnActionSequencerSequenceCompleted(owner);
			}
		}

		private void validatePendingTriggers()
		{
			ParticipationData data = participationData;
			if (data == null || data.CurrentParticipationState != ParticipationState.Pending || participationController.PriorityCompetition == null || participationController.PriorityCompetition.Competitors.Count == 0)
			{
				if (outsideTriggerSince.Count > 0)
				{
					outsideTriggerSince.Clear();
				}
				return;
			}
			if (playerCollider == null)
			{
				playerCollider = GetComponent<Collider>();
			}
			if (playerCollider == null || !playerCollider.enabled)
			{
				return;
			}
			Bounds playerBounds = playerCollider.bounds;
			Vector3 playerCenter = playerBounds.center;
			float reach = playerBounds.extents.magnitude;
			competitorsOutsideTrigger.Clear();
			List<Competitor<GameObject>> competitors = participationController.PriorityCompetition.Competitors;
			for (int i = 0; i < competitors.Count; i++)
			{
				GameObject competitorObject = competitors[i].Value;
				if (competitorObject.IsDestroyed())
				{
					continue;
				}
				bool hasTrigger;
				if (isInsideTriggers(competitorObject, playerCenter, reach, out hasTrigger) || !hasTrigger)
				{
					outsideTriggerSince.Remove(competitorObject);
					continue;
				}
				float since;
				if (!outsideTriggerSince.TryGetValue(competitorObject, out since))
				{
					outsideTriggerSince[competitorObject] = Time.time;
				}
				else if (Time.time - since >= TRIGGER_EXIT_GRACE_SEC)
				{
					competitorsOutsideTrigger.Add(competitorObject);
				}
			}
			for (int i = 0; i < competitorsOutsideTrigger.Count; i++)
			{
				outsideTriggerSince.Remove(competitorsOutsideTrigger[i]);
				participationController.RemoveCompetitorFromZoneExit(competitorsOutsideTrigger[i]);
			}
		}

		private bool isInsideTriggers(GameObject actionGraphObject, Vector3 point, float reach, out bool hasTrigger)
		{
			hasTrigger = false;
			if (actionGraphObject.transform.IsChildOf(base.transform))
			{
				return true;
			}
			actionGraphObject.GetComponentsInChildren(false, triggerColliderBuffer);
			bool inside = false;
			for (int i = 0; i < triggerColliderBuffer.Count; i++)
			{
				Collider triggerCollider = triggerColliderBuffer[i];
				if (!triggerCollider.enabled || !triggerCollider.isTrigger)
				{
					continue;
				}
				MeshCollider meshCollider = triggerCollider as MeshCollider;
				if ((meshCollider != null && !meshCollider.convex) || ActionSequencer.FindActionGraphObject(triggerCollider.gameObject) != actionGraphObject)
				{
					continue;
				}
				hasTrigger = true;
				if ((triggerCollider.ClosestPoint(point) - point).sqrMagnitude <= reach * reach)
				{
					inside = true;
					break;
				}
			}
			triggerColliderBuffer.Clear();
			return inside;
		}

		private void Update()
		{
			validatePendingTriggers();
			if (participationData == null || participationData.ParticipatingGO == null)
			{
				return;
			}
			GameObject gameObject = ActionSequencer.FindActionGraphObject(participationData.ParticipatingGO.Value);
			if (gameObject != null)
			{
				if (participationData.CurrentParticipationState == ParticipationState.Participating)
				{
					if (interactRequest.Active && gameObject.activeInHierarchy)
					{
						Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PenguinInteraction), gameObject, delegate
						{
							CoroutineRunner.Start(preStartInteraction(participantReference.Value), this, "preStartInteraction");
						});
						interactRequest.Reset();
					}
				}
				else if (participationData.CurrentParticipationState == ParticipationState.Pending && !canInteractWithActionGraph(gameObject))
				{
					participationController.ClearPriorityCompetition(participantReference);
				}
			}
			interactRequest.Update();
		}
	}
}
