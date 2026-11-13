using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class SnowballAimingRays : MonoBehaviour
	{
		public enum AimingType
		{
			None,
			TrailMarker,
			ParticleMarkers
		}

		public AimingType aimingType = AimingType.None;

		public Transform trailTransformPrefab = null;

		public ParticleSystem particleMarkerPrefab = null;

		private Transform trailTransform = null;

		private float yMax = 20f;

		private float vertStep = 0.45f;

		private float startDelay = 0.75f;

		private float sequenceIntervalDelay = 2f;

		private float scanIntervalDelay = 0.0025f;

		private bool isPlaying = false;

		private float timeStamp = 0f;

		private LocalPenguinSnowballThrower penguinSnowballThrow;

		private void Awake()
		{
			penguinSnowballThrow = GetComponent<LocalPenguinSnowballThrower>();
		}

		public void Init()
		{
			switch (aimingType)
			{
			case AimingType.None:
				break;
			case AimingType.TrailMarker:
				if (!isPlaying)
				{
					StartCoroutine("StartSequence");
				}
				break;
			case AimingType.ParticleMarkers:
				if (!isPlaying)
				{
					StartCoroutine("StartSequence");
				}
				break;
			}
		}

		public void Update()
		{
			if (isPlaying && aimingType != 0 && Time.time - timeStamp >= sequenceIntervalDelay)
			{
				if (penguinSnowballThrow.IsHoldingSnowball())
				{
					StartCoroutine("FireRays");
					timeStamp = Time.time;
				}
				else
				{
					isPlaying = false;
				}
			}
		}

		private IEnumerator StartSequence()
		{
			yield return new WaitForSeconds(startDelay);
			if (penguinSnowballThrow.IsHoldingSnowball())
			{
				timeStamp = Time.time;
				StartCoroutine("FireRays");
				isPlaying = true;
			}
			else
			{
				isPlaying = false;
			}
		}

		private IEnumerator FireRays()
		{
			if (trailTransformPrefab == null)
			{
				yield return null;
			}
			switch (aimingType)
			{
			case AimingType.TrailMarker:
			{
				trailTransform = Object.Instantiate(trailTransformPrefab);
				Vector3 fwd2 = base.transform.TransformDirection(Vector3.forward) + new Vector3(0f, -0.25f, 0f);
				for (float i = 0f; i < yMax; i += vertStep)
				{
					Vector3 origin = new Vector3(base.transform.position.x, i, base.transform.position.z);
					RaycastHit hit;
					if (Physics.Raycast(origin, fwd2, out hit, 100f))
					{
						ShakeTarget(hit.collider);
						trailTransform.position = hit.point + -(fwd2.normalized * 0.25f);
					}
					yield return new WaitForSeconds(scanIntervalDelay);
				}
				Object.Destroy(trailTransform.gameObject);
				break;
			}
			case AimingType.ParticleMarkers:
			{
				Vector3 fwd2 = base.transform.TransformDirection(Vector3.forward) + new Vector3(0f, -0.25f, 0f);
				for (float j = 0f; j < yMax; j += vertStep)
				{
					Vector3 origin2 = new Vector3(base.transform.position.x, j, base.transform.position.z);
					RaycastHit hit2;
					if (Physics.Raycast(origin2, fwd2, out hit2, 100f))
					{
						ShakeTarget(hit2.collider);
						Vector3 position = hit2.point + -(fwd2.normalized * 0.25f);
						ParticleSystem particleSystem = Object.Instantiate(particleMarkerPrefab);
						particleSystem.transform.position = position;
						particleSystem.Play();
					}
					yield return new WaitForSeconds(scanIntervalDelay);
				}
				break;
			}
			}
		}

		private void ShakeTarget(Collider col)
		{
			if (col.CompareTag("IslandTarget"))
			{
				col.gameObject.GetComponent<IslandTarget>().ShakeTarget(new Vector3(0.25f, 0.25f, 0.25f));
			}
		}

		public void StopPlaying()
		{
			StopCoroutine("FireRays");
			if (trailTransform != null)
			{
				Object.Destroy(trailTransform.gameObject);
			}
			isPlaying = false;
		}
	}
}
