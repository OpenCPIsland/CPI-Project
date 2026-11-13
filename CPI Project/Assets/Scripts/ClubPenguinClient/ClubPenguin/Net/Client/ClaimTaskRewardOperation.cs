using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpPOST]
	[HttpContentType("text/plain")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[RequestQueue("Task")]
	[HttpPath("cp-api-base-uri", "/task/v1/reward")]
	public class ClaimTaskRewardOperation : CPAPIHttpOperation
	{
		[HttpRequestTextBody]
		public string RequestBody;

		[HttpResponseJsonBody]
		public ClaimTaskRewardResponse ResponseBody;

		public ClaimTaskRewardOperation(string taskId)
		{
			RequestBody = taskId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = new ClaimTaskRewardResponse();
			DailyTaskProgress dailyTaskProgress = SetTaskProgressOperation.ReadCurrentDay(offlineDatabase, offlineDefinitions);
			TaskProgress taskProgress = default(TaskProgress);
			taskProgress.taskId = RequestBody;
			bool alreadyClaimed = false;
			for (int i = 0; i < dailyTaskProgress.Tasks.Count; i++)
			{
				if (dailyTaskProgress.Tasks[i].taskId == RequestBody)
				{
					taskProgress = dailyTaskProgress.Tasks[i];
					alreadyClaimed = taskProgress.claimed;
					break;
				}
			}
			if (alreadyClaimed)
			{
				// Answered without a reward: the task was already paid out, and the
				// caller treats an empty reward as "nothing more to give".
				return;
			}
			if (!offlineDefinitions.IsTaskComplete(RequestBody))
			{
				// Same empty answer for a task that is not finished. The button that
				// asks for this is greyed out until it is, but that is the interface
				// being polite rather than a rule anybody is holding to.
				return;
			}
			Reward reward = offlineDefinitions.GetTaskReward(RequestBody);
			if (reward != null && !reward.isEmpty())
			{
				offlineDefinitions.AddReward(reward, ResponseBody);
				JsonService jsonService = Service.Get<JsonService>();
				ResponseBody.reward = jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward)));
			}
			taskProgress.claimed = true;
			SetTaskProgressOperation.SaveProgress(offlineDatabase, offlineDefinitions, taskProgress);
		}
	}
}
