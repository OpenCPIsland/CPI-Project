using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class LocomotionData : ScopedData
	{
		[SerializeField]
		private LocomotionState? locoState = null;

		public LocomotionState LocoState
		{
			get
			{
				return locoState.Value;
			}
			set
			{
				if (!locoState.HasValue || locoState.Value != value)
				{
					changedLocoState(value);
					locoState = value;
				}
			}
		}

		public bool LocomotionStateIsInitialized
		{
			get
			{
				return locoState.HasValue;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(LocomotionDataMonoBehaviour);
			}
		}

		public event Action<LocomotionState> PlayerLocoStateChanged;

		protected override void notifyWillBeDestroyed()
		{
			this.PlayerLocoStateChanged = null;
		}

		private void changedLocoState(LocomotionState style)
		{
			if (this.PlayerLocoStateChanged != null)
			{
				this.PlayerLocoStateChanged(style);
			}
		}
	}
}
