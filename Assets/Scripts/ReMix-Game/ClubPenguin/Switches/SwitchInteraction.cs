using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class SwitchInteraction : Switch
	{
		public GameObject Owner;

		public GameObject Trigger;

		public override object GetSwitchParameters()
		{
			if (Owner != null && !Owner.CompareTag("Player"))
			{
				throw new NotSupportedException("No server support to export an interaction swith with the owner " + Owner.GetPath() + " in switch " + base.gameObject.GetPath());
			}
			if (Trigger == null)
			{
				Trigger = base.gameObject;
			}
			return Trigger.GetPath();
		}

		public override string GetSwitchType()
		{
			return "interaction";
		}

		public void Start()
		{
			if (Owner == null)
			{
				Owner = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			}
			if (Trigger == null)
			{
				Trigger = base.gameObject;
			}
		}

		public void Update()
		{
			if (!Owner.IsDestroyed() && SceneRefs.ActionSequencer != null)
			{
				GameObject trigger = SceneRefs.ActionSequencer.GetTrigger(Owner);
				if (trigger != null)
				{
					Change(trigger == Trigger);
				}
			}
		}
	}
}
