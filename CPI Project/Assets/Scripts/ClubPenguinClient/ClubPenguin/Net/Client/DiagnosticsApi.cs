using ClubPenguin.Net.Domain.Diagnostics;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class DiagnosticsApi
	{
		private ClubPenguinClient clubPenguinClient;

		private Dictionary<Log.PriorityFlags, string> logLevelsToString = new Dictionary<Log.PriorityFlags, string>();

		private List<LogParameters> logParametersList = new List<LogParameters>();

		public DiagnosticsApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
			foreach (Log.PriorityFlags value in Enum.GetValues(typeof(Log.PriorityFlags)))
			{
				logLevelsToString[value] = value.ToString().ToUpper();
			}
		}

		public APICall<PostDiagnosticsLogOperation> PostDiagnosticsLog(List<LogParameters> logParametersList)
		{
			PostDiagnosticsLogOperation operation = new PostDiagnosticsLogOperation(logParametersList);
			return new APICall<PostDiagnosticsLogOperation>(clubPenguinClient, operation);
		}

		public void LogImmediate(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default)
		{
			if (!clubPenguinClient.OfflineMode)
			{
				LogParameters logParameters = new LogParameters();
				logParameters.logName = channel.ToString();
				logParameters.severity = logLevelsToString[logLevel];
				logParameters.message = Service.Get<JsonService>().Serialize(logObject);
				logParametersList.Add(logParameters);
				APICall<PostDiagnosticsLogOperation> aPICall = PostDiagnosticsLog(logParametersList);
				aPICall.OnComplete += onPostDiagnosticsLog;
				aPICall.Execute();
			}
		}

		public void LogBatch(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default)
		{
			if (!clubPenguinClient.OfflineMode)
			{
				LogParameters logParameters = new LogParameters();
				logParameters.logName = channel.ToString();
				logParameters.severity = logLevelsToString[logLevel];
				logParameters.message = Service.Get<JsonService>().Serialize(logObject);
				logParametersList.Add(logParameters);
			}
		}

		public void LogFlush()
		{
			if (!clubPenguinClient.OfflineMode)
			{
				APICall<PostDiagnosticsLogOperation> aPICall = PostDiagnosticsLog(logParametersList);
				aPICall.OnComplete += onPostDiagnosticsLog;
				aPICall.Execute();
			}
		}

		private void onPostDiagnosticsLog(PostDiagnosticsLogOperation operation, HttpResponse response)
		{
			logParametersList.Clear();
		}
	}
}
