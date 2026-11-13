using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Analytics
{
	[ActionCategory("Analytics")]
	public class AnalyticsGameActionAction : FsmStateAction
	{
		[Tooltip("One of 'FTUE' or 'quest' for quest logging - otherwise as stated in BI Docs")]
		public string Context;

		[ActionSection("Quest Specific Logging")]
		[Tooltip("should be name of mascot")]
		public string QuestId;

		[Tooltip("01|01 first part is the season, next part is the episode")]
		public string SeasonAndEpisode;

		[Tooltip("Human readable objective name the sub section of the quest that the user is on")]
		public string ObjectiveName;

		[Tooltip("numbering within quest 01 means it's the 1st objective in the quest")]
		public string ObjectiveNumber;

		[Tooltip("Human readable step name (RH-dialog-Ahoy, RH-popup-lighthouse-out)")]
		[ActionSection("All Funnels (including Quests) Logging")]
		public string StepName;

		[Tooltip("numbering within objective 01 means it's the 1st task in this quest objective bucket")]
		public string StepNumber;

		[ActionSection("Other BI Logging")]
		public string Action;

		public string Type;

		public string Location;

		public string Level;

		public string Message;

		private string logType;

		public override void OnEnter()
		{
			if ((Context.ToLower() == "quest" || Context.ToLower() == "quest_fail") && !string.IsNullOrEmpty(QuestId))
			{
				string type = Context + "." + QuestId + "." + SeasonAndEpisode;
				string step_number = StepNumber;
				string message = "";
				if (StepNumber.Contains("."))
				{
					char[] separator = new char[1]
					{
						'.'
					};
					string[] array = StepNumber.Split(separator);
					step_number = array[0];
					message = array[1];
				}
				Service.Get<ICPSwrveService>().QuestFunnel(type, ObjectiveNumber, ObjectiveName, step_number, StepName, message, true);
			}
			else if (Context == "FTUE")
			{
				string step_number = StepNumber;
				string message = "";
				if (StepNumber.Contains("."))
				{
					char[] separator = new char[1]
					{
						'.'
					};
					string[] array = StepNumber.Split(separator);
					step_number = array[0];
					message = array[1];
				}
				Service.Get<ICPSwrveService>().Funnel(Context, step_number, Action, message, true);
			}
			else if (!string.IsNullOrEmpty(StepNumber))
			{
				string step_number = StepNumber;
				logType = Type;
				logType += Message;
				Service.Get<ICPSwrveService>().Funnel(Context, step_number, StepName, logType, true);
			}
			else
			{
				Service.Get<ICPSwrveService>().Action("game." + Context, Action, Type, Location, Message, null, Level);
			}
			Finish();
		}
	}
}
