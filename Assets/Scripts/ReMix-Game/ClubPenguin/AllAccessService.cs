using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class AllAccessService
	{
		private AllAccessEventDefinition currentlyActiveEvent;

		private bool isAllAccessCurrentlyActive;

		public bool IsAllAccessActive()
		{
			AllAccessEventDefinition currentEvent;
			return tryGetCurrentlyActiveEvent(out currentEvent);
		}

		public TimeSpan GetRemainingTime()
		{
			AllAccessEventDefinition currentEvent;
			if (tryGetCurrentlyActiveEvent(out currentEvent))
			{
				DateTime dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
				ScheduledEventDateDefinition scheduledEventDateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[currentEvent.DateDefinitionKey.Id];
				return (scheduledEventDateDefinition.Dates.EndDate.Date > dateTime) ? (scheduledEventDateDefinition.Dates.EndDate.Date - dateTime) : TimeSpan.Zero;
			}
			return TimeSpan.Zero;
		}

		public string GetAllAccessEventKey()
		{
			AllAccessEventDefinition currentEvent;
			if (tryGetCurrentlyActiveEvent(out currentEvent))
			{
				return currentEvent.Id;
			}
			return null;
		}

		private bool tryGetCurrentlyActiveEvent(out AllAccessEventDefinition currentEvent)
		{
			currentEvent = null;
			bool flag = false;
			if (currentlyActiveEvent == null)
			{
				Dictionary<string, AllAccessEventDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, AllAccessEventDefinition>>();
				foreach (KeyValuePair<string, AllAccessEventDefinition> item in dictionary)
				{
					if (doesEventMatchPlatform(item.Value))
					{
						ScheduledEventDateDefinition scheduledEventDatedDef = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[item.Value.DateDefinitionKey.Id];
						if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(scheduledEventDatedDef))
						{
							currentEvent = item.Value;
							currentlyActiveEvent = item.Value;
							flag = true;
						}
					}
				}
			}
			else
			{
				DateTime target = Service.Get<ContentSchedulerService>().ScheduledEventDate();
				ScheduledEventDateDefinition scheduledEventDatedDef = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[currentlyActiveEvent.DateDefinitionKey.Id];
				if (DateTimeUtils.IsBefore(target, scheduledEventDatedDef.Dates.EndDate))
				{
					currentEvent = currentlyActiveEvent;
					flag = true;
				}
				else
				{
					currentlyActiveEvent = null;
					flag = false;
				}
			}
			if (isAllAccessCurrentlyActive != flag)
			{
				updateMembershipDataValues(flag);
			}
			isAllAccessCurrentlyActive = flag;
			return flag;
		}

		private static bool doesEventMatchPlatform(AllAccessEventDefinition allAccessEventDefinition)
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				return true;
			case RuntimePlatform.IPhonePlayer:
				return allAccessEventDefinition.ApplyToAppleUsers;
			case RuntimePlatform.Android:
				return allAccessEventDefinition.ApplyToGoogleUsers;
			default:
				return false;
			}
		}

		private static void updateMembershipDataValues(bool isAllAccessActive)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle[] entitiesByType = cPDataEntityCollection.GetEntitiesByType<MembershipData>();
			for (int i = 0; i < entitiesByType.Length; i++)
			{
				MembershipData component = cPDataEntityCollection.GetComponent<MembershipData>(entitiesByType[i]);
				if (component.MembershipType != MembershipType.Member)
				{
					component.IsMember = isAllAccessActive;
					component.MembershipType = (isAllAccessActive ? MembershipType.AllAccessEventMember : MembershipType.None);
				}
			}
		}
	}
}
