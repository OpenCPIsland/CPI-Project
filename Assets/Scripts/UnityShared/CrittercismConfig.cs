using UnityEngine;

public class CrittercismConfig
{
	public AndroidJavaObject GetAndroidConfig()
	{
		return null;
	}

	public string GetCustomVersionName()
	{
		return "";
	}

	public void SetCustomVersionName(string customVersionName)
	{
	}

	public void SetUrlBlackListPatterns(string[] patterns)
	{
	}

	public string[] GetUrlBlackListPatterns()
	{
		return new string[0];
	}

	public bool IsLogcatReportingEnabled()
	{
		return false;
	}

	public void SetLogcatReportingEnabled(bool shouldCollectLogcat)
	{
	}

	public bool IsServiceMonitoringEnabled()
	{
		return false;
	}

	public void SetServiceMonitoringEnabled(bool isServiceMonitoringEnabled)
	{
	}

	private void CallConfigMethod(string methodName, params object[] args)
	{
	}

	private RetType CallConfigMethod<RetType>(string methodName, params object[] args)
	{
		return default(RetType);
	}
}
