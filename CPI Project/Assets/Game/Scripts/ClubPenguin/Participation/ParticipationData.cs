using ClubPenguin.Compete;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Participation
{
	[Serializable]
	public class ParticipationData : BaseData, IEditableParticipationData
	{
		[SerializeField]
		private GameObject participatingObject;

		private Competitor<GameObject> participant;

		private bool isInteractButtonAvailable = true;

		private ParticipationState currentParticipationState;

		public bool IsInteractButtonAvailable
		{
			get
			{
				return isInteractButtonAvailable;
			}
			set
			{
				isInteractButtonAvailable = value;
			}
		}

		public Competitor<GameObject> ParticipatingGO
		{
			get
			{
				return participant;
			}
			set
			{
				participant = value;
				if (value != null)
				{
					participatingObject = value.theObject;
				}
				else
				{
					participatingObject = null;
				}
				dispatchCurrentState();
			}
		}

		public ParticipationState CurrentParticipationState
		{
			get
			{
				return currentParticipationState;
			}
			set
			{
				currentParticipationState = value;
				dispatchCurrentState();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ParticipationDataMonoBehaviour);
			}
		}

		public bool isTopPriorityAndState(ParticipationState state, GameObject go)
		{
			return participatingObject == go && currentParticipationState == state;
		}

		public bool isTopPriorityAndState(ParticipationState state, Competitor<GameObject> go)
		{
			return ParticipatingGO == go && currentParticipationState == state;
		}

		private void dispatchCurrentState()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ParticipationDataEvent.StateChanged(currentParticipationState, participatingObject));
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
