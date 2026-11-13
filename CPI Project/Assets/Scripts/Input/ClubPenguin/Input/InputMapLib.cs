using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Input
{
	public abstract class InputMapLib : ScriptableObject
	{
		[SerializeField]
		protected bool blockInput = false;

		[SerializeField]
		protected bool autoEnabled = true;

		private bool enabled;

		[SerializeField]
		private List<InputMapLib> enabledDependencies = new List<InputMapLib>();

		public bool InputBlocker
		{
			get
			{
				return Enabled && blockInput;
			}
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					enabled = value;
					toggleEnabledDependencies(enabled);
					this.OnEnabledToggled.InvokeSafe(enabled);
				}
			}
		}

		public event Action<bool> OnEnabledToggled;

		private void toggleEnabledDependencies(bool toggle)
		{
			foreach (InputMapLib enabledDependency in enabledDependencies)
			{
				enabledDependency.Enabled = toggle;
			}
		}

		public virtual void Initialize()
		{
		}

		public abstract bool ProcessInput(ControlScheme controlScheme);

		public abstract void ResetInput();
	}
}
