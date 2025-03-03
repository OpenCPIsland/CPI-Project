using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class InputMapGroup : InputMapLib
	{
		[SerializeField]
		private List<InputMapLib> maps = new List<InputMapLib>();

		public override void Initialize()
		{
			foreach (InputMapLib map in maps)
			{
				map.OnEnabledToggled += onInputMapEnabledToggle;
				map.Initialize();
			}
			base.Initialize();
		}

		private void onInputMapEnabledToggle(bool toggle)
		{
			if (toggle)
			{
				base.Enabled |= autoEnabled;
			}
			else if (autoEnabled)
			{
				bool flag = true;
				foreach (InputMapLib map in maps)
				{
					flag &= map.Enabled;
				}
				if (flag)
				{
					base.Enabled = false;
				}
			}
		}

		public override bool ProcessInput(ControlScheme controlScheme)
		{
			if (!base.Enabled)
			{
				return false;
			}
			bool flag = false;
			foreach (InputMapLib map in maps)
			{
				flag = (map.ProcessInput(controlScheme) || flag);
			}
			return flag || base.InputBlocker;
		}

		public override void ResetInput()
		{
			foreach (InputMapLib map in maps)
			{
				map.ResetInput();
			}
		}
	}
}
