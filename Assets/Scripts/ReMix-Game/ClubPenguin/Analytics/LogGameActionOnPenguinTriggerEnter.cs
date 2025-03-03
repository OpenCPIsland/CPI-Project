using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	[RequireComponent(typeof(Collider))]
	public class LogGameActionOnPenguinTriggerEnter : MonoBehaviour
	{
		public string Context;

		public string Action;

		public string Message;

		public string Type;

		public string Location;

		private void OnTriggerEnter(Collider triggeringCollider)
		{
			if (triggeringCollider.CompareTag("Player"))
			{
				Service.Get<ICPSwrveService>().Action("game." + Context, Action, Type, Location, Message);
			}
		}
	}
}
