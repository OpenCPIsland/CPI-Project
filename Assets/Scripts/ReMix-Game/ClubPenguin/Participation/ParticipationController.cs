#define UNITY_ASSERTIONS
using ClubPenguin.Compete;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Participation
{
	public class ParticipationController : MonoBehaviour
	{
		[SerializeField]
		private ParticipationData participationData;

		private CPDataEntityCollection dataEntityCollection;

		private Dictionary<string, AbstractParticipationFilter> participationFilters;

		internal Competition<GameObject> PriorityCompetition;

		public bool IsBeingDestroyed = false;

		public System.Action InitializationCompleteAction;

		private int retainedTicketNumber = 0;

		private void Awake()
		{
			participationFilters = new Dictionary<string, AbstractParticipationFilter>();
		}

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
			{
				ParticipationData component;
				if (dataEntityCollection.TryGetComponent(handle, out component))
				{
					participationData = component;
					ResetParticipation();
					initializePriorityCompetition();
					if (InitializationCompleteAction != null)
					{
						InitializationCompleteAction();
					}
				}
				else
				{
					dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ParticipationData>>(onParticipationDataReady);
				}
			}
			else
			{
				Log.LogError(this, "Unable to find handle");
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			InitializationCompleteAction = null;
			IsBeingDestroyed = true;
			if (dataEntityCollection != null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ParticipationData>>(onParticipationDataReady);
			}
			StopObservingWinner();
		}

		private void StopObservingWinner()
		{
			if (PriorityCompetition != null && PriorityCompetition.LatestResults != null && PriorityCompetition.LatestResults.Winner != null)
			{
				StopObserving(PriorityCompetition.LatestResults.Winner);
			}
		}

		private IEnumerator WatchDogCoroutineForRetained(int ticket)
		{
			yield return new WaitForSeconds(1f);
			if (participationData.CurrentParticipationState == ParticipationState.Retained && ticket == retainedTicketNumber)
			{
				StopObservingWinner();
				ResetParticipation();
				base.gameObject.SetActive(false);
				yield return new WaitForFrame(1);
				base.gameObject.SetActive(true);
			}
		}

		private bool onParticipationDataReady(DataEntityEvents.ComponentAddedEvent<ParticipationData> evt)
		{
			ResetParticipation();
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && evt.Handle == handle)
			{
				participationData = evt.Component;
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ParticipationData>>(onParticipationDataReady);
				initializePriorityCompetition();
			}
			return false;
		}

		private void initializePriorityCompetition()
		{
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
			{
				PriorityCompetition = new Competition<GameObject>(new ParticipationCompetitionRules(handle, dataEntityCollection, participationData));
			}
		}

		public ParticipationData GetParticipationData()
		{
			return participationData;
		}

		public bool IsInteracting()
		{
			bool result = false;
			if (participationData != null)
			{
				result = (participationData.CurrentParticipationState == ParticipationState.Participating || participationData.CurrentParticipationState == ParticipationState.Retained);
			}
			return result;
		}

		public bool IsInteractingWithGameObject(GameObject go)
		{
			if (participationData != null && participationData.ParticipatingGO != null)
			{
				return participationData.ParticipatingGO.Value == go;
			}
			return false;
		}

		public Competitor<GameObject> GetCurrentInteractingParticipantReference()
		{
			return participationData.ParticipatingGO;
		}

		public void AddParticipationFilter(AbstractParticipationFilter filter)
		{
			if (!participationFilters.ContainsKey(filter.getId()))
			{
				participationFilters.Add(filter.getId(), filter);
			}
		}

		public void RemoveParticipationFilter(string filderId)
		{
			if (participationFilters.ContainsKey(filderId))
			{
				participationFilters.Remove(filderId);
			}
		}

		public bool PrepareParticipation(ParticipationRequest request)
		{
			bool result = true;
			if ((request.RequestType == ParticipationRequest.Type.Prepare || request.RequestType == ParticipationRequest.Type.ForcedInteraction) && doesRequestPassFilters(request))
			{
				EnterRequestIntoParticipationCompetition(request);
			}
			return result;
		}

		public bool StartParticipation(ParticipationRequest request)
		{
			bool result = false;
			if ((request.RequestType == ParticipationRequest.Type.Stop || !participationData.isTopPriorityAndState(ParticipationState.Retained, request.Requestor)) && request.RequestType == ParticipationRequest.Type.Start)
			{
				result = MoveRequestToParticipate(request);
			}
			return result;
		}

		public bool LockParticipation(ParticipationRequest request)
		{
			if (participationData.isTopPriorityAndState(ParticipationState.Participating, request.CompetitorReference))
			{
				participationData.CurrentParticipationState = ParticipationState.Retained;
				return true;
			}
			return false;
		}

		public void ForceStopParticipation(ParticipationRequest request)
		{
			Internal_UpdateParticipationWhenSomethingGotRemoved(request.CompetitorReference, "Controller_ForceStopRequest");
		}

		public bool StopParticipation(ParticipationRequest request)
		{
			return StopCurrentParticipation(request);
		}

		public CompetitionResults<GameObject> GetLatestResults()
		{
			return PriorityCompetition.LatestResults;
		}

		private bool doesRequestPassFilters(ParticipationRequest request)
		{
			foreach (AbstractParticipationFilter value in participationFilters.Values)
			{
				if (!value.doesRequestPassFilter(request))
				{
					return false;
				}
			}
			return true;
		}

		private Competitor<GameObject> EnterRequestIntoParticipationCompetition(ParticipationRequest request)
		{
			Competitor<GameObject> competitor = null;
			if (PriorityCompetition != null)
			{
				competitor = PriorityCompetition.EnterCompetitor(request.Requestor);
				AddObserverAndListenersToParticipationRequestor(competitor);
				UpdateParticipationWhenRequestReceived(request);
			}
			return competitor;
		}

		private void UpdateParticipationWhenRequestReceived(ParticipationRequest request)
		{
			if (participationData.CurrentParticipationState == ParticipationState.Participating || participationData.CurrentParticipationState == ParticipationState.Retained)
			{
				return;
			}
			PriorityCompetition.Decide();
			if (PriorityCompetition.LatestResults != null && PriorityCompetition.LatestResults.Winner != null)
			{
				participationData.ParticipatingGO = PriorityCompetition.LatestResults.Winner;
				if (request.RequestType == ParticipationRequest.Type.Prepare)
				{
					participationData.CurrentParticipationState = ParticipationState.Pending;
				}
				else if (request.RequestType == ParticipationRequest.Type.ForcedInteraction)
				{
					participationData.CurrentParticipationState = ParticipationState.Participating;
				}
			}
			else
			{
				ResetParticipation();
			}
		}

		private void ResetParticipation()
		{
			participationData.ParticipatingGO = null;
			participationData.CurrentParticipationState = ParticipationState.Ready;
		}

		private void AddObserverAndListenersToParticipationRequestor(Competitor<GameObject> participant)
		{
			ParticipationObserver participationObserver = participant.Value.GetComponent<ParticipationObserver>();
			if (participationObserver == null)
			{
				participationObserver = participant.Value.AddComponent<ParticipationObserver>();
			}
			if (participationObserver != null && !participationObserver.IsObserving(base.gameObject))
			{
				participationObserver.StartObserving(base.gameObject, this, participant);
				participationObserver.ObserverExit += OnObserverExit;
				participationObserver.ObserverDisable += OnObserverDisable;
			}
		}

		private bool MoveRequestToParticipate(ParticipationRequest request)
		{
			if (participationData.isTopPriorityAndState(ParticipationState.Pending, request.Requestor))
			{
				participationData.CurrentParticipationState = ParticipationState.Participating;
			}
			return participationData.isTopPriorityAndState(ParticipationState.Participating, request.Requestor);
		}

		private bool StopCurrentParticipation(ParticipationRequest request)
		{
			bool flag = participationData.isTopPriorityAndState(ParticipationState.Participating, request.Requestor) || participationData.isTopPriorityAndState(ParticipationState.Retained, request.Requestor);
			if (flag)
			{
				Internal_UpdateParticipationWhenSomethingGotRemoved(request.CompetitorReference, "Controller_stopRequest");
				Competitor<GameObject> competitor = PriorityCompetition.GetCompetitor(request.Requestor);
				if (competitor == null)
				{
					StopObserving(request.CompetitorReference);
				}
			}
			return flag;
		}

		private void Internal_UpdateParticipationWhenSomethingGotRemoved(Competitor<GameObject> gameObjectRemoved, string source)
		{
			PriorityCompetition.Decide();
			if (PriorityCompetition.LatestResults != null && PriorityCompetition.LatestResults.Winner != null)
			{
				participationData.ParticipatingGO = PriorityCompetition.LatestResults.Winner;
				participationData.CurrentParticipationState = ParticipationState.Pending;
			}
			else
			{
				ResetParticipation();
			}
		}

		public void StopObserving(Competitor<GameObject> competitor)
		{
			if (competitor != null && !competitor.Value.IsDestroyed())
			{
				ParticipationObserver component = competitor.Value.GetComponent<ParticipationObserver>();
				if (component != null && !component.gameObject.IsDestroyed() && !base.gameObject.IsDestroyed())
				{
					component.StopObserving(base.gameObject);
					component.ObserverDisable -= OnObserverDisable;
					component.ObserverExit -= OnObserverExit;
				}
			}
		}

		public void ClearPriorityCompetition(Competitor<GameObject> go)
		{
			try
			{
				foreach (Competitor<GameObject> competitor in PriorityCompetition.Competitors)
				{
					if (!competitor.theObject.IsDestroyed())
					{
						StopObserving(competitor);
					}
				}
			}
			finally
			{
				PriorityCompetition.RemoveAllCompetitorsAndResults();
				ResetParticipation();
			}
		}

		private void OnObserverDisable(GameObject go, ParticipationObserver observer)
		{
			Competitor<GameObject> competitor = PriorityCompetition.GetCompetitor(go);
			if (competitor != null)
			{
				bool flag = false;
				try
				{
					PriorityCompetition.RemoveCompetitor(go);
					if (participationData.CurrentParticipationState != ParticipationState.Retained)
					{
						Internal_UpdateParticipationWhenSomethingGotRemoved(competitor, "Controller_triggerExit");
						StopObserving(competitor);
						flag = true;
					}
				}
				catch
				{
					ClearPriorityCompetition(competitor);
				}
			}
		}

		private void OnObserverExit(Competitor<GameObject> competitor, GameObject playerGameObject, ParticipationObserver observer)
		{
			Assert.IsTrue(playerGameObject == base.gameObject, "Not my player!");
			if (playerGameObject == base.gameObject && PriorityCompetition.GetCompetitor(competitor.theObject) != null)
			{
				bool flag = false;
				try
				{
					PriorityCompetition.RemoveCompetitor(competitor);
					if (participationData.CurrentParticipationState != ParticipationState.Retained)
					{
						Internal_UpdateParticipationWhenSomethingGotRemoved(competitor, "triggerExit");
						StopObserving(competitor);
						flag = true;
					}
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
					ClearPriorityCompetition(competitor);
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void LogRequest(ParticipationRequest request, bool accepted)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public void LogRequest(ParticipationRequest.Type requestType, Competitor<GameObject> requestor, string description, bool accepted)
		{
		}

		[Conditional("UNITY_EDITOR")]
		public void LogRequest(ParticipationRequest.Type requestType, GameObject requestor, string description, bool accepted)
		{
		}
	}
}
