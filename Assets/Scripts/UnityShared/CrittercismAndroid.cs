using System;
using System.Net;
using UnityEngine;

public static class CrittercismAndroid
{
	public static void Init(string appID, string[] urlFilters)
	{
	}

	public static void Init(string appID, CrittercismConfig config)
	{
	}

	private static string StackTrace(Exception e)
	{
		return "";
	}

	public static void LogHandledException(Exception e)
	{
	}

	private static void LogUnhandledException(Exception e)
	{
	}

	public static void LogNetworkRequest(string method, string uriString, long latency, long bytesRead, long bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
	}

	public static bool GetOptOut()
	{
		return true;
	}

	public static void SetOptOut(bool optOutStatus)
	{
	}

	public static bool DidCrashOnLastLoad()
	{
		return false;
	}

	public static void SetUsername(string username)
	{
	}

	public static void SetMetadata(string[] keys, string[] values)
	{
	}

	public static void SetValue(string key, string value)
	{
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
	}

	public static void BeginUserflow(string userflowName)
	{
	}

	[Obsolete("BeginTransaction is deprecated, please use BeginUserflow instead.")]
	public static void BeginTransaction(string userflowName)
	{
		BeginUserflow(userflowName);
	}

	public static void CancelUserflow(string userflowName)
	{
	}

	[Obsolete("CancelTransaction is deprecated, please use CancelUserflow instead.")]
	public static void CancelTransaction(string userflowName)
	{
		CancelUserflow(userflowName);
	}

	public static void EndUserflow(string userflowName)
	{
	}

	[Obsolete("EndTransaction is deprecated, please use EndUserflow instead.")]
	public static void EndTransaction(string userflowName)
	{
		EndUserflow(userflowName);
	}

	public static void FailUserflow(string userflowName)
	{
	}

	[Obsolete("FailTransaction is deprecated, please use FailUserflow instead.")]
	public static void FailTransaction(string userflowName)
	{
		FailUserflow(userflowName);
	}

	public static void SetUserflowValue(string userflowName, int value)
	{
	}

	[Obsolete("SetTransactionValue is deprecated, please use SetUserflowValue instead.")]
	public static void SetTransactionValue(string userflowName, int value)
	{
		SetUserflowValue(userflowName, value);
	}

	public static int GetUserflowValue(string userflowName)
	{
		return -1;
	}

	[Obsolete("GetTransactionValue is deprecated, please use GetUserflowValue instead.")]
	public static int GetTransactionValue(string userflowName)
	{
		return GetUserflowValue(userflowName);
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
	{
	}

	public static void SetLogUnhandledExceptionAsCrash(bool value)
	{
	}

	public static bool GetLogUnhandledExceptionAsCrash()
	{
		return false;
	}

	private static void OnLogMessageReceived(string logString, string stack, LogType type)
	{
	}

	private static void PluginCallStatic(string methodName, params object[] args)
	{
	}

	private static RetType PluginCallStatic<RetType>(string methodName, params object[] args)
	{
		return default(RetType);
	}
}
