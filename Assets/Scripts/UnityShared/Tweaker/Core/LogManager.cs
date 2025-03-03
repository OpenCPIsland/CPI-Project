using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tweaker.Core
{
	public static class LogManager
	{
		public static ITweakerLogManager Instance
		{
			get;
			private set;
		}

		public static void Set(ITweakerLogManager instance)
		{
			Instance = instance;
		}

		static LogManager()
		{
			Set(new DummyLogManager());
		}

		public static ITweakerLogger GetLogger(string name)
		{
			return Instance.GetLogger(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static ITweakerLogger GetCurrentClassLogger()
		{
			return Instance.GetLogger(GetClassFullName());
		}

		public static ITweakerLogger GetClassLogger<T>()
		{
			return Instance.GetLogger(typeof(T).FullName);
		}

		private static string GetClassFullName()
		{
			try
			{
				int num = 0;
				Type declaringType;
				string text;
				do
				{
					StackFrame stackFrame = new StackFrame(num, false);
					MethodBase method = stackFrame.GetMethod();
					declaringType = method.DeclaringType;
					if (declaringType == null)
					{
						text = method.Name;
						break;
					}
					num++;
					text = declaringType.FullName;
				}
				while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase) || text.Equals(typeof(LogManager).FullName, StringComparison.Ordinal));
				return text;
			}
			catch (Exception)
			{
				return "Unknown";
			}
		}
	}
}
