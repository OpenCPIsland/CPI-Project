using System;
using System.Net;
using UnityEngine;

public static class CrittercismIOS
{
	private const int crUnityId = 0;

	private static volatile bool logUnhandledExceptionAsCrash = false;

	public static void Init(string appID, string[] urlFilters)
	{
	}

	private static string StackTrace(Exception e)
	{
		return "";
	}

	public static void LogHandledException(Exception e)
	{
	}

	public static bool GetOptOut()
	{
		return true;
	}

	public static void SetOptOut(bool isOptedOut)
	{
	}

	public static void SetUsername(string username)
	{
	}

	public static void SetValue(string val, string key)
	{
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
	}

	public static void LogNetworkRequest(string method, string uriString, double latency, int bytesRead, int bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
	}

	public static bool DidCrashOnLastLoad()
	{
		return false;
	}

	public static void BeginUserflow(string name)
	{
	}

	[Obsolete("BeginTransaction is deprecated, please use BeginUserflow instead.")]
	public static void BeginTransaction(string name)
	{
	}

	public static void BeginUserflow(string name, int value)
	{
	}

	[Obsolete("BeginTransaction is deprecated, please use BeginUserflow instead.")]
	public static void BeginTransaction(string name, int value)
	{
		BeginUserflow(name, value);
	}

	public static void CancelUserflow(string name)
	{
	}

	[Obsolete("CancelTransaction is deprecated, please use CancelUserflow instead.")]
	public static void CancelTransaction(string name)
	{
		CancelUserflow(name);
	}

	public static void EndUserflow(string name)
	{
	}

	[Obsolete("EndTransaction is deprecated, please use EndUserflow instead.")]
	public static void EndTransaction(string name)
	{
		EndUserflow(name);
	}

	public static void FailUserflow(string name)
	{
	}

	[Obsolete("FailTransaction is deprecated, please use FailUserflow instead.")]
	public static void FailTransaction(string name)
	{
		FailUserflow(name);
	}

	public static void SetUserflowValue(string name, int value)
	{
	}

	[Obsolete("SetTransactionValue is deprecated, please use SetUserflowValue instead.")]
	public static void SetTransactionValue(string name, int value)
	{
		SetUserflowValue(name, value);
	}

	public static int GetUserflowValue(string name)
	{
		return -1;
	}

	[Obsolete("GetTransactionValue is deprecated, please use GetUserflowValue instead.")]
	public static int GetTransactionValue(string name)
	{
		return GetUserflowValue(name);
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
	{
	}

	public static void SetLogUnhandledExceptionAsCrash(bool value)
	{
	}

	public static bool GetLogUnhandledExceptionAsCrash()
	{
		return logUnhandledExceptionAsCrash;
	}

	private static void OnLogMessageReceived(string logString, string stack, LogType type)
	{
	}
}
