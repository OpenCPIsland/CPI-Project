using System;
using UnityEngine;

namespace ClubPenguin.Props
{
	[DisallowMultipleComponent]
	public class PropExperience : MonoBehaviour
	{
		public string InstanceId;

		public long OwnerId;

		public bool IsOwnerLocalPlayer;

		public PropDefinition PropDef;

		public float DestroyAfterSec = -1f;

		public event Action<string, long, bool, PropDefinition> PropExperienceStarted;

		public void StartExperience()
		{
			if (string.IsNullOrEmpty(InstanceId))
			{
				throw new InvalidOperationException("Instance id not set! Cannot start experience");
			}
			if (this.PropExperienceStarted != null)
			{
				this.PropExperienceStarted(InstanceId, OwnerId, IsOwnerLocalPlayer, PropDef);
			}
			if (DestroyAfterSec >= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject, DestroyAfterSec);
			}
		}

		public void OnDestroy()
		{
			this.PropExperienceStarted = null;
		}
	}
}
