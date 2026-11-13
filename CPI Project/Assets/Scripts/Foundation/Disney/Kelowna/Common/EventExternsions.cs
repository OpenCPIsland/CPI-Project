using Disney.LaunchPadFramework;
using System;

namespace Disney.Kelowna.Common
{
	public static class EventExternsions
	{
		public delegate void InvokeAction(Delegate d, params object[] args);

		public static void InvokeSafe(this MulticastDelegate e, InvokeAction invoke, params object[] args)
		{
			if ((object)e != null)
			{
				Delegate[] invocationList = e.GetInvocationList();
				int num = invocationList.Length;
				for (int i = 0; i < num; i++)
				{
					try
					{
						invoke(invocationList[i], args);
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(e.Target, "The event handler for delegate '{0}' threw an exception. Execution will continue but the game may be in a broken state.", e.Method);
						Log.LogException(e.Target, ex);
					}
				}
			}
		}

		public static void InvokeSafe(this Action e)
		{
			e.InvokeSafe(invokeAction);
		}

		private static void invokeAction(Delegate d, params object[] args)
		{
			((Action)d)();
		}

		public static void InvokeSafe<T>(this Action<T> e, T arg1)
		{
			e.InvokeSafe(invokeActionOneArg<T>, arg1);
		}

		private static void invokeActionOneArg<T>(Delegate d, params object[] args)
		{
			((Action<T>)d)((T)args[0]);
		}

		public static void InvokeSafe<T, U>(this Action<T, U> e, T arg1, U arg2)
		{
			e.InvokeSafe(invokeActionTwoArg<T, U>, arg1, arg2);
		}

		private static void invokeActionTwoArg<T, U>(Delegate d, params object[] args)
		{
			((Action<T, U>)d)((T)args[0], (U)args[1]);
		}

		public static void InvokeSafe<T, U, V>(this Action<T, U, V> e, T arg1, U arg2, V arg3)
		{
			e.InvokeSafe(invokeActionThreeArg<T, U, V>, arg1, arg2, arg3);
		}

		private static void invokeActionThreeArg<T, U, V>(Delegate d, params object[] args)
		{
			((Action<T, U, V>)d)((T)args[0], (U)args[1], (V)args[2]);
		}
	}
}
