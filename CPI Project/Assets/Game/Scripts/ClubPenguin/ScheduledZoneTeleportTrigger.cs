using ClubPenguin.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Collider))]
	public class ScheduledZoneTeleportTrigger : MonoBehaviour
	{
		// On scene load, it will once check for the tag "Player". If true. Checks if that date is activate. If true, does nothing. If false, teleports "Player" to the selected zone after a 2 second delay.
		public ZoneDefinition TeleportZone;

		public ScheduledEventDateDefinition Schedule;

		public float TeleportDelay = 2f;

		private bool hasRun;

		private void OnTriggerEnter(Collider other)
		{
			if (hasRun || !other.CompareTag(TagConstants.TAG_PLAYER))
			{
				return;
			}

			if (TeleportZone == null || Schedule == null)
			{
				return;
			}

			DateTime now = DateTime.UtcNow;
			if (now < Schedule.Dates.EndDate.Date)
			{
				return;
			}

			hasRun = true;
			StartCoroutine(LoadZoneAfterDelay());
		}

		private System.Collections.IEnumerator LoadZoneAfterDelay()
		{
			yield return new WaitForSeconds(TeleportDelay);
			Service.Get<ZoneTransitionService>().LoadZone(TeleportZone, "Loading");
		}
	}
}