using Fabric;
using System;
using UnityEngine;

namespace ClubPenguin.Audio
{
	internal class AudioEffects : MonoBehaviour
	{
		[Serializable]
		public struct OnCollisionEntry
		{
			public string EventName;

			public float MinRelativeVelocity;

			public float MaxRelativeVelocity;
		}

		public OnCollisionEntry[] OnCollision = new OnCollisionEntry[0];

		private float minRelativeVelSq;

		public void OnCollisionEnter(Collision collision)
		{
			float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
			int num = 0;
			while (true)
			{
				if (num < OnCollision.Length)
				{
					if (sqrMagnitude >= OnCollision[num].MinRelativeVelocity * OnCollision[num].MinRelativeVelocity && sqrMagnitude < OnCollision[num].MaxRelativeVelocity * OnCollision[num].MaxRelativeVelocity)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			EventManager.Instance.PostEvent(OnCollision[num].EventName, EventAction.PlaySound, base.gameObject);
		}
	}
}
