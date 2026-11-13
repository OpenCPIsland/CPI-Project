using Disney.LaunchPadFramework;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Kelowna.Common
{
	public class CoroutineOwnerMap<TCoroutine> where TCoroutine : RadicalCoroutine
	{
		private readonly Dictionary<object, List<TCoroutine>> ownerToCoroutinesMap = new Dictionary<object, List<TCoroutine>>();

		public void Add(object owner, TCoroutine coroutine)
		{
			if (coroutine.Disposed || coroutine.Cancelled || coroutine.Completed)
			{
				Log.LogError(this, "Cannot add disposed coroutine to coroutine owner map : " + coroutine.TrackedName);
			}
			List<TCoroutine> value;
			if (!ownerToCoroutinesMap.TryGetValue(owner, out value))
			{
				value = new List<TCoroutine>();
				ownerToCoroutinesMap.Add(owner, value);
			}
			if (!value.Contains(coroutine))
			{
				value.Add(coroutine);
				coroutine.EDisposed += delegate
				{
					remove(owner, coroutine);
				};
			}
		}

		private void remove(object owner, TCoroutine coroutine)
		{
			List<TCoroutine> value;
			if (ownerToCoroutinesMap.TryGetValue(owner, out value))
			{
				value.RemoveAll((TCoroutine c) => c.Equals(coroutine));
				if (value.Count == 0)
				{
					ownerToCoroutinesMap.Remove(owner);
				}
			}
		}

		public void StopAllForOwner(object owner)
		{
			List<TCoroutine> value;
			if (!ownerToCoroutinesMap.TryGetValue(owner, out value))
			{
				return;
			}
			TCoroutine[] array = value.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				TCoroutine val = array[i];
				if (!val.Disposed && !val.Cancelled)
				{
					val.Stop();
				}
			}
			if (ownerToCoroutinesMap.ContainsKey(owner))
			{
				ownerToCoroutinesMap.Remove(owner);
			}
		}

		public void StopAll()
		{
			object[] array = ownerToCoroutinesMap.Keys.ToArray();
			foreach (object owner in array)
			{
				StopAllForOwner(owner);
			}
			ownerToCoroutinesMap.Clear();
		}

		public int GetCountForOwner(object owner)
		{
			List<TCoroutine> value;
			if (ownerToCoroutinesMap.TryGetValue(owner, out value))
			{
				return value.Count;
			}
			return 0;
		}

		public int GetCountForAllOwners()
		{
			int num = 0;
			foreach (KeyValuePair<object, List<TCoroutine>> item in ownerToCoroutinesMap)
			{
				num += GetCountForOwner(item.Key);
			}
			return num;
		}

		public int GetOwnerCount()
		{
			return ownerToCoroutinesMap.Count;
		}

		public void Clear()
		{
			ownerToCoroutinesMap.Clear();
		}
	}
}
