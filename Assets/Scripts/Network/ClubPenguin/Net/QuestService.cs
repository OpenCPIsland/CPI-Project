using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.Net
{
	internal class QuestService : BaseNetworkService, IQuestService, INetworkService
	{
		private struct PendingQuestUpdateEvent
		{
			public EventType eventType;

			public QuestStatus status;

			public string questId;

			public string objective;

			public SignedResponse<QuestStateCollection> syncData;
		}

		private enum EventType
		{
			STATUS_UPDATE,
			PROGRESS_UPDATE,
			SYNC_WITH_GAME_SERVER
		}

		private const float REQUEST_TIMEOUT_TIME = 35f;

		private Queue<PendingQuestUpdateEvent> requests = new Queue<PendingQuestUpdateEvent>();

		private bool _processingRequest = false;

		private bool processingRequest
		{
			get
			{
				return _processingRequest;
			}
			set
			{
				_processingRequest = value;
				if (!_processingRequest)
				{
					processRequest();
				}
			}
		}

		public QuestStatus? PendingSetStatusStatus
		{
			get
			{
				QuestStatus? result = null;
				foreach (PendingQuestUpdateEvent request in requests)
				{
					if (request.eventType == EventType.STATUS_UPDATE)
					{
						result = request.status;
					}
				}
				return result;
			}
		}

		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.QUEST_OBJECTIVES_UPDATED, onObjectiveCompleted);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.QUEST_ERROR, onQuestProgressionError);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.QUEST_DATA_SYNCED, onQuestDataSynced);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ON_QUEST, onQuest);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_JOIN, onConnected);
		}

		public void SetStatus(string questId, QuestStatus status)
		{
			PendingQuestUpdateEvent item = default(PendingQuestUpdateEvent);
			item.eventType = EventType.STATUS_UPDATE;
			item.status = status;
			item.questId = questId;
			requests.Enqueue(item);
			processRequest();
		}

		private void setStatus(string questId, QuestStatus status)
		{
			if (status == QuestStatus.SUSPENDED)
			{
				ClearQueue();
			}
			Stopwatch sw = new Stopwatch();
			sw.Start();
			APICall<SetStatusOperation> aPICall = clubPenguinClient.QuestApi.SetStatus(questId, status);
			aPICall.OnResponse += delegate(SetStatusOperation op, HttpResponse response)
			{
				RewardSource rewardSource = RewardSource.QUEST_STARTED;
				if (status == QuestStatus.COMPLETED)
				{
					rewardSource = RewardSource.QUEST_COMPLETED;
				}
				if (sw.Elapsed.TotalSeconds >= 10.0)
				{
					Log.LogNetworkErrorFormatted(this, "Set status for quest took a long time: {0}", sw.Elapsed);
				}
				questDataReturned(op.ResponseBody, rewardSource);
			};
			aPICall.OnError += handleRequestError;
			aPICall.Execute();
		}

		public void CompleteObjective(string objective)
		{
			PendingQuestUpdateEvent item = default(PendingQuestUpdateEvent);
			item.eventType = EventType.PROGRESS_UPDATE;
			item.objective = objective;
			requests.Enqueue(item);
			processRequest();
		}

		private void completeObjective(string objective)
		{
			clubPenguinClient.GameServer.QuestCompleteObjective(objective);
		}

		private void onObjectiveCompleted(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<QuestObjectives> progress = (SignedResponse<QuestObjectives>)data;
			APICall<SetProgressOperation> aPICall = clubPenguinClient.QuestApi.SetProgress(progress);
			aPICall.OnResponse += objectiveCompleteQuestDataReturned;
			aPICall.OnError += handleRequestError;
			aPICall.OnError += onQuestProgressionError;
			aPICall.Execute();
		}

		private void objectiveCompleteQuestDataReturned(SetProgressOperation operation, HttpResponse httpResponse)
		{
			questDataReturned(operation.ResponseBody, RewardSource.QUEST_OBJECTIVE);
		}

		private void onQuestDataSynced(GameServerEvent gameServerEvent, object data)
		{
			processingRequest = false;
		}

		private void onQuestProgressionError(GameServerEvent gameServerEvent, object data)
		{
			onQuestProgressionError();
			processingRequest = false;
		}

		private void onQuestProgressionError(SetProgressOperation operation, HttpResponse httpResponse)
		{
			onQuestProgressionError();
		}

		public void RestartQuest(string questId)
		{
		}

		protected void handleRequestError(CPAPIHttpOperation operation, HttpResponse httpResponse)
		{
			handleCPResponseError(httpResponse);
			processingRequest = false;
		}

		public void ClearQueue()
		{
			requests.Clear();
			processingRequest = false;
		}

		private void processRequest()
		{
			if (!processingRequest && requests.Count > 0 && clubPenguinClient.GameServer.CurrentRoom() != null)
			{
				CoroutineRunner.StopAllForOwner(this);
				PendingQuestUpdateEvent lastRequest = requests.Dequeue();
				processingRequest = true;
				switch (lastRequest.eventType)
				{
				case EventType.STATUS_UPDATE:
					setStatus(lastRequest.questId, lastRequest.status);
					break;
				case EventType.PROGRESS_UPDATE:
					completeObjective(lastRequest.objective);
					break;
				case EventType.SYNC_WITH_GAME_SERVER:
					questDataReturned(lastRequest.syncData);
					break;
				}
				CoroutineRunner.Start(processRequestTimeOut(lastRequest), this, "ProcessRequestTimeout");
			}
		}

		private IEnumerator processRequestTimeOut(PendingQuestUpdateEvent lastRequest)
		{
			yield return new WaitForSeconds(35f);
			if (processingRequest)
			{
				Log.LogErrorFormatted(this, "QuestService process request timeout \neventType: {0} \nstatus: {1} \nquestId: {2} \nobjective: {3}", lastRequest.eventType, lastRequest.status, lastRequest.questId, lastRequest.objective);
				processingRequest = false;
				processRequest();
			}
		}

		private void onConnected(GameServerEvent gameServerEvent, object data)
		{
			processRequest();
		}

		private void questDataReturned(QuestChangeResponse data, RewardSource rewardSource)
		{
			questDataReturned(data.questStateCollection);
			if (data.reward != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(rewardSource, data.questId, data.reward.ToReward()));
			}
			handleCPResponse(data);
		}

		private void questDataReturned(SignedResponse<QuestStateCollection> data)
		{
			if (clubPenguinClient.GameServer.CurrentRoom() == null)
			{
				PendingQuestUpdateEvent item = default(PendingQuestUpdateEvent);
				item.eventType = EventType.SYNC_WITH_GAME_SERVER;
				item.syncData = data;
				requests.Enqueue(item);
				processingRequest = false;
				return;
			}
			clubPenguinClient.GameServer.QuestSetQuestState(data);
			QuestStateCollection questStateCollection = new QuestStateCollection();
			questStateCollection.AddRange(data.Data);
			QuestStateCollection questStateCollection2 = new QuestStateCollection();
			foreach (PendingQuestUpdateEvent request in requests)
			{
				switch (request.eventType)
				{
				case EventType.STATUS_UPDATE:
				{
					for (int i = 0; i < questStateCollection.Count; i++)
					{
						if (questStateCollection[i].questId == request.questId)
						{
							questStateCollection[i].status = request.status;
							if (request.status == QuestStatus.COMPLETED)
							{
								questStateCollection2.Add(questStateCollection[i]);
							}
						}
					}
					break;
				}
				case EventType.PROGRESS_UPDATE:
				{
					for (int i = 0; i < questStateCollection.Count; i++)
					{
						if (questStateCollection[i].status == QuestStatus.ACTIVE)
						{
							if (questStateCollection[i].completedObjectives == null)
							{
								questStateCollection[i].completedObjectives = new QuestObjectives();
							}
							questStateCollection[i].completedObjectives.Add(request.objective);
							break;
						}
					}
					break;
				}
				}
			}
			for (int i = 0; i < questStateCollection2.Count; i++)
			{
				questStateCollection.Remove(questStateCollection2[i]);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new QuestServiceEvents.QuestStatesRecieved(questStateCollection));
		}

		private void onQuest(GameServerEvent gameServerEvent, object data)
		{
			OnQuestState onQuestState = (OnQuestState)data;
			Service.Get<EventDispatcher>().DispatchEvent(new QuestServiceEvents.PlayerOnQuest(onQuestState.SessionId, onQuestState.MascotName));
		}

		private void onQuestProgressionError()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(QuestServiceErrors.QuestProgressionError));
			ClearQueue();
		}

		public void QA_SetProgress(QuestObjectives objectives)
		{
			APICall<QASetProgressOperation> aPICall = clubPenguinClient.QuestApi.QA_SetProgress(objectives);
			aPICall.OnResponse += delegate(QASetProgressOperation op, HttpResponse response)
			{
				questDataReturned(op.ResponseBody, RewardSource.QUEST_OBJECTIVE);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void QA_SetStatus(string questId, QuestStatus status)
		{
			APICall<QASetStatusOperation> aPICall = clubPenguinClient.QuestApi.QA_SetStatus(questId, status);
			aPICall.OnResponse += delegate(QASetStatusOperation op, HttpResponse response)
			{
				RewardSource rewardSource = RewardSource.QUEST_STARTED;
				if (status == QuestStatus.COMPLETED)
				{
					rewardSource = RewardSource.QUEST_COMPLETED;
				}
				questDataReturned(op.ResponseBody, rewardSource);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void QA_ResetQuest(string questId)
		{
			APICall<QAClearQuestOperation> aPICall = clubPenguinClient.QuestApi.QA_ClearQuest(questId);
			aPICall.OnResponse += delegate(QAClearQuestOperation op, HttpResponse response)
			{
				questDataReturned(op.ResponseBody);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void QA_UnlockQuest(string questId)
		{
			APICall<QAUnlockQuestOperation> aPICall = clubPenguinClient.QuestApi.QA_UnlockQuest(questId);
			aPICall.OnResponse += delegate(QAUnlockQuestOperation op, HttpResponse response)
			{
				questDataReturned(op.ResponseBody);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}
	}
}
