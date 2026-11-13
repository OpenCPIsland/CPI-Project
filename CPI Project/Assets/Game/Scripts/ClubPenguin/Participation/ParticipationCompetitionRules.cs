using ClubPenguin.Actions;
using ClubPenguin.Adventure;
using ClubPenguin.Compete;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Props;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Participation
{
	public class ParticipationCompetitionRules : CompetitionRules<GameObject>
	{
		public enum Ranking
		{
			A = 100,
			B = 95,
			C = 94,
			D = 80,
			E = 70,
			F = 60,
			Z = 0,
			Invalid = -1
		}

		protected DataEntityHandle PlayerHandle;

		private CPDataEntityCollection DataEntityCollection;

		private ParticipationData participation;

		public ParticipationCompetitionRules(DataEntityHandle playerHandle, CPDataEntityCollection dataEntityCollection, ParticipationData data)
		{
			PlayerHandle = playerHandle;
			DataEntityCollection = dataEntityCollection;
			participation = data;
		}

		public override void AssignPointsToCompetitors(List<Competitor<GameObject>> competitors)
		{
			for (int i = 0; i < competitors.Count; i++)
			{
				Competitor<GameObject> competitor = competitors[i];
				if (competitor.Value.IsDestroyed())
				{
					competitor.Points = 0;
					continue;
				}
				FilterTriggerAction component = competitor.Value.GetComponent<FilterTriggerAction>();
				if (component != null && !checkCanInteractWithFilter(competitor.Value, component))
				{
					competitor.Points = 0;
					continue;
				}
				ForceInteractionAction component2 = competitor.Value.GetComponent<ForceInteractionAction>();
				if (component2 != null)
				{
					competitor.Points = 100;
					continue;
				}
				InteractionIdentifier component3 = competitor.Value.GetComponent<InteractionIdentifier>();
				if (component3 != null)
				{
					competitor.Points = getScoreFromInteractionIdentifier(component3);
					continue;
				}
				GameObject gameObject = null;
				GameObjectReferenceData component4;
				if (DataEntityCollection.TryGetComponent(PlayerHandle, out component4))
				{
					gameObject = component4.GameObject;
				}
				bool flag = participation.ParticipatingGO == competitor && participation.CurrentParticipationState == ParticipationState.Participating;
				bool flag2 = competitor.Value.GetComponent<PropExperience>() != null && gameObject != null && !gameObject.IsDestroyed() && competitor.Value.transform.root == gameObject.transform;
				if (flag || flag2)
				{
					competitor.Points = 95;
					continue;
				}
				bool flag3 = competitor.Value.GetComponent<Prop>() != null;
				bool flag4 = competitor.Value.GetComponent<SendQuestEvent>() != null;
				bool flag5 = competitor.Value.GetComponent<Action>() != null;
				bool flag6 = competitor.Value.GetComponent<PropExperience>() != null;
				if (flag3)
				{
					competitor.Points = 94;
				}
				if (flag4)
				{
					competitor.Points = 80;
				}
				if (flag5)
				{
					competitor.Points = 70;
				}
				if (flag6)
				{
					competitor.Points = 60;
				}
			}
		}

		private int getScoreFromInteractionIdentifier(InteractionIdentifier interaction)
		{
			if (Service.Get<ActionButtonRequestRuleService>().ContainsRule(interaction.ID))
			{
				ActionButtonRequestRuleDefinition rule = Service.Get<ActionButtonRequestRuleService>().GetRule(interaction.ID);
				return 100 - rule.Priority;
			}
			return -1;
		}

		private bool checkCanInteractWithFilter(GameObject actionGraphGO, FilterTriggerAction filter)
		{
			SessionIdData component;
			if (DataEntityCollection.TryGetComponent(PlayerHandle, out component))
			{
				return filter.CanInteract(component.SessionId, actionGraphGO);
			}
			return false;
		}
	}
}
