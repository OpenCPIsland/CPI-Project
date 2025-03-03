using ClubPenguin.Adventure;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestLogDailyTasks : MonoBehaviour
	{
		public Transform ScrollContent;

		public GameObject VideoTeaserPanel;

		private static bool hasWatchedVideo = false;

		public void Start()
		{
			LoadDailyTaskItems();
			ShowVideoTeaser();
		}

		public void OnVideoWatchButtonClick()
		{
			hasWatchedVideo = true;
			VideoTeaserPanel.SetActive(false);
		}

		private void ShowVideoTeaser()
		{
			if (!hasWatchedVideo && (Service.Get<QuestService>().ActiveQuest == null || Service.Get<QuestService>().ActiveQuest.Definition.name != "FTUE1"))
			{
				VideoTeaserPanel.SetActive(true);
			}
		}

		private void LoadDailyTaskItems()
		{
			foreach (ClubPenguin.Task.Task task in Service.Get<TaskService>().Tasks)
			{
				CoroutineRunner.Start(LoadDailyTaskItem(task), this, "QuestLogDailyTasks.LoadDailyTaskItem");
			}
		}

		private IEnumerator LoadDailyTaskItem(ClubPenguin.Task.Task taskData)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(taskData.Definition.TaskLogItemContentKey);
			yield return assetRequest;
			GameObject dailyTaskItemGameObject = Object.Instantiate(assetRequest.Asset);
			dailyTaskItemGameObject.transform.SetParent(ScrollContent, false);
			QuestLogDailyTasksItem dailyTaskItem = dailyTaskItemGameObject.GetComponent<QuestLogDailyTasksItem>();
			dailyTaskItem.LoadDailyTaskData(taskData);
		}
	}
}
