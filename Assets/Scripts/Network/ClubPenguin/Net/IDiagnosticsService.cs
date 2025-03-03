using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain.Diagnostics;
using Disney.LaunchPadFramework;
using System;

namespace ClubPenguin.Net
{
	public interface IDiagnosticsService : INetworkService
	{
		void LogImmediate(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default);

		void LogBatch(Log.PriorityFlags logLevel, StructuredLogObject logObject, LogChannelType channel = LogChannelType.Default);

		void LogExceptionImmediate(Exception ex);

		void LogExceptionBatch(Exception ex);

		void LogFlush();

		StructuredLogObject CreateBaseStructuredLogObject(string message, LogMessageType type, string stackTrace = null, string playerId = null, string playerName = null);
	}
}
