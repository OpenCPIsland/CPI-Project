using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class IntegrationTest
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExcludePlatformAttribute : Attribute
	{
		public string[] platformsToExclude;

		public ExcludePlatformAttribute(params RuntimePlatform[] platformsToExclude)
		{
			this.platformsToExclude = platformsToExclude.Select((RuntimePlatform platform) => platform.ToString()).ToArray();
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExpectExceptions : Attribute
	{
		public string[] exceptionTypeNames;

		public bool succeedOnException;

		public ExpectExceptions()
			: this(false)
		{
		}

		public ExpectExceptions(bool succeedOnException)
			: this(succeedOnException, new string[0])
		{
		}

		public ExpectExceptions(bool succeedOnException, params string[] exceptionTypeNames)
		{
			this.succeedOnException = succeedOnException;
			this.exceptionTypeNames = exceptionTypeNames;
		}

		public ExpectExceptions(bool succeedOnException, params Type[] exceptionTypes)
			: this(succeedOnException, exceptionTypes.Select((Type type) => type.FullName).ToArray())
		{
		}

		public ExpectExceptions(params string[] exceptionTypeNames)
			: this(false, exceptionTypeNames)
		{
		}

		public ExpectExceptions(params Type[] exceptionTypes)
			: this(false, exceptionTypes)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class IgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DynamicTestAttribute : Attribute
	{
		private readonly string m_SceneName;

		public DynamicTestAttribute(string sceneName)
		{
			if (sceneName.EndsWith(".unity"))
			{
				sceneName = sceneName.Substring(0, sceneName.Length - ".unity".Length);
			}
			m_SceneName = sceneName;
		}

		public bool IncludeOnScene(string sceneName)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sceneName);
			return fileNameWithoutExtension == m_SceneName;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SucceedWithAssertions : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TimeoutAttribute : Attribute
	{
		public float timeout;

		public TimeoutAttribute(float seconds)
		{
			timeout = seconds;
		}
	}

	public const string passMessage = "IntegrationTest Pass";

	public const string failMessage = "IntegrationTest Fail";

	public static void Pass()
	{
		LogResult("IntegrationTest Pass");
	}

	public static void Pass(GameObject go)
	{
		LogResult(go, "IntegrationTest Pass");
	}

	public static void Fail(string reason)
	{
		Fail();
		if (!string.IsNullOrEmpty(reason))
		{
			Debug.Log(reason);
		}
	}

	public static void Fail(GameObject go, string reason)
	{
		Fail(go);
		if (!string.IsNullOrEmpty(reason))
		{
			Debug.Log(reason);
		}
	}

	public static void Fail()
	{
		LogResult("IntegrationTest Fail");
	}

	public static void Fail(GameObject go)
	{
		LogResult(go, "IntegrationTest Fail");
	}

	public static void Assert(bool condition)
	{
		Assert(condition, "");
	}

	public static void Assert(GameObject go, bool condition)
	{
		Assert(go, condition, "");
	}

	public static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			Fail(message);
		}
	}

	public static void Assert(GameObject go, bool condition, string message)
	{
		if (!condition)
		{
			Fail(go, message);
		}
	}

	private static void LogResult(string message)
	{
		Debug.Log(message);
	}

	private static void LogResult(GameObject go, string message)
	{
		Debug.Log(message + " (" + FindTestObject(go).name + ")", go);
	}

	private static GameObject FindTestObject(GameObject go)
	{
		GameObject gameObject = go;
		while (gameObject.transform.parent != null)
		{
			if (gameObject.GetComponent("TestComponent") != null)
			{
				return gameObject;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		return go;
	}
}
