using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	public class LogSwrveActionOnMethod : MonoBehaviour
	{
		[Header("Choose which method(s) to log the action")]
		public bool LogOnStart = false;

		public bool LogOnEnable = false;

		public bool LogOnDisable = false;

		[Tooltip("Consider logging on disable unless you must do so in destroy")]
		public bool LogOnDestroy = false;

		[Tooltip("Payload #1")]
		[Header("Swrve specific data. Be sure to correlate with the BI spec")]
		public string Tier1;

		public string Tier2;

		public string Tier3;

		public string Tier4;

		public string Context;

		public string Message;

		public string Level;

		public void LogAction()
		{
			if (string.IsNullOrEmpty(Tier1))
			{
				throw new InvalidOperationException("The Tier1 field cannot be null or empty");
			}
			Service.Get<ICPSwrveService>().Action(Tier1, (!string.IsNullOrEmpty(Tier2)) ? Tier2 : null, (!string.IsNullOrEmpty(Tier3)) ? Tier3 : null, (!string.IsNullOrEmpty(Tier4)) ? Tier4 : null, (!string.IsNullOrEmpty(Context)) ? Context : null, (!string.IsNullOrEmpty(Message)) ? Message : null, (!string.IsNullOrEmpty(Level)) ? Level : null);
		}

		private void Start()
		{
			if (LogOnStart)
			{
				LogAction();
			}
		}

		private void OnEnable()
		{
			if (LogOnEnable)
			{
				LogAction();
			}
		}

		private void OnDisable()
		{
			if (LogOnDisable)
			{
				LogAction();
			}
		}

		private void OnDestroy()
		{
			if (LogOnDestroy)
			{
				LogAction();
			}
		}
	}
}
