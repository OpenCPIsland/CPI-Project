using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class QuestApi
	{
		private ClubPenguinClient clubPenguinClient;

		public QuestApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<SetProgressOperation> SetProgress(SignedResponse<QuestObjectives> objectives)
		{
			SetProgressOperation operation = new SetProgressOperation(objectives);
			return new APICall<SetProgressOperation>(clubPenguinClient, operation);
		}

		public APICall<QASetProgressOperation> QA_SetProgress(QuestObjectives objectives)
		{
			QASetProgressOperation operation = new QASetProgressOperation(objectives);
			return new APICall<QASetProgressOperation>(clubPenguinClient, operation);
		}

		public APICall<QASetStatusOperation> QA_SetStatus(string questId, QuestStatus status)
		{
			QASetStatusOperation operation = new QASetStatusOperation(questId, status);
			return new APICall<QASetStatusOperation>(clubPenguinClient, operation);
		}

		public APICall<QAClearQuestOperation> QA_ClearQuest(string questId)
		{
			QAClearQuestOperation operation = new QAClearQuestOperation(questId);
			return new APICall<QAClearQuestOperation>(clubPenguinClient, operation);
		}

		public APICall<QAUnlockQuestOperation> QA_UnlockQuest(string questId)
		{
			QAUnlockQuestOperation operation = new QAUnlockQuestOperation(questId);
			return new APICall<QAUnlockQuestOperation>(clubPenguinClient, operation);
		}

		public APICall<SetStatusOperation> SetStatus(string questId, QuestStatus status)
		{
			SetStatusOperation operation = new SetStatusOperation(questId, status);
			return new APICall<SetStatusOperation>(clubPenguinClient, operation);
		}
	}
}
