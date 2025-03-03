using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class MovementRotation : ProximityBroadcaster
	{
		[Tooltip("Transform to use as center of rotation")]
		public Transform rotationCenter;

		[Tooltip("Seconds to complete 1 revolution")]
		public float secondsPerRotation = 10f;

		[Tooltip("Keep object facing original rotation")]
		public bool doNotRotateObject = true;

		public bool isControlledByParent = false;

		public bool isActive = false;

		private Vector3 centerPosition;

		private Vector3 centerDistance;

		private INetworkServicesManager network;

		public override void Awake()
		{
			base.Awake();
			centerPosition = rotationCenter.position;
			centerDistance = base.gameObject.transform.position - centerPosition;
		}

		private void OnEnable()
		{
			network = Service.Get<INetworkServicesManager>();
		}

		private void FixedUpdate()
		{
			if (network != null && isActive)
			{
				float num = 360f / secondsPerRotation * ((float)(network.GameTimeMilliseconds % (int)(secondsPerRotation * 1000f)) / 1000f);
				base.gameObject.transform.position = centerPosition + Quaternion.AngleAxis(num - 90f, Vector3.forward) * centerDistance;
				if (!doNotRotateObject)
				{
					base.gameObject.transform.rotation = Quaternion.AngleAxis(num, Vector3.forward);
				}
			}
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			if (!isControlledByParent)
			{
				isActive = true;
			}
		}

		public override void OnProximityExit(ProximityListener other)
		{
			if (!isControlledByParent)
			{
				isActive = false;
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			if (rotationCenter != null)
			{
				Gizmos.color = Color.gray;
				Gizmos.DrawWireSphere(rotationCenter.position, Vector3.Distance(base.transform.position, rotationCenter.position));
			}
		}

		public override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (rotationCenter != null)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, rotationCenter.position);
			}
		}

		public void SetActive(bool active)
		{
			isActive = active;
		}
	}
}
