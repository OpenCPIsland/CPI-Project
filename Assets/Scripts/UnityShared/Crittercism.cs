using Disney.MobileNetwork;
using System;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

public class Crittercism : MonoBehaviour
{
	private const string HANDLED_EXCEPTION_PREFIX = "[Handled Exception] ";

	private const string HANDLED_FATAL = "FATAL";

	private const string HANDLED_EXCEPTION = "EXCEPTION";

	private const string HANDLED_CRITICAL = "CRITICAL";

	private const string HANDLED_ERROR = "ERROR";

	private const string HANDLED_NETWORK_ERROR = "NETWORK_ERROR";

	public string[] UrlFilters = new string[0];

	public bool EnableFloodFiltering = true;

	public int FloodThresholdMS = 200;

	public string[] LogStrippingPatterns = new string[1]
	{
		"[0-9]{2}:[0-9]{2}:[0-9]{2}\\s[a-zA-z]{2}"
	};

	public void Init(bool postShudown, IStandaloneErrorLogger errorLogger)
	{
		if (postShudown)
		{
			StandaloneErrorReporter.Init(errorLogger);
			return;
		}
		Service.Set(errorLogger);
		StandaloneErrorReporter.Init(errorLogger);
		SetLogUnhandledExceptionAsCrash(false);
	}

	public void ForceCrash()
	{
		StandaloneErrorReporter.ForceCrash();
	}

	public static void LogHandledException(Exception e)
	{
		StandaloneErrorReporter.LogHandledException(e);
	}

	public static bool GetOptOut()
	{
		return StandaloneErrorReporter.GetOptOut();
	}

	public static void SetOptOut(bool isOptedOut)
	{
		StandaloneErrorReporter.SetOptOut(isOptedOut);
	}

	public static void SetUsername(string username)
	{
		StandaloneErrorReporter.SetUsername(username);
	}

	public static void SetValue(string key, string value)
	{
		StandaloneErrorReporter.SetValue(key, value);
	}

	public static void SetMetadata(string[] keys, string[] values)
	{
		int num = keys.Length;
		for (int i = 0; i < num; i++)
		{
			string val = keys[i];
			string key = values[i];
			StandaloneErrorReporter.SetValue(val, key);
		}
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
		StandaloneErrorReporter.LeaveBreadcrumb(breadcrumb);
	}

	public static void LogNetworkRequest(string method, string uriString, double latencyInSeconds, int bytesRead, int bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
		StandaloneErrorReporter.LogNetworkRequest(method, uriString, (long)latencyInSeconds * 1000, bytesRead, bytesSent, responseCode, exceptionStatus);
	}

	public static void LogNetworkRequest(string method, string uriString, long latencyInMilliseconds, int bytesRead, int bytesSent, HttpStatusCode responseCode, WebExceptionStatus exceptionStatus)
	{
		StandaloneErrorReporter.LogNetworkRequest(method, uriString, (double)latencyInMilliseconds / 1000.0, bytesRead, bytesSent, responseCode, exceptionStatus);
	}

	public static bool DidCrashOnLastLoad()
	{
		return StandaloneErrorReporter.DidCrashOnLastLoad();
	}

	public static void BeginUserflow(string name)
	{
		StandaloneErrorReporter.BeginUserflow(name);
	}

	public static void BeginUserflow(string name, int value)
	{
		StandaloneErrorReporter.BeginUserflow(name, value);
	}

	public static void CancelUserflow(string name)
	{
		StandaloneErrorReporter.CancelUserflow(name);
	}

	public static void EndUserflow(string name)
	{
		StandaloneErrorReporter.EndUserflow(name);
	}

	public static void FailUserflow(string name)
	{
		StandaloneErrorReporter.FailUserflow(name);
	}

	public static void SetUserflowValue(string name, int value)
	{
		StandaloneErrorReporter.SetUserflowValue(name, value);
	}

	public static int GetUserflowValue(string name)
	{
		return StandaloneErrorReporter.GetUserflowValue(name);
	}

	public static void SetLogUnhandledExceptionAsCrash(bool value)
	{
		StandaloneErrorReporter.SetLogUnhandledExceptionAsCrash(value);
	}

	public static bool GetLogUnhandledExceptionAsCrash()
	{
		return StandaloneErrorReporter.GetLogUnhandledExceptionAsCrash();
	}

	public static string addErrorPrefix(string logString)
	{
		string str = "";
		if (logString.Contains("EXCEPTION"))
		{
			str += string.Format("{0} {1} ", "[Handled Exception] ", "EXCEPTION");
		}
		else if (logString.Contains("CRITICAL"))
		{
			str += string.Format("{0} {1} ", "[Handled Exception] ", "CRITICAL");
		}
		else if (logString.Contains("FATAL"))
		{
			str += string.Format("{0} {1} ", "[Handled Exception] ", "FATAL");
		}
		else if (logString.Contains("NETWORK_ERROR"))
		{
			str += string.Format("{0} {1} ", "[Handled Exception] ", "NETWORK_ERROR");
		}
		else if (logString.Contains("ERROR"))
		{
			str += string.Format("{0} {1} ", "[Handled Exception] ", "ERROR");
		}
		return str + stripTimestamps(logString);
	}

	public static string stripTimestamps(string logString)
	{
		return Regex.Replace(logString, "\\[\\d+\\:\\d+\\:\\d+ ..\\]", "");
	}
}
