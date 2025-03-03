using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTest
{
	public class TestComponent : MonoBehaviour, ITestComponent, IComparable<ITestComponent>
	{
		[Flags]
		public enum IncludedPlatforms
		{
			WindowsEditor = 0x1,
			OSXEditor = 0x2,
			WindowsPlayer = 0x4,
			OSXPlayer = 0x8,
			LinuxPlayer = 0x10,
			MetroPlayerX86 = 0x20,
			MetroPlayerX64 = 0x40,
			MetroPlayerARM = 0x80,
			WindowsWebPlayer = 0x100,
			OSXWebPlayer = 0x200,
			Android = 0x400,
			IPhonePlayer = 0x800,
			TizenPlayer = 0x1000,
			WP8Player = 0x2000,
			BB10Player = 0x4000,
			NaCl = 0x8000,
			PS3 = 0x10000,
			XBOX360 = 0x20000,
			WiiPlayer = 0x40000,
			PSP2 = 0x80000,
			PS4 = 0x100000,
			PSMPlayer = 0x200000,
			XboxOne = 0x400000
		}

		private sealed class NullTestComponentImpl : ITestComponent, IComparable<ITestComponent>
		{
			public GameObject gameObject
			{
				get;
				private set;
			}

			public string Name
			{
				get
				{
					return "";
				}
			}

			public int CompareTo(ITestComponent other)
			{
				if (other == this)
				{
					return 0;
				}
				return -1;
			}

			public void EnableTest(bool enable)
			{
			}

			public bool IsTestGroup()
			{
				throw new NotImplementedException();
			}

			public ITestComponent GetTestGroup()
			{
				return null;
			}

			public bool IsExceptionExpected(string exceptionType)
			{
				throw new NotImplementedException();
			}

			public bool ShouldSucceedOnException()
			{
				throw new NotImplementedException();
			}

			public double GetTimeout()
			{
				throw new NotImplementedException();
			}

			public bool IsIgnored()
			{
				throw new NotImplementedException();
			}

			public bool ShouldSucceedOnAssertions()
			{
				throw new NotImplementedException();
			}

			public bool IsExludedOnThisPlatform()
			{
				throw new NotImplementedException();
			}
		}

		public static ITestComponent NullTestComponent = new NullTestComponentImpl();

		public float timeout = 5f;

		public bool ignored = false;

		public bool succeedAfterAllAssertionsAreExecuted = false;

		public bool expectException = false;

		public string expectedExceptionList = "";

		public bool succeedWhenExceptionIsThrown = false;

		public IncludedPlatforms includedPlatforms = (IncludedPlatforms)(-1);

		public string[] platformsToIgnore = null;

		public bool dynamic;

		public string dynamicTypeName;

		public string Name
		{
			get
			{
				return (base.gameObject == null) ? "" : base.gameObject.name;
			}
		}

		public bool IsExludedOnThisPlatform()
		{
			return platformsToIgnore != null && platformsToIgnore.Any((string platform) => platform == Application.platform.ToString());
		}

		private static bool IsAssignableFrom(Type a, Type b)
		{
			return a.IsAssignableFrom(b);
		}

		public bool IsExceptionExpected(string exception)
		{
			exception = exception.Trim();
			if (!expectException)
			{
				return false;
			}
			if (string.IsNullOrEmpty(expectedExceptionList.Trim()))
			{
				return true;
			}
			foreach (string item in from e in expectedExceptionList.Split(',')
				select e.Trim())
			{
				if (exception == item)
				{
					return true;
				}
				Type type = Type.GetType(exception) ?? GetTypeByName(exception);
				Type type2 = Type.GetType(item) ?? GetTypeByName(item);
				if (type != null && type2 != null && IsAssignableFrom(type2, type))
				{
					return true;
				}
			}
			return false;
		}

		public bool ShouldSucceedOnException()
		{
			return succeedWhenExceptionIsThrown;
		}

		public double GetTimeout()
		{
			return timeout;
		}

		public bool IsIgnored()
		{
			return ignored;
		}

		public bool ShouldSucceedOnAssertions()
		{
			return succeedAfterAllAssertionsAreExecuted;
		}

		private static Type GetTypeByName(string className)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => a.GetTypes()).FirstOrDefault((Type type) => type.Name == className);
		}

		public void OnValidate()
		{
			if (timeout < 0.01f)
			{
				timeout = 0.01f;
			}
		}

		public void EnableTest(bool enable)
		{
			if (enable && dynamic)
			{
				Type type = Type.GetType(dynamicTypeName);
				MonoBehaviour monoBehaviour = base.gameObject.GetComponent(type) as MonoBehaviour;
				if (monoBehaviour != null)
				{
					UnityEngine.Object.DestroyImmediate(monoBehaviour);
				}
				base.gameObject.AddComponent(type);
			}
			if (base.gameObject.activeSelf != enable)
			{
				base.gameObject.SetActive(enable);
			}
		}

		public int CompareTo(ITestComponent obj)
		{
			if (obj == NullTestComponent)
			{
				return 1;
			}
			int num = base.gameObject.name.CompareTo(obj.gameObject.name);
			if (num == 0)
			{
				num = base.gameObject.GetInstanceID().CompareTo(obj.gameObject.GetInstanceID());
			}
			return num;
		}

		public bool IsTestGroup()
		{
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				Component component = base.gameObject.transform.GetChild(i).GetComponent(typeof(TestComponent));
				if (component != null)
				{
					return true;
				}
			}
			return false;
		}

		public ITestComponent GetTestGroup()
		{
			Transform parent = base.gameObject.transform.parent;
			if (parent == null)
			{
				return NullTestComponent;
			}
			return parent.GetComponent<TestComponent>();
		}

		public override bool Equals(object o)
		{
			if (o is TestComponent)
			{
				return this == o as TestComponent;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(TestComponent a, TestComponent b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			if (a.dynamic && b.dynamic)
			{
				return a.dynamicTypeName == b.dynamicTypeName;
			}
			if (a.dynamic || b.dynamic)
			{
				return false;
			}
			return a.gameObject == b.gameObject;
		}

		public static bool operator !=(TestComponent a, TestComponent b)
		{
			return !(a == b);
		}

		public static TestComponent CreateDynamicTest(Type type)
		{
			GameObject gameObject = CreateTest(type.Name);
			gameObject.hideFlags |= HideFlags.DontSave;
			gameObject.SetActive(false);
			TestComponent component = gameObject.GetComponent<TestComponent>();
			component.dynamic = true;
			component.dynamicTypeName = type.AssemblyQualifiedName;
			object[] customAttributes = type.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj is IntegrationTest.TimeoutAttribute)
				{
					component.timeout = (obj as IntegrationTest.TimeoutAttribute).timeout;
				}
				else if (obj is IntegrationTest.IgnoreAttribute)
				{
					component.ignored = true;
				}
				else if (obj is IntegrationTest.SucceedWithAssertions)
				{
					component.succeedAfterAllAssertionsAreExecuted = true;
				}
				else if (obj is IntegrationTest.ExcludePlatformAttribute)
				{
					component.platformsToIgnore = (obj as IntegrationTest.ExcludePlatformAttribute).platformsToExclude;
				}
				else if (obj is IntegrationTest.ExpectExceptions)
				{
					IntegrationTest.ExpectExceptions expectExceptions = obj as IntegrationTest.ExpectExceptions;
					component.expectException = true;
					component.expectedExceptionList = string.Join(",", expectExceptions.exceptionTypeNames);
					component.succeedWhenExceptionIsThrown = expectExceptions.succeedOnException;
				}
			}
			gameObject.AddComponent(type);
			return component;
		}

		public static GameObject CreateTest()
		{
			return CreateTest("New Test");
		}

		private static GameObject CreateTest(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.AddComponent<TestComponent>();
			return gameObject;
		}

		public static List<TestComponent> FindAllTestsOnScene()
		{
			IEnumerable<TestComponent> source = Resources.FindObjectsOfTypeAll(typeof(TestComponent)).Cast<TestComponent>();
			return source.ToList();
		}

		public static List<TestComponent> FindAllTopTestsOnScene()
		{
			return (from component in FindAllTestsOnScene()
				where component.gameObject.transform.parent == null
				select component).ToList();
		}

		public static List<TestComponent> FindAllDynamicTestsOnScene()
		{
			return (from t in FindAllTestsOnScene()
				where t.dynamic
				select t).ToList();
		}

		public static void DestroyAllDynamicTests()
		{
			foreach (TestComponent item in FindAllDynamicTestsOnScene())
			{
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}

		public static void DisableAllTests()
		{
			foreach (TestComponent item in FindAllTestsOnScene())
			{
				item.EnableTest(false);
			}
		}

		public static bool AnyTestsOnScene()
		{
			return FindAllTestsOnScene().Any();
		}

		public static bool AnyDynamicTestForCurrentScene()
		{
			return GetTypesWithHelpAttribute(SceneManager.GetActiveScene().name).Any();
		}

		public static IEnumerable<Type> GetTypesWithHelpAttribute(string sceneName)
		{
			try
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					Type[] types = null;
					try
					{
						types = assembly.GetTypes();
					}
					catch (ReflectionTypeLoadException ex)
					{
						Debug.LogError("Failed to load types from: " + assembly.FullName);
						Exception[] loaderExceptions = ex.LoaderExceptions;
						foreach (Exception exception in loaderExceptions)
						{
							Debug.LogException(exception);
						}
					}
					if (types != null)
					{
						try
						{
							Type[] array = types;
							foreach (Type type in array)
							{
								object[] attributes = type.GetCustomAttributes(typeof(IntegrationTest.DynamicTestAttribute), true);
								if (attributes.Length == 1)
								{
									IntegrationTest.DynamicTestAttribute a = attributes.Single() as IntegrationTest.DynamicTestAttribute;
									if (a.IncludeOnScene(sceneName))
									{
										yield return type;
									}
								}
							}
						}
						finally
						{
						}
					}
				}
			}
			finally
			{
			}
		}

	/*	GameObject ITestComponent.get_gameObject()
		{
			return base.gameObject;
		}*/
	}
}
