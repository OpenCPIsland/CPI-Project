#define UNITY_ASSERTIONS
using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Props
{
	public class PropProjectile : MonoBehaviour
	{
		public enum ProjectileMode
		{
			Calculated,
			Physics,
			Animated
		}

		public ProjectileMode Mode = ProjectileMode.Calculated;

		public float ArcDelta = 1f;

		[Range(0.01f, 10f)]
		public float TravelTimeSec = 1f;

		public Vector3 WorldStart;

		public Vector3 WorldDestination;

		public float DestroyDelaySec = 1.45f;

		public float HorizontalTorque = 10f;

		public float VerticalTorque = 90f;

		public Vector3 Force = new Vector3(0f, 440f, 0f);

		[HideInInspector]
		public bool RevealSpawned = false;

		[HideInInspector]
		public PropExperience SpawnedPropExperience;

		private Transform transformRef;

		private Vector3 startPosition;

		private Vector3 startToDestination;

		private float halfTravelTime;

		private float halfTravelTimeRecip;

		private float travelTime;

		private float travelTimeRecip;

		private float elapsedTime;

		private bool isTravelComplete;

		private void Start()
		{
			transformRef = base.transform;
			startPosition = WorldStart;
			transformRef.position = startPosition;
			startToDestination = WorldDestination - WorldStart;
			switch (Mode)
			{
			case ProjectileMode.Calculated:
			{
				travelTime = TravelTimeSec;
				halfTravelTime = travelTime * 0.5f;
				halfTravelTimeRecip = 1f / halfTravelTime;
				travelTimeRecip = 1f / travelTime;
				elapsedTime = 0f;
				isTravelComplete = false;
				Rigidbody component = GetComponent<Rigidbody>();
				if (component != null)
				{
					component.useGravity = false;
				}
				Collider component2 = GetComponent<Collider>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
				break;
			}
			case ProjectileMode.Physics:
				Throw();
				break;
			}
		}

		public void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void Throw()
		{
			Assert.IsTrue(Mode == ProjectileMode.Physics);
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			base.gameObject.transform.parent = null;
			base.gameObject.transform.rotation = Quaternion.Euler(eulerAngles);
			Collider component = GetComponent<Collider>();
			component.enabled = true;
			Rigidbody component2 = GetComponent<Rigidbody>();
			component2.AddTorque(base.transform.up * HorizontalTorque);
			component2.AddTorque(base.transform.right * VerticalTorque);
			component2.AddRelativeForce(Force);
			CoroutineRunner.Start(destroyAfterDelay(), this, "destroyAfterDelay");
		}

		private void Update()
		{
			if (Mode != 0 || transformRef == null || isTravelComplete)
			{
				return;
			}
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= travelTime)
			{
				isTravelComplete = true;
				elapsedTime = travelTime;
			}
			float num = 1f - Mathf.Abs((elapsedTime - halfTravelTime) * halfTravelTimeRecip);
			float num2 = ArcDelta * num;
			float d = elapsedTime * travelTimeRecip;
			Vector3 position = startPosition + startToDestination * d;
			position.y += num2;
			transformRef.position = position;
			if (isTravelComplete)
			{
				if (RevealSpawned && SpawnedPropExperience != null)
				{
					SpawnedPropExperience.gameObject.SetActive(true);
					SpawnedPropExperience.StartExperience();
				}
				transformRef = null;
				CoroutineRunner.Start(destroyAfterDelay(), this, "destroyAfterDelay");
			}
		}

		private IEnumerator destroyAfterDelay()
		{
			yield return new WaitForSeconds(DestroyDelaySec);
			if (RevealSpawned && SpawnedPropExperience != null)
			{
				if (Mode == ProjectileMode.Physics)
				{
					SpawnedPropExperience.gameObject.transform.position = base.transform.position;
				}
				SpawnedPropExperience.gameObject.SetActive(true);
				SpawnedPropExperience.StartExperience();
			}
			Object.Destroy(base.gameObject);
		}
	}
}
