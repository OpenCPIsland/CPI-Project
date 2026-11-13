using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	public class LogSwrveActionOnStart : MonoBehaviour
	{
		[Header("Consider using LogSwrveActionOnMethod instead as it is more flexible.")]
		public string Tier1;

		public string Tier2;

		public string Tier3;

		public string Tier4;

		public string Context;

		public string Message;

		public string Level;

		private void Start()
		{
			if (string.IsNullOrEmpty(Tier1))
			{
				throw new InvalidOperationException("The Tier1 field cannot be null or empty");
			}
			Service.Get<ICPSwrveService>().Action(Tier1, (!string.IsNullOrEmpty(Tier2)) ? Tier2 : null, (!string.IsNullOrEmpty(Tier3)) ? Tier3 : null, (!string.IsNullOrEmpty(Tier4)) ? Tier4 : null, (!string.IsNullOrEmpty(Context)) ? Context : null, (!string.IsNullOrEmpty(Message)) ? Message : null, (!string.IsNullOrEmpty(Level)) ? Level : null);
		}
	}
}
