using ClubPenguin.Adventure;
using ClubPenguin.Tutorial;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class AdventureReminderTutorialStateHandler : AbstractAccountStateHandler
	{
		public string ContinueEvent;

		private MascotService mascotService;

		public new void Start()
		{
			base.Start();
			mascotService = Service.Get<MascotService>();
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				if (Service.Get<GameStateController>().IsFTUEComplete)
				{
					setNumPlayingDaysWithAvailableAdventure();
				}
				rootStateMachine.SendEvent(ContinueEvent);
			}
		}

		private void setNumPlayingDaysWithAvailableAdventure()
		{
			foreach (Mascot mascot in mascotService.Mascots)
			{
				if (mascot.IsQuestGiver)
				{
					AdventureReminderTutorial.SetNumPlayingDaysWithAvailableAdventure(mascot.Name);
				}
			}
		}
	}
}
