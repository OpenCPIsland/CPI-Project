using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class TaskApi
	{
		private ClubPenguinClient clubPenguinClient;

		public TaskApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<SetTaskProgressOperation> SetProgress(SignedResponse<TaskProgress> task)
		{
			SetTaskProgressOperation operation = new SetTaskProgressOperation(task);
			return new APICall<SetTaskProgressOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimTaskRewardOperation> ClaimTaskReward(string taskId)
		{
			ClaimTaskRewardOperation operation = new ClaimTaskRewardOperation(taskId);
			return new APICall<ClaimTaskRewardOperation>(clubPenguinClient, operation);
		}
	}
}
