using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class LoginZoneStateHandler : AbstractAccountStateHandler
	{
		public string ContinueEvent;

		private ContentSchedulerService contentSchedulerService;

		private GameStateController gameStateController;

		public new void Start()
		{
			base.Start();
			contentSchedulerService = Service.Get<ContentSchedulerService>();
			gameStateController = Service.Get<GameStateController>();
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				if (Service.Get<GameStateController>().IsFTUEComplete)
				{
					CoroutineRunner.Start(Service.Get<GameData>().LoadDataForDefinitions(InitGameDataAction.DefinitionTypesToLoadAfterBoot, parseLoginZoneDefinitions), this, "LoginZoneStateHandler.OnStageChanged");
				}
				else
				{
					gameStateController.LoginZone = "";
				}
				rootStateMachine.SendEvent(ContinueEvent);
			}
		}

		private void parseLoginZoneDefinitions()
		{
			LoginZoneDefinition[] array = Service.Get<IGameData>().Get<LoginZoneDefinition[]>();
			for (int i = 0; i < array.Length; i++)
			{
				ScheduledEventDateDefinition scheduledEventDateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[array[i].ScheduledEventDateKey.Id];
				if (contentSchedulerService.IsDuringScheduleEventDates(scheduledEventDateDefinition) && (!hasConditions(array[i]) || anyConditionsMet(array[i].ANYConditions)))
				{
					if (array[i].Zone != null)
					{
						gameStateController.LoginZone = array[i].Zone.Id;
						return;
					}
					Log.LogErrorFormatted(this, "The Zone for the event {0} is missing", scheduledEventDateDefinition.name);
				}
			}
			gameStateController.LoginZone = "";
		}

		private bool hasConditions(LoginZoneDefinition definition)
		{
			if (definition.ANYConditions != null && definition.ANYConditions.Length > 0)
			{
				return true;
			}
			return false;
		}

		private bool anyConditionsMet(LoginZoneDefinition.CompositeLoginZoneCondition[] ANYConditions)
		{
			if (ANYConditions == null || ANYConditions.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < ANYConditions.Length; i++)
			{
				if (ANYConditions[i].ANDConditions == null || ANYConditions[i].ANDConditions.Length <= 0)
				{
					continue;
				}
				bool flag = true;
				for (int j = 0; j < ANYConditions[i].ANDConditions.Length; j++)
				{
					if (!baseConditionMet(ANYConditions[i].ANDConditions[j]))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private bool baseConditionMet(LoginZoneDefinition.LoginZoneCondition condition)
		{
			ScheduledEventDateDefinition scheduledEventDateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[condition.DateDefinitionKey.Id];
			if (!DateTimeUtils.DoesDateFallBetween(contentSchedulerService.ScheduledEventDate(), scheduledEventDateDefinition.Dates.StartDate, scheduledEventDateDefinition.Dates.EndDate))
			{
				return false;
			}
			bool result = false;
			string key = condition.PlayerPrefsKey;
			if (condition.AddPlayerNameToKey)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				string displayName = cPDataEntityCollection.GetComponent<DisplayNameData>(cPDataEntityCollection.LocalPlayerHandle).DisplayName;
				key = displayName + condition.PlayerPrefsKey;
			}
			switch (condition.TypeOfCondition)
			{
			case LoginZoneDefinition.ConditionType.PlayerPrefExist:
				result = PlayerPrefs.HasKey(key);
				break;
			case LoginZoneDefinition.ConditionType.PlayerPrefDoesNotExist:
				result = !PlayerPrefs.HasKey(key);
				break;
			}
			return result;
		}
	}
}
