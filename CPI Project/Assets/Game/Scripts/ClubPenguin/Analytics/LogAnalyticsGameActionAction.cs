using ClubPenguin.Actions;
using Disney.MobileNetwork;

namespace ClubPenguin.Analytics
{
	public class LogAnalyticsGameActionAction : Action
	{
		public string Context = "";

		public string Action = "";

		public string Type = "";

		public string Message = "";

		public string Location = "";

		public bool IsSingular;

		public string SingularID = "";

		protected override void OnEnable()
		{
			if (Owner.CompareTag("Player"))
			{
				if (IsSingular)
				{
					Service.Get<ICPSwrveService>().ActionSingular(SingularID, "game." + Context, Action);
				}
				else
				{
					Service.Get<ICPSwrveService>().Action("game." + Context, Action, Message, Type);
				}
			}
		}

		protected override void CopyTo(Action _destWarper)
		{
			LogAnalyticsGameActionAction logAnalyticsGameActionAction = _destWarper as LogAnalyticsGameActionAction;
			logAnalyticsGameActionAction.Context = Context;
			logAnalyticsGameActionAction.Action = Action;
			logAnalyticsGameActionAction.Message = Message;
			logAnalyticsGameActionAction.Type = Type;
			logAnalyticsGameActionAction.Location = Location;
			logAnalyticsGameActionAction.IsSingular = IsSingular;
			logAnalyticsGameActionAction.SingularID = SingularID;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			Completed();
		}
	}
}
