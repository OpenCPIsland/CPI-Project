using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class BackButtonController : MonoBehaviour
	{
		public IRootLevelBackAction rootLevelBackAction;

		private readonly List<Action> backStack = new List<Action>();

		private bool isEnabled = true;

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				isEnabled = value;
			}
		}

		public void Add(Action callback)
		{
			backStack.Add(callback);
		}

		public void Remove(Action callback, bool invokeRemovedCallback = false)
		{
			backStack.Remove(callback);
			if (callback != null && invokeRemovedCallback)
			{
				callback();
			}
		}

		public void Execute()
		{
			if (isEnabled && backStack.Count > 0)
			{
				Action action = backStack[backStack.Count - 1];
				if (action != null && rootLevelBackAction != null && rootLevelBackAction.GetType() == action.Method.DeclaringType)
				{
					action();
				}
				else
				{
					Remove(action, true);
				}
			}
		}

		public int NumCallbacks()
		{
			return backStack.Count;
		}
	}
}
