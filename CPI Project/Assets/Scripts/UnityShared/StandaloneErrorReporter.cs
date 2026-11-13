using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class StandaloneErrorReporter
{
	private static volatile bool logUnhandledExceptionAsCrash = false;

	private static IStandaloneErrorLogger logger;

	public static void Init(IStandaloneErrorLogger errorLogger)
	{
		logger = errorLogger;
		try
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			Application.logMessageReceivedThreaded += OnLogMessageReceived;
			Debug.Log("CrittercismStandalone: Sucessfully Initialized");
		}
		catch
		{
			Debug.Log("Crittercism Unity plugin failed to initialize.");
		}
	}

	public static string StackTrace(Exception e)
	{
		string text = e.StackTrace;
		List<Exception> list = new List<Exception>();
		list.Add(e);
		if (text != null)
		{
			Exception innerException = e.InnerException;
			while (innerException != null && list.IndexOf(innerException) < 0)
			{
				list.Add(innerException);
				text = innerException.GetType().FullName + " : " + innerException.Message + "\r\n" + innerException.StackTrace + "\r\n" + text;
				innerException = innerException.InnerException;
			}
		}
		else
		{
			text = "";
		}
		return Crittercism.stripTimestamps(text);
	}

	public static void LogHandledException(Exception e)
	{
		if (e != null)
		{
			string stackTrace = StackTraceUtility.ExtractStackTrace();
			logger.LogHandledException(e, stackTrace);
		}
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
		bool result = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = false;
		}
		return result;
	}

	public static void BeginUserflow(string name)
	{
	}

	[Obsolete("BeginTransaction is deprecated, please use BeginUserflow instead.")]
	public static void BeginTransaction(string name)
	{
		BeginUserflow(name);
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
		if (args != null && args.ExceptionObject != null)
		{
			try
			{
				Exception ex = args.ExceptionObject as Exception;
				if (ex != null)
				{
					logger.LogUnhandledException(ex);
				}
			}
			catch
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("CrittercismStandalone: Failed to log exception");
				}
			}
		}
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
		if (type == LogType.Exception && logUnhandledExceptionAsCrash)
		{
			return;
		}
		string text = string.Format("{0}\r\n{1}", Crittercism.stripTimestamps(logString), stack);
		switch (type)
		{
		case LogType.Warning:
		case LogType.Log:
			break;
		case LogType.Exception:
			if (stack.Contains("UnityEngine.Debug:LogException("))
			{
				logger.LogHandledExceptionMessage(text);
			}
			else
			{
				logger.LogUnhandledException(text);
			}
			break;
		case LogType.Error:
		case LogType.Assert:
			logger.LogError(text);
			break;
		}
	}

	public static void ForceCrash()
	{
	}
}
