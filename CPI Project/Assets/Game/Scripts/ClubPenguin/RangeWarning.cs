using System.Linq;
using UnityEngine;

namespace ClubPenguin
{
	public class RangeWarning : ProximityBroadcaster
	{
		public GameObject targetObject;

		public Vector3 startScale = new Vector3(1f, 1f, 1f);

		public float startSpeedSeconds = 1f;

		public Vector3 warnScale = new Vector3(1f, 1f, 1f);

		public float warnSpeedSeconds = 1f;

		private Vector3 oldScale;

		private Vector3 newScale;

		private bool isScaling = false;

		private float lerpTime = 0f;

		private float lerpSpeed = 0f;

		private Collider[] childrenColliders;

		public override void Awake()
		{
			base.Awake();
			targetObject.transform.localScale = startScale;
			childrenColliders = GetComponentsInChildren<Collider>();
			childrenColliders = childrenColliders.Where((Collider x) => x.gameObject.GetInstanceID() != base.gameObject.GetInstanceID()).ToArray();
			updateColliders(false);
		}

		public void Update()
		{
			if (isScaling)
			{
				targetObject.transform.localScale = Vector3.Lerp(oldScale, newScale, lerpTime);
				lerpTime += lerpSpeed * Time.deltaTime;
				if (lerpTime > 1f)
				{
					isScaling = false;
				}
			}
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			oldScale = targetObject.transform.localScale;
			newScale = warnScale;
			lerpTime = 0f;
			lerpSpeed = 1f / warnSpeedSeconds;
			isScaling = true;
			updateColliders(true);
		}

		public override void OnProximityExit(ProximityListener other)
		{
			oldScale = targetObject.transform.localScale;
			newScale = startScale;
			lerpTime = 0f;
			lerpSpeed = 1f / startSpeedSeconds;
			isScaling = true;
			updateColliders(false);
		}

		private void updateColliders(bool isEnabled)
		{
			Collider[] array = childrenColliders;
			foreach (Collider collider in array)
			{
				collider.enabled = isEnabled;
			}
		}
	}
}
