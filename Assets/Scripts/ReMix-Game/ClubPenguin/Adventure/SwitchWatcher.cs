using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/Switch")]
	public class SwitchWatcher : TaskWatcher
	{
		public string SwitchName;

		private Transform owner;

		public override object GetExportParameters()
		{
			return ExportedSwitch.Create(GameObject.Find(SwitchName).GetComponent<Switch>());
		}

		public override string GetWatcherType()
		{
			return "switch";
		}

		public override void OnActivate()
		{
			base.OnActivate();
			GameObject gameObject = GameObject.Find(SwitchName);
			if (gameObject != null)
			{
				owner = gameObject.transform;
				base.dispatcher.AddListener<SwitchEvents.SwitchChange>(onSwitchChange);
			}
			else
			{
				owner = null;
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			if (owner != null)
			{
				base.dispatcher.RemoveListener<SwitchEvents.SwitchChange>(onSwitchChange);
			}
		}

		private bool onSwitchChange(SwitchEvents.SwitchChange evt)
		{
			if (evt.Owner == owner && evt.Value)
			{
				taskIncrement();
			}
			return false;
		}
	}
}
