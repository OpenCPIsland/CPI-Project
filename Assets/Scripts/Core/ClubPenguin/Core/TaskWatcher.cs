using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	public abstract class TaskWatcher : ScriptableObject
	{
		public string CriteriaSwitchName;

		private Switch criteriaSwitch;

		protected ITask task
		{
			get;
			private set;
		}

		protected EventDispatcher dispatcher
		{
			get;
			private set;
		}

		public void Init(ITask task)
		{
			this.task = task;
			dispatcher = Service.Get<EventDispatcher>();
		}

		public virtual void OnActivate()
		{
			if (!string.IsNullOrEmpty(CriteriaSwitchName))
			{
				GameObject gameObject = GameObject.Find(CriteriaSwitchName);
				if (gameObject != null)
				{
					criteriaSwitch = gameObject.GetComponent<Switch>();
				}
			}
		}

		public virtual void OnDeactivate()
		{
			criteriaSwitch = null;
		}

		protected void taskIncrement()
		{
			if (string.IsNullOrEmpty(CriteriaSwitchName) || (criteriaSwitch != null && criteriaSwitch.OnOff))
			{
				task.Increment();
			}
		}

		public abstract object GetExportParameters();

		public abstract string GetWatcherType();
	}
}
