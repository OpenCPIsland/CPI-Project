using ClubPenguin.NPC;
using ClubPenguin.Task;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestLogDailyTasksItem : MonoBehaviour
	{
		public Text XPText;

		public Text CoinsText;

		public Text InventoryText;

		public Text DescriptionText;

		public Image MascotIconImage;

		public Image MemberOnlyImage;

		public Image CompletedImage;

		private ClubPenguin.Task.Task taskData;

		public void LoadDailyTaskData(ClubPenguin.Task.Task taskData)
		{
			this.taskData = taskData;
			showTaskData(taskData);
			Service.Get<EventDispatcher>().AddListener<TaskEvents.TaskUpdated>(onTaskUpdated);
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<TaskEvents.TaskUpdated>(onTaskUpdated);
		}

		private void showTaskData(ClubPenguin.Task.Task taskData)
		{
			XPText.text = "";
			List<MascotXPRewardDefinition> definitions = taskData.Definition.Reward.GetDefinitions<MascotXPRewardDefinition>();
			if (definitions.Count > 0)
			{
				XPText.text = definitions[0].XP.ToString();
			}
			CoinsText.text = CoinRewardableDefinition.Coins(taskData.Definition.Reward).ToString();
			DescriptionText.text = taskData.Definition.Description;
			InventoryText.text = taskData.Counter + "/" + taskData.Definition.Threshold;
			CompletedImage.enabled = taskData.IsComplete;
			MemberOnlyImage.enabled = false;
		}

		private bool onTaskUpdated(TaskEvents.TaskUpdated evt)
		{
			if (evt.Task.Definition.Title == taskData.Definition.Title)
			{
				showTaskData(evt.Task);
			}
			return false;
		}
	}
}
