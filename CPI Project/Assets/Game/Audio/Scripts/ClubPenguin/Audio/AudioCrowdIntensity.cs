using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Fabric;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Audio
{
	[RequireComponent(typeof(SphereCollider))]
	internal class AudioCrowdIntensity : MonoBehaviour
	{
		private static readonly int maxColliders = 16;

		private static readonly float sampleTime = 3f;

		public string EventName;

		public string ParticipantCountRTP;

		private int layerMask;

		private int count;

		private Vector3 spherePos;

		private float sphereRadius;

		private Collider[] colliders = new Collider[maxColliders];

		public void Awake()
		{
			SphereCollider component = GetComponent<SphereCollider>();
			spherePos = base.transform.position + component.center;
			sphereRadius = component.radius;
			count = 0;
			layerMask = LayerConstants.GetAllPlayersLayerCollisionMask();
			CoroutineRunner.Start(updateCount(), this, "ObjectiveListener");
		}

		private IEnumerator updateCount()
		{
			while (true)
			{
				count = Physics.OverlapSphereNonAlloc(spherePos, sphereRadius, colliders, layerMask);
				if (!string.IsNullOrEmpty(EventName))
				{
					EventManager.Instance.SetParameter(EventName, ParticipantCountRTP, count, base.gameObject);
				}
				yield return new WaitForSeconds(sampleTime);
			}
		}
	}
}
