using System;
using System.Collections.Generic;

namespace ClubPenguin.Core
{
	public static class SceneRefs
	{
		private interface ISceneRefWrapper
		{
			object Instance
			{
				get;
			}

			void Unset();
		}

		private class SceneRefWrapper<T> : ISceneRefWrapper
		{
			public static T instance = default(T);

			public object Instance
			{
				get
				{
					return instance;
				}
			}

			public void Unset()
			{
				instance = default(T);
			}
		}

		private static List<ISceneRefWrapper> refWrapperList;

		public static void Set<T>(T instance)
		{
			if (SceneRefWrapper<T>.instance != null)
			{
				throw new InvalidOperationException(string.Format("An instance of this reference class {0} has already been set!", typeof(T).FullName));
			}
			SceneRefWrapper<T>.instance = instance;
			if (refWrapperList == null)
			{
				refWrapperList = new List<ISceneRefWrapper>();
			}
			refWrapperList.Add(new SceneRefWrapper<T>());
		}

		public static T Get<T>()
		{
			if (!IsSet<T>())
			{
				throw new InvalidOperationException(string.Format("Cannot find reference: {0}", typeof(T).FullName));
			}
			return SceneRefWrapper<T>.instance;
		}

		public static bool IsSet<T>()
		{
			return SceneRefWrapper<T>.instance != null;
		}

		public static void Remove<T>(T instance)
		{
			if (!IsSet<T>())
			{
				throw new InvalidOperationException(string.Format("Cannot find reference: {0}", typeof(T).FullName));
			}
			int num = 0;
			while (true)
			{
				if (num < refWrapperList.Count)
				{
					if (refWrapperList[num].Instance is T && object.ReferenceEquals(refWrapperList[num].Instance, instance))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			refWrapperList[num].Unset();
			refWrapperList.RemoveAt(num);
		}

		public static void ClearAll()
		{
			if (refWrapperList != null)
			{
				for (int num = refWrapperList.Count - 1; num >= 0; num--)
				{
					refWrapperList[num].Unset();
				}
				refWrapperList.Clear();
			}
		}
	}
}
