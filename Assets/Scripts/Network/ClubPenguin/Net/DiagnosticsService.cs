using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain.Diagnostics;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class DiagnosticsService : BaseNetworkService, IDiagnosticsService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		private void removeListeners()
		{
		}

		public void LogImmediate(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default)
		{
			clubPenguinClient.DiagnosticsApi.LogImmediate(logLevel, logObject, channel);
		}

		public void LogBatch(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default)
		{
			clubPenguinClient.DiagnosticsApi.LogBatch(logLevel, logObject, channel);
		}

		public void LogExceptionImmediate(Exception ex)
		{
			StructuredLogObject logObject = CreateBaseStructuredLogObject(ex.ToString(), LogMessageType.HandledException);
			LogImmediate(Log.PriorityFlags.ERROR, logObject, LogChannelType.Exception);
		}

		public void LogExceptionBatch(Exception ex)
		{
			StructuredLogObject logObject = CreateBaseStructuredLogObject(ex.ToString(), LogMessageType.HandledException);
			LogBatch(Log.PriorityFlags.ERROR, logObject, LogChannelType.Exception);
		}

		public void LogFlush()
		{
			clubPenguinClient.DiagnosticsApi.LogFlush();
		}

		public StructuredLogObject CreateBaseStructuredLogObject(string message, LogMessageType type, string stackTrace = null, string playerId = null, string playerName = null)
		{
			StructuredLogObject structuredLogObject = new StructuredLogObject();
			structuredLogObject.Value = message;
			structuredLogObject.LogMessageType = type.ToString();
			structuredLogObject.ClientVersion = ClientInfo.Instance.ClientVersion;
			structuredLogObject.DeviceModel = SystemInfo.deviceModel;
			structuredLogObject.OperatingSystem = SystemInfo.operatingSystem;
			structuredLogObject.SystemMemorySize = Convert.ToString(SystemInfo.systemMemorySize);
			if (!string.IsNullOrEmpty(stackTrace))
			{
				structuredLogObject.StackTrace = stackTrace;
			}
			playerName = (playerName ?? "<not set>");
			playerId = (playerId ?? "<not set>");
			structuredLogObject.PlayerName = playerName;
			structuredLogObject.PlayerId = playerId;
			return structuredLogObject;
		}
	}
}
