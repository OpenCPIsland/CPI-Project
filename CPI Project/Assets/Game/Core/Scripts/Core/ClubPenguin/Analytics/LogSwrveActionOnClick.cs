using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Analytics
{
	[RequireComponent(typeof(Button))]
	public class LogSwrveActionOnClick : MonoBehaviour
	{
		public string Tier1;

		public string Tier2;

		public string Tier3;

		public string Tier4;

		public string Context;

		public string Message;

		public string Level;

		private void Start()
		{
			Button component = GetComponent<Button>();
			component.onClick.AddListener(onClick);
		}

		private void onClick()
		{
			if (string.IsNullOrEmpty(Tier1))
			{
				throw new InvalidOperationException("The Tier1 field cannot be null or empty");
			}
			Service.Get<ICPSwrveService>().Action(Tier1, (!string.IsNullOrEmpty(Tier2)) ? Tier2 : null, (!string.IsNullOrEmpty(Tier3)) ? Tier3 : null, (!string.IsNullOrEmpty(Tier4)) ? Tier4 : null, (!string.IsNullOrEmpty(Context)) ? Context : null, (!string.IsNullOrEmpty(Message)) ? Message : null, (!string.IsNullOrEmpty(Level)) ? Level : null);
		}
	}
}
