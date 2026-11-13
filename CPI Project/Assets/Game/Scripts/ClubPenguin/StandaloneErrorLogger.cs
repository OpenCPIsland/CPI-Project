using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain.Diagnostics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.Core;

namespace ClubPenguin
{
	public class StandaloneErrorLogger : IStandaloneErrorLogger
	{
		private const int BATCH_LIMIT = 10;

		private IDiagnosticsService diagService;

		private readonly Queue<StructuredLogObject> queuedLogs = new Queue<StructuredLogObject>();

		private int batchedCount;

		internal IDiagnosticsService DiagnosticsService
		{
			set
			{
				diagService = value;
				if (diagService != null)
				{
					Flush();
				}
			}
		}

		public StandaloneErrorLogger()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionStart>(OnSceneTransitionStart);
		}

		private bool OnSceneTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			Flush();
			return false;
		}

		public void LogHandledException(Exception ex, string stackTrace)
		{
			StructuredLogObject logObject = createStructuredLogObject(ex.ToString(), LogMessageType.HandledException, stackTrace);
			log(logObject, true);
		}

		public void LogHandledExceptionMessage(string message)
		{
			StructuredLogObject logObject = createStructuredLogObject(message, LogMessageType.HandledException);
			log(logObject, true);
		}

		public void LogUnhandledException(Exception ex)
		{
			StructuredLogObject logObject = createStructuredLogObject(ex.ToString(), LogMessageType.UnhandledException);
			log(logObject);
		}

		public void LogUnhandledException(string message)
		{
			StructuredLogObject logObject = createStructuredLogObject(message, LogMessageType.HandledException);
			log(logObject);
		}

		public void LogError(string errorMsg)
		{
			StructuredLogObject logObject = createStructuredLogObject(errorMsg, LogMessageType.Error);
			log(logObject, true);
		}

		private StructuredLogObject createStructuredLogObject(string message, LogMessageType type, string stackTrace = null)
		{
			string playerName = null;
			string playerId = null;
			try
			{
				SessionManager sessionManager = Service.Get<SessionManager>();
				playerName = sessionManager.LocalUser.DisplayName.Text;
				playerId = sessionManager.LocalUser.Id;
			}
			catch (Exception)
			{
			}
			return diagService.CreateBaseStructuredLogObject(message, type, stackTrace, playerId, playerName);
		}

		private void log(StructuredLogObject logObject, bool batch = false)
		{
			batchedCount++;
			if (diagService == null)
			{
				queuedLogs.Enqueue(logObject);
				return;
			}
			diagService.LogBatch(Log.PriorityFlags.ERROR, logObject, LogChannelType.Exception);
			if (!batch || batchedCount > 10)
			{
				Flush();
			}
		}

		[Invokable("Debug.ErrorTests.FlushErrorLogger")]
		public void Flush()
		{
			if (queuedLogs.Count > 0)
			{
				foreach (StructuredLogObject queuedLog in queuedLogs)
				{
					diagService.LogBatch(Log.PriorityFlags.ERROR, queuedLog, LogChannelType.Exception);
				}
				queuedLogs.Clear();
			}
			batchedCount = 0;
			diagService.LogFlush();
		}
	}
}
