using NUnitLite;
using NUnitLite.Runner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class NUnitLiteUnityRunner
{
	private class UnitTestFailureException : Exception
	{
	}

	private static readonly HashSet<Assembly> Tested;

	public static Action<string, ResultSummary> Presenter
	{
		get;
		set;
	}

	private static bool TreatFailuresAsExceptions
	{
		get;
		set;
	}

	static NUnitLiteUnityRunner()
	{
		Tested = new HashSet<Assembly>();
		Presenter = UnityConsolePresenter;
	}

	public static void RunTests(bool failuesAreExceptions)
	{
		TreatFailuresAsExceptions = failuesAreExceptions;
		RunTests(Assembly.GetCallingAssembly());
	}

	private static void RunTests(Assembly assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		if (!Tested.Contains(assembly))
		{
			Tested.Add(assembly);
			using (StringWriter stringWriter = new StringWriter())
			{
				NUnitStreamUI nUnitStreamUI = new NUnitStreamUI(stringWriter);
				nUnitStreamUI.Execute(assembly);
				ResultSummary summary = nUnitStreamUI.Summary;
				string arg = stringWriter.GetStringBuilder().ToString();
				Presenter(arg, summary);
			}
		}
	}

	private static void UnityConsolePresenter(string longResult, ResultSummary result)
	{
		if (result != null && (result.ErrorCount > 0 || result.FailureCount > 0))
		{
			if (TreatFailuresAsExceptions)
			{
				Debug.LogError(longResult);
				throw new UnitTestFailureException();
			}
			Debug.LogWarning(longResult);
		}
		else
		{
			Debug.Log(longResult);
		}
	}
}
