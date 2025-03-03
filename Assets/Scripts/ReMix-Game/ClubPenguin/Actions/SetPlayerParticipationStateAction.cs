using ClubPenguin.Compete;
using ClubPenguin.Participation;
using ClubPenguin.Props;
using System;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class SetPlayerParticipationStateAction : Action
	{
		public string State = ParticipationState.Ready.ToString();

		protected override void CopyTo(Action _destWarper)
		{
			SetPlayerParticipationStateAction setPlayerParticipationStateAction = _destWarper as SetPlayerParticipationStateAction;
			setPlayerParticipationStateAction.State = State;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			PropUser component = GetComponent<PropUser>();
			ParticipationController component2 = GetComponent<ParticipationController>();
			ParticipationState participationState = (ParticipationState)Enum.Parse(typeof(ParticipationState), State);
			if (component != null && component2 != null)
			{
				Competitor<GameObject> currentInteractingParticipantReference = component2.GetCurrentInteractingParticipantReference();
				if (component2.IsInteractingWithGameObject(base.gameObject) && participationState == ParticipationState.Pending)
				{
					component2.StopParticipation(new ParticipationRequest(ParticipationRequest.Type.Stop, currentInteractingParticipantReference, "SetPlayerParticipationStateAction"));
				}
				else if (participationState == ParticipationState.Participating)
				{
					component2.StartParticipation(new ParticipationRequest(ParticipationRequest.Type.Start, base.gameObject, "SetPlayerParticipationStateAction"));
				}
			}
			Completed();
		}
	}
}
