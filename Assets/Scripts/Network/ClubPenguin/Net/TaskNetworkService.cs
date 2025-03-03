using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using UnityEngine;

namespace ClubPenguin.Net
{
	internal class TaskNetworkService : BaseNetworkService, ITaskService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.TASK_COUNT_UPDATED, onTaskCountUpdated);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.TASK_PROGRESS_UPDATED, onTaskProgressUpdated);
		}

		public void Pickup(string path, string tag, Vector3 position)
		{
			clubPenguinClient.GameServer.Pickup(path, tag, position);
		}

		public void ClaimReward(string taskId)
		{
			APICall<ClaimTaskRewardOperation> aPICall = clubPenguinClient.TaskApi.ClaimTaskReward(taskId);
			aPICall.OnResponse += delegate(ClaimTaskRewardOperation op, HttpResponse httpResponse)
			{
				Reward reward = op.ResponseBody.reward.ToReward();
				if (reward != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.TASK, taskId, reward));
				}
				handleCPResponse(op.ResponseBody);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onTaskCountUpdated(GameServerEvent gameServerEvent, object data)
		{
			TaskProgress taskProgress = (TaskProgress)data;
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.TaskCounterChanged(taskProgress.taskId, taskProgress.counter));
		}

		private void onTaskProgressUpdated(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<TaskProgress> progress = (SignedResponse<TaskProgress>)data;
			APICall<SetTaskProgressOperation> aPICall = clubPenguinClient.TaskApi.SetProgress(progress);
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}
	}
}
