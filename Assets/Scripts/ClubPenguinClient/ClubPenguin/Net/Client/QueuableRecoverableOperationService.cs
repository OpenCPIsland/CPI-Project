using Disney.LaunchPadFramework;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class QueuableRecoverableOperationService : RecoverableOperationService
	{
		protected class QueuedOperation
		{
			public readonly HttpOperation Operation;

			public readonly string[] Parameters;

			public readonly string[] Queues;

			public QueuedOperation(HttpOperation operation, string[] parameters, RequestQueueAttribute[] operationQueues)
			{
				Operation = operation;
				Parameters = parameters;
				int num = operationQueues.Length;
				Queues = new string[num];
				for (int i = 0; i < num; i++)
				{
					Queues[i] = operationQueues[i].QueueId;
				}
			}
		}

		private Dictionary<string, Queue<QueuedOperation>> queues;

		public QueuableRecoverableOperationService(ClubPenguinClient clubPenguinClient)
			: base(clubPenguinClient)
		{
			queues = new Dictionary<string, Queue<QueuedOperation>>();
		}

		protected override void sendOperation(HttpOperation operation, params string[] parameters)
		{
			RequestQueueAttribute[] array = (RequestQueueAttribute[])operation.GetType().GetCustomAttributes(typeof(RequestQueueAttribute), true);
			if (array.Length == 0)
			{
				base.sendOperation(operation, parameters);
				return;
			}
			QueuedOperation pending = new QueuedOperation(operation, parameters, array);
			addToQueue(pending);
			processQueue(pending);
		}

		private void addToQueue(QueuedOperation pending)
		{
			for (int i = 0; i < pending.Queues.Length; i++)
			{
				string key = pending.Queues[i];
				if (!queues.ContainsKey(key))
				{
					queues.Add(key, new Queue<QueuedOperation>());
				}
				queues[key].Enqueue(pending);
			}
		}

		private void processQueue(QueuedOperation pending)
		{
			for (int i = 0; i < pending.Queues.Length; i++)
			{
				if (queues[pending.Queues[i]].Peek() != pending)
				{
					return;
				}
			}
			Delegate baseOnComplete = pending.Operation["on-complete"];
			pending.Operation["on-complete"] = (Action<HttpOperation, HttpResponse>)delegate(HttpOperation self, HttpResponse response)
			{
				try
				{
					baseOnComplete.DynamicInvoke(self, response);
				}
				catch (Exception ex)
				{
					Log.LogException(this, ex);
				}
				removeFromQueue(pending);
			};
			base.sendOperation(pending.Operation, pending.Parameters);
		}

		private void removeFromQueue(QueuedOperation pending)
		{
			for (int i = 0; i < pending.Queues.Length; i++)
			{
				Queue<QueuedOperation> queue = queues[pending.Queues[i]];
				QueuedOperation queuedOperation = queue.Dequeue();
				if (queuedOperation != pending)
				{
					Log.LogErrorFormatted(this, "Expected {0} at the front of queue {1} but found {2}", pending, queue, queuedOperation);
					processQueue(queuedOperation);
				}
				if (queue.Count > 0)
				{
					processQueue(queue.Peek());
				}
			}
		}
	}
}
