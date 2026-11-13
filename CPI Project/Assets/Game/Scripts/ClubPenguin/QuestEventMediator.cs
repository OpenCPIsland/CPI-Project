using ClubPenguin.Adventure;
using ClubPenguin.Task;
using Disney.LaunchPadFramework;

namespace ClubPenguin
{
	public class QuestEventMediator
	{
		private QuestService questService;

		public QuestEventMediator(EventDispatcher eventDispatcher, QuestService questService)
		{
			this.questService = questService;
			eventDispatcher.AddListener<TaskEvents.TaskCompleted>(onTaskComplete);
		}

		private bool onTaskComplete(TaskEvents.TaskCompleted evt)
		{
			questService.SendEvent(evt.Task.Id + "Complete");
			return false;
		}
	}
}
