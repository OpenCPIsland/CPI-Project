using DisneyMobile.CoreUnitySystems.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class SignalGroupAll : Signal
	{
		public List<Signal> mSignals = new List<Signal>();

		public override bool IsSignaled()
		{
			foreach (Signal mSignal in mSignals)
			{
				if (!mSignal.IsSignaled())
				{
					return false;
				}
			}
			if (mSignals.Count == 0)
			{
				return false;
			}
			return true;
		}

		public void OnEnable()
		{
			RefreshChildrenSignalMapping();
		}

		public override void ActivateSignal()
		{
			foreach (Signal mSignal in mSignals)
			{
				mSignal.ActivateSignal();
			}
		}

		public override void Reset()
		{
			foreach (Signal mSignal in mSignals)
			{
				mSignal.Reset();
			}
		}

		public void RefreshChildrenSignalMapping()
		{
			mSignals.Clear();
			Component[] componentsInChildren = GetComponentsInChildren(typeof(Signal));
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Signal signal = componentsInChildren[i] as Signal;
				if (signal != null && signal != this)
				{
					AddSignal(signal);
				}
			}
		}

		private void AddSignal(Signal signal)
		{
			if (signal != this)
			{
				mSignals.AddIfUnique(signal);
			}
		}
	}
}
