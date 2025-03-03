using Disney.LaunchPadFramework.Utility.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class CoroutineGroup
	{
		private List<ICoroutine> coroutines = new List<ICoroutine>();

		private Dictionary<ICoroutine, Action> handlerMap = new Dictionary<ICoroutine, Action>();

		public bool IsFinished
		{
			get
			{
				for (int i = 0; i < coroutines.Count; i++)
				{
					if (!coroutines[i].Cancelled && !coroutines[i].Completed)
					{
						return false;
					}
				}
				return true;
			}
		}

		public ICoroutine StartAndAdd(IEnumerator enumerator, object owner, string debugName)
		{
			ICoroutine coroutine = CoroutineRunner.Start(enumerator, owner, debugName);
			if (!coroutine.Disposed)
			{
				Add(coroutine);
			}
			return coroutine;
		}

		public ICoroutine StartAndAddPersistent(IEnumerator enumerator, object owner, string debugName)
		{
			ICoroutine coroutine = CoroutineRunner.StartPersistent(enumerator, owner, debugName);
			if (!coroutine.Disposed)
			{
				Add(coroutine);
			}
			return coroutine;
		}

		public void Add(ICoroutine coroutine)
		{
			if (!coroutine.Disposed && !coroutines.Contains(coroutine))
			{
				coroutines.Add(coroutine);
				Action value = delegate
				{
					Remove(coroutine);
				};
				handlerMap.Add(coroutine, value);
				coroutine.EDisposed += value;
			}
		}

		public void Remove(ICoroutine coroutine)
		{
			remove(coroutine, true);
		}

		public bool Contains(ICoroutine coroutine)
		{
			return coroutines.Contains(coroutine);
		}

		private void remove(ICoroutine coroutine, bool removeFromList)
		{
			if (removeFromList)
			{
				coroutines.Remove(coroutine);
			}
			if (handlerMap.ContainsKey(coroutine))
			{
				Action value = handlerMap[coroutine];
				handlerMap.Remove(coroutine);
				coroutine.EDisposed -= value;
			}
		}

		public void Clear()
		{
			MutableIterator<ICoroutine> mutableIterator = new MutableIterator<ICoroutine>();
			mutableIterator.Begin(coroutines);
			while (mutableIterator.IsValid())
			{
				remove(mutableIterator.Current, false);
				mutableIterator.RemoveCurrent();
				mutableIterator.Next();
			}
			coroutines.Clear();
			handlerMap.Clear();
		}

		public void StopAll()
		{
			foreach (ICoroutine coroutine in coroutines)
			{
				coroutine.Cancel();
			}
			Clear();
		}
	}
}
