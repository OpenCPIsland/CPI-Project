using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using ClubPenguin.Task;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DailyChallengeTaskCompletionItem : MonoBehaviour
	{
		private enum TaskState
		{
			inProgress,
			Complete,
			Claimed
		}

		private const int CHECKMARK_SPRITE_OFF_INDEX = 0;

		private const int CHECKMARK_SPRITE_ON_INDEX = 1;

		public Text CoinRewardText;

		public GameObjectSelector[] CompletionCheckMarks;

		public TintSelector[] CompletionBackgrounds;

		public GameObject CompletePanel;

		public GameObject ClaimedPanel;

		public GameObject MemberLockPanel;

		private ClubPenguin.Task.Task task;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void Start()
		{
			if (Service.Get<TaskService>().HasLoadedTasks)
			{
				setupTaskCompletion();
			}
			else
			{
				dispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded);
			}
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			setupTaskCompletion();
			return false;
		}

		private void setupTaskCompletion()
		{
			task = getTaskCompletionTask();
			if (task != null)
			{
				showTask(task);
				showMemberStatus();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private ClubPenguin.Task.Task getTaskCompletionTask()
		{
			ClubPenguin.Task.Task result = null;
			foreach (ClubPenguin.Task.Task task2 in Service.Get<TaskService>().Tasks)
			{
				if (task2.Definition.Category == TaskDefinition.TaskCategory.TaskCompletion)
				{
					result = task2;
					break;
				}
			}
			return result;
		}

		private void showTask(ClubPenguin.Task.Task task)
		{
			CoinRewardText.text = CoinRewardableDefinition.Coins(task.Definition.Reward).ToString();
			enableCheckMarks();
			showTaskProgress();
			changeState(getStateForTask());
		}

		public void OnClaimButtonClicked()
		{
			Service.Get<DailyChallengeService>().ClaimTaskReward(task);
			changeState(TaskState.Claimed);
			logClaimBI();
			Service.Get<EventDispatcher>().DispatchEvent(new TaskEvents.TaskRewardClaimed(task));
		}

		private void logClaimBI()
		{
			PresenceData component = Service.Get<CPDataEntityCollection>().GetComponent<PresenceData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			string tier = "";
			if (component != null)
			{
				tier = component.World;
			}
			Service.Get<ICPSwrveService>().Action("daily_task." + task.Definition.Group.ToString().ToLower() + ".claim_daily_reward", task.Definition.name, tier);
		}

		private void enableCheckMarks()
		{
			for (int i = 0; i < CompletionCheckMarks.Length; i++)
			{
				if (i < task.Definition.Threshold)
				{
					CompletionCheckMarks[i].gameObject.SetActive(true);
				}
				else
				{
					CompletionCheckMarks[i].gameObject.SetActive(false);
				}
			}
			for (int i = 0; i < CompletionBackgrounds.Length; i++)
			{
				if (i < task.Definition.Threshold + 1)
				{
					CompletionBackgrounds[i].gameObject.SetActive(true);
				}
				else
				{
					CompletionBackgrounds[i].gameObject.SetActive(false);
				}
			}
		}

		private void showTaskProgress()
		{
			for (int i = 0; i < CompletionCheckMarks.Length; i++)
			{
				if (i < task.Counter)
				{
					CompletionCheckMarks[i].SelectGameObject(1);
				}
				else
				{
					CompletionCheckMarks[i].SelectGameObject(0);
				}
			}
			for (int i = 0; i < CompletionBackgrounds.Length; i++)
			{
				if (i < task.Counter + 1)
				{
					CompletionBackgrounds[i].Select(1);
				}
				else
				{
					CompletionBackgrounds[i].Select(0);
				}
			}
		}

		private void showMemberStatus()
		{
			if (MemberLockPanel != null)
			{
				MemberLockPanel.SetActive(!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember());
			}
		}

		private TaskState getStateForTask()
		{
			TaskState result = TaskState.inProgress;
			if (task.IsRewardClaimed)
			{
				result = TaskState.Claimed;
			}
			else if (task.IsComplete)
			{
				result = TaskState.Complete;
			}
			return result;
		}

		private void changeState(TaskState newState)
		{
			switch (newState)
			{
			case TaskState.Claimed:
				CompletePanel.SetActive(false);
				ClaimedPanel.SetActive(true);
				break;
			case TaskState.Complete:
				ClaimedPanel.SetActive(false);
				CompletePanel.SetActive(true);
				break;
			case TaskState.inProgress:
				ClaimedPanel.SetActive(false);
				CompletePanel.SetActive(false);
				break;
			}
		}
	}
}
